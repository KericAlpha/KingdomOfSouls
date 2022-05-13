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

        ChooseFirstTurn();
    }

    void ChooseFirstTurn()
    {
        if(playerUnit.Unit.Speed >= enemyUnit.Unit.Speed)
        {
            ActionSelection();
        }
        else
        {
            StartCoroutine(EnemyMove());
        }
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

    IEnumerator PlayerMove()
    {
        state = BattleState.PerformMove;
        var move = playerUnit.Unit.Moves[currentMove];

        yield return RunMove(playerUnit, enemyUnit, move);

        if(state == BattleState.PerformMove)
        {
            StartCoroutine(EnemyMove());
        }
    }

    IEnumerator EnemyMove()
    {
        state = BattleState.PerformMove;

        var move = enemyUnit.Unit.GetRandomMove();

        yield return RunMove(enemyUnit, playerUnit, move);

        if (state == BattleState.PerformMove)
        {
            ActionSelection();
        }
    }

    IEnumerator RunMove(BattleUnit sourceUnit, BattleUnit targetUnit, Move move)
    {
        state = BattleState.PerformMove;

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

        if(move.MoveBase.MoveCategory == MoveCategory.Status)
        {
            yield return RunMoveEffects(move, sourceUnit.Unit, targetUnit.Unit);
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

        if (targetUnit.Unit.HP <= 0)
        {
            yield return dialogueBox.TypeDialogue($"{targetUnit.Unit.UnitBase.Name} has been slain");
            yield return new WaitForSeconds(2f);

            CheckForBattleOver(targetUnit);
        }

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

    IEnumerator RunMoveEffects(Move move, Unit source, Unit target)
    {
        var effects = move.MoveBase.MoveEffects;
        // Stat Boosting
        if (move.MoveBase.MoveEffects.Boosts != null)
        {
            if (move.MoveBase.Target == MoveTarget.Self)
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
            target.SetStatus(effects.Status);
        }

        yield return ShowStatusChanges(source);
        yield return ShowStatusChanges(target);
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
            StartCoroutine(PlayerMove());
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
            state = BattleState.Busy;
            StartCoroutine(SwitchPartyMember(selectedMember));
        }
        else if(Input.GetKeyDown(KeyCode.F))
        {
            partyScreen.gameObject.SetActive(false);
            ActionSelection(); 
        }
    }

    IEnumerator SwitchPartyMember(Unit newUnit)
    {
        bool currentUnitDead = true;
        if(playerUnit.Unit.HP > 0)
        {
            currentUnitDead = false;
            yield return dialogueBox.TypeDialogue($"{playerUnit.Unit.UnitBase.Name} is switching");
            yield return new WaitForSeconds(1f);
        }

        playerUnit.Setup(newUnit);

        dialogueBox.SetMoveNames(newUnit.Moves);

        yield return dialogueBox.TypeDialogue($"Boom waddup {newUnit.UnitBase.Name} here.");
        yield return new WaitForSeconds(1f);

        if(currentUnitDead)
        {
            ChooseFirstTurn();
        }
        else
        {
            StartCoroutine(EnemyMove());
        }
    }
}



public enum BattleState
{
    Start, ActionSelection, MoveSelection, PerformMove, Busy, PartyScreen, BattleOver
}
