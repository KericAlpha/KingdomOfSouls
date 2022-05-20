using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleSystem : MonoBehaviour
{
    [SerializeField] BattleUnit playerUnit;
    [SerializeField] BattleUnit enemyUnit;
    [SerializeField] BattleDialogueBox dialogueBox;
    [SerializeField] PartyScreen partyScreen;

    public event Action<bool> OnBattleOver;

    BattleState state;
    BattleState? prevState; 
    int currentAction;
    int currentMove;
    int currentMember; 

    Party playerParty;
    Unit enemy;

    public void StartBattle(Party playerParty, Unit enemy)
    {
        this.playerParty = playerParty;
        this.enemy = enemy;
        StartCoroutine(SetupBattle());
    }

    public IEnumerator SetupBattle()
    {
        playerUnit.Setup(playerParty.GetNotFaintedUnit());
        enemyUnit.Setup(enemy);

        partyScreen.Init();

        dialogueBox.SetMoveNames(playerUnit.Unit.Moves);

        yield return dialogueBox.TypeDialogue($"What is {enemyUnit.Unit.UnitBase.Name} doing here?");
        yield return new WaitForSeconds(1f);

        ActionSelection();
    } 

    void BattleOver(bool won)
    {
        state = BattleState.BattleOver;
        playerParty.Units.ForEach(unit => unit.OnBattleOver());
        OnBattleOver(won);
    }

    void ActionSelection()
    {
        state = BattleState.ActionSelection;
        StartCoroutine(dialogueBox.TypeDialogue("Choose an action"));
        dialogueBox.EnableActionSelector(true);
    }

    void OpenPartyScreen()
    {
        state = BattleState.PartyScreen;
        partyScreen.SetPartyData(playerParty.Units);
        partyScreen.gameObject.SetActive(true);  
    }

    void MoveSelection()
    {
        state = BattleState.MoveSelection;
        currentAction = 0;
        currentMove = 0;
        dialogueBox.EnableActionSelector(false);
        dialogueBox.EnableDialogueText(false);
        dialogueBox.EnableMoveSelector(true);
    }

    IEnumerator RunTurns(BattleAction playerAction)
    {
        state = BattleState.RunningTurn;

        if(playerAction == BattleAction.Move)
        {
            playerUnit.Unit.CurrentMove = playerUnit.Unit.Moves[currentMove];
            enemyUnit.Unit.CurrentMove = enemyUnit.Unit.GetRandomMove();

            // Chech who is first
            bool playerGoesFirst = playerUnit.Unit.Speed >= enemyUnit.Unit.Speed;

            var firstUnit = (playerGoesFirst) ? playerUnit : enemyUnit;
            var secondUnit = (playerGoesFirst) ? enemyUnit : playerUnit;

            var nextUnit = secondUnit.Unit;

            // First Turn 
            yield return RunMove(firstUnit, secondUnit, firstUnit.Unit.CurrentMove);
            yield return RunAfterTurn(firstUnit);
            if (state == BattleState.BattleOver) yield break;

            if(nextUnit.HP > 0)
            {
                // Second Turn
                yield return RunMove(secondUnit, firstUnit, secondUnit.Unit.CurrentMove);
                yield return RunAfterTurn(secondUnit);
                if (state == BattleState.BattleOver) yield break;
            }
            
        }

        else
        {
            if(playerAction == BattleAction.SwitchPokemon)
            {
                var selectedPokemon = playerParty.Units[currentMember];
                state = BattleState.Busy;
                yield return SwitchPartyMember(selectedPokemon);
            }

            // Enemy Turn
            var enemyMove = enemyUnit.Unit.GetRandomMove();
            yield return RunMove(enemyUnit, playerUnit, enemyMove);
            yield return RunAfterTurn(enemyUnit);
            if (state == BattleState.BattleOver) yield break;
        }

        if(state != BattleState.BattleOver)
        {
            ActionSelection();
        }
    }

    IEnumerator RunMove(BattleUnit sourceUnit, BattleUnit targetUnit, Move move)
    {
        state = BattleState.RunningTurn;

        bool canRunMove = sourceUnit.Unit.OnBeforeMove();
        if(!canRunMove)
        {
            yield return ShowStatusChanges(sourceUnit.Unit);

            sourceUnit.Unit.OnAfterTurn();
            yield return ShowStatusChanges(sourceUnit.Unit);
            yield return sourceUnit.Hud.UpdateHP();

            if (sourceUnit.Unit.HP <= 0)
            {
                yield return dialogueBox.TypeDialogue($"{sourceUnit.Unit.UnitBase.Name} has been slain");
                yield return new WaitForSeconds(2f);

                CheckForBattleOver(sourceUnit);
            }

            yield break;
        }
        yield return ShowStatusChanges(sourceUnit.Unit);

        sourceUnit.Unit.DecreaseMana(move.ManaCost);

        yield return dialogueBox.TypeDialogue($"{sourceUnit.Unit.UnitBase.Name} used {move.MoveBase.Name}");
        yield return new WaitForSeconds(1f);

        if(CheckIfMoveHits(move, sourceUnit.Unit, targetUnit.Unit))
        {
            if (move.MoveBase.MoveCategory == MoveCategory.Status)
            {
                yield return RunMoveEffects(move.MoveBase.MoveEffects, sourceUnit.Unit, targetUnit.Unit, move.MoveBase.Target);
                yield return sourceUnit.Hud.UpdateMana();
                sourceUnit.Hud.SetStatusIcon();
                targetUnit.Hud.SetStatusIcon();
            }
            else
            {
                bool isDead = targetUnit.Unit.TakeDamage(move, sourceUnit.Unit);
                yield return targetUnit.Hud.UpdateHP();
                yield return sourceUnit.Hud.UpdateMana();
            }

            if(move.MoveBase.SecondaryEffects != null && move.MoveBase.SecondaryEffects.Count > 0 && targetUnit.Unit.HP > 0)
            {
                foreach(var secondeffect in move.MoveBase.SecondaryEffects)
                {
                    var random = UnityEngine.Random.Range(1, 101);
                    if(random <= secondeffect.Chance)
                    {
                        yield return RunMoveEffects(secondeffect, sourceUnit.Unit, targetUnit.Unit, secondeffect.Target);
                    }
                }
            }

            if (targetUnit.Unit.HP <= 0)
            {
                yield return dialogueBox.TypeDialogue($"{targetUnit.Unit.UnitBase.Name} has been slain");
                yield return new WaitForSeconds(2f);

                CheckForBattleOver(targetUnit);
            }
            
        }
        else
        {
            yield return dialogueBox.TypeDialogue($"{sourceUnit.Unit.UnitBase.Name} missed");
        }
        
    }

    IEnumerator RunMoveEffects(MoveEffects effects, Unit source, Unit target, MoveTarget moveTarget)
    {
        // Stat Boosting
        if (effects.Boosts != null)
        {
            if (moveTarget == MoveTarget.Self)
            {
                source.ApplyBoosts(effects.Boosts);
            }
            else
            {
                target.ApplyBoosts(effects.Boosts);
            }
        }

        // Status Condition
        if(effects.Status != ConditionID.none)
        {
            if(target.StatusL.Count < 1)
            {
                target.SetStatus(effects.Status);
            }
            else
            {
                yield return dialogueBox.TypeDialogue($"{target.UnitBase.Name} already has a status condition!");
            }
        }

        yield return ShowStatusChanges(source);
        yield return ShowStatusChanges(target);
    }

    IEnumerator RunAfterTurn(BattleUnit sourceUnit)
    {
        if (state == BattleState.BattleOver) yield break;
        yield return new WaitUntil(() => state == BattleState.RunningTurn);

        // Status may damage sourceUnit and that can lead to the sourceUnit being dead
        sourceUnit.Unit.OnAfterTurn();
        yield return ShowStatusChanges(sourceUnit.Unit);
        yield return sourceUnit.Hud.UpdateHP();

        if (sourceUnit.Unit.HP <= 0)
        {
            yield return dialogueBox.TypeDialogue($"{sourceUnit.Unit.UnitBase.Name} has been slain");
            yield return new WaitForSeconds(2f);

            CheckForBattleOver(sourceUnit);
        }
    }

    bool CheckIfMoveHits(Move move, Unit source, Unit target)
    {
        if(move.MoveBase.AlwaysHits)
        {
            return true;
        }

        float moveAccuracy = move.MoveBase.Accuracy;

        int accuracy = source.StatBoosts[Stat.Accuracy];

        var boostValues = new float[] { 1f, 1.4f, 1.7f, 2f, 2.5f };

        if (accuracy > 0)
        {
            moveAccuracy *= boostValues[accuracy];
        }
        else
        {
            moveAccuracy /= boostValues[-accuracy];
        }

        return UnityEngine.Random.Range(1, 101) <= moveAccuracy;
    }

    IEnumerator ShowStatusChanges(Unit unit)
    {
        while(unit.StatusChanges.Count > 0)
        {
            var message = unit.StatusChanges.Dequeue();
            yield return dialogueBox.TypeDialogue(message);
            yield return new WaitForSeconds(1.5f);
        }
    }

    void CheckForBattleOver(BattleUnit faintedUnit)
    {
        if(faintedUnit.IsPlayerUnit)
        {
            var nextUnit = playerParty.GetNotFaintedUnit();
            if (nextUnit != null)
            {
                OpenPartyScreen();
            }
            else
            {
                BattleOver(false);
            }
        }
        else
        {
            BattleOver(true);
        }
    }

    public void HandleUpdate()
    {
        if(state == BattleState.ActionSelection)
        {
            HandleActionSelection();
        }

        else if(state == BattleState.MoveSelection)
        {
            HandleMoveSelection();
        }
        else if(state == BattleState.PartyScreen)
        {
            HandlePartySelection();
        }
    }

    void HandleActionSelection()
    {
        if(Input.GetKeyDown(KeyCode.D))
        {
            currentAction++;
        }
        else if(Input.GetKeyDown(KeyCode.A))
        {
            currentAction--;
        }
        else if(Input.GetKeyDown(KeyCode.S))
        {
            currentAction += 2;
        }
        else if(Input.GetKeyDown(KeyCode.W))
        {
            currentAction -= 2;
        }

        currentAction = Mathf.Clamp(currentAction, 0, 3);

        dialogueBox.UpdateActionSelection(currentAction);

        if(Input.GetKeyDown(KeyCode.Space))
        {
            if(currentAction == 0)
            {
                // Fight
                MoveSelection();
            }
            else if(currentAction == 1)
            {
                // Bag
            }
            else if (currentAction == 2)
            {
                // Switch/Party
                prevState = state;
                OpenPartyScreen();
            }
            else if (currentAction == 3)
            {
                // Run
            }
        }
    }

    void HandleMoveSelection()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            if (currentMove < playerUnit.Unit.Moves.Count - 1)
            {
                currentMove++;
            }
        }
        else if (Input.GetKeyDown(KeyCode.W))
        {
            if (currentMove > 0)
            {
                currentMove--;
            }
        }

        dialogueBox.UpdateMoveSelection(currentMove, playerUnit.Unit.Moves[currentMove]);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            dialogueBox.EnableMoveSelector(false);
            dialogueBox.EnableDialogueText(true);
            StartCoroutine(RunTurns(BattleAction.Move));
        }
        else if(Input.GetKeyDown(KeyCode.F))
        {
            dialogueBox.EnableMoveSelector(false);
            dialogueBox.EnableDialogueText(true);
            ActionSelection();
        }
    }

    void HandlePartySelection()
    {
        if (Input.GetKeyDown(KeyCode.D))
        {
            currentMember++;
        }
        else if (Input.GetKeyDown(KeyCode.A))
        {
            currentMember--;
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            currentMember += 2;
        }
        else if (Input.GetKeyDown(KeyCode.W))
        {
            currentMember -= 2;
        }

        currentMember = Mathf.Clamp(currentMember, 0, playerParty.Units.Count-1);

        partyScreen.UpdateMemberSelection(currentMember);

        if(Input.GetKeyDown(KeyCode.Space))
        {
            var selectedMember = playerParty.Units[currentMember];
            if(selectedMember.HP <= 0)
            {
                partyScreen.SetMessageText("He dead brah");
                return;
            }
            if(selectedMember == playerUnit.Unit)
            {
                partyScreen.SetMessageText("He already it battle brah");
                return;
            }

            partyScreen.gameObject.SetActive(false);

            if(prevState == BattleState.ActionSelection)
            {
                prevState = null;
                StartCoroutine(RunTurns(BattleAction.SwitchPokemon));
            }
            else
            {
                state = BattleState.Busy;
                StartCoroutine(SwitchPartyMember(selectedMember));
            } 
        }
        else if(Input.GetKeyDown(KeyCode.F))
        {
            partyScreen.gameObject.SetActive(false);
            ActionSelection(); 
        }
    }

    IEnumerator SwitchPartyMember(Unit newUnit)
    {
        if(playerUnit.Unit.HP > 0)
        {
            yield return dialogueBox.TypeDialogue($"{playerUnit.Unit.UnitBase.Name} is switching");
            yield return new WaitForSeconds(1f);
        }

        playerUnit.Setup(newUnit);

        dialogueBox.SetMoveNames(newUnit.Moves);

        yield return dialogueBox.TypeDialogue($"Boom waddup {newUnit.UnitBase.Name} here.");
        yield return new WaitForSeconds(1f);

        state = BattleState.RunningTurn;
    }
}



public enum BattleState
{
    Start, ActionSelection, MoveSelection, RunningTurn, Busy, PartyScreen, BattleOver
}

public enum BattleAction
{
    Move, SwitchPokemon, UseItem, Run
}
