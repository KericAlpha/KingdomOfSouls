using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBattleSystem : MonoBehaviour
{
    [SerializeField] NewBattleUnit partyUnit1;
    [SerializeField] BattleUnit partyUnit2;
    [SerializeField] BattleUnit partyUnit3;
    [SerializeField] BattleUnit partyUnit4;
    [SerializeField] NewBattleUnit enemyUnit1;
    [SerializeField] BattleUnit enemyUnit2;
    [SerializeField] BattleUnit enemyUnit3;
    [SerializeField] BattleUnit enemyUnit4;
    [SerializeField] Selection selection;
    [SerializeField] NewPartyScreen partyScreen;

    public event Action<bool> OnBattleOver;

    BattleState state;
    BattleState? prevState;
    int currentAction;
    int currentMove;
    int currentMember;
    Unit switchMember;

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
        Debug.Log("setup battle");
        partyUnit1.Setup(playerParty.GetNotFaintedUnit());
        Debug.Log(partyUnit1.Unit.UnitBase.Name);
        enemyUnit1.Setup(enemy);

        partyScreen.Init();

        selection.SetMoveNames(partyUnit1.Unit.Moves, 0);

        //yield return selection.TypeDialogue($"What is {enemyUnit1.Unit.UnitBase.Name} doing here?");
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
        //StartCoroutine(selection.TypeDialogue("Choose an action"));
        selection.EnableActionSelector(true);
    }

    void OpenPartyScreen()
    {
        state = BattleState.PartyScreen;
        /*Debug.Log("Before");
        foreach(Unit u in playerParty.Units)
        {
            Debug.Log(u.UnitBase.name + "" + u.isInBattle);
        }*/
        partyScreen.SetPartyData(playerParty.Units);
        /*Debug.Log("After");
        foreach (Unit u in playerParty.Units)
        {
            Debug.Log(u.UnitBase.name + "" + u.isInBattle);
        }*/
        partyScreen.gameObject.SetActive(true);
    }

    void MoveSelection()
    {
        state = BattleState.MoveSelection;
        currentAction = 0;
        currentMove = 0;
        //selection.EnableActionSelector(false);
        //selection.EnableDialogueText(false);
        selection.EnableMoveSelector(true);
    }

    IEnumerator RunTurns(BattleAction playerAction)
    {
        state = BattleState.RunningTurn;

        if (playerAction == BattleAction.Move)
        {
            partyUnit1.Unit.CurrentMove = partyUnit1.Unit.Moves[currentMove];
            enemyUnit1.Unit.CurrentMove = enemyUnit1.Unit.GetRandomMove();

            // Chech who is first
            bool playerGoesFirst = partyUnit1.Unit.Speed >= enemyUnit1.Unit.Speed;

            var firstUnit = (playerGoesFirst) ? partyUnit1 : enemyUnit1;
            var secondUnit = (playerGoesFirst) ? enemyUnit1 : partyUnit1;

            var nextUnit = secondUnit.Unit;

            // First Turn 
            yield return RunMove(firstUnit, secondUnit, firstUnit.Unit.CurrentMove);
            yield return RunAfterTurn(firstUnit);
            if (state == BattleState.BattleOver) yield break;

            if (nextUnit.HP > 0)
            {
                // Second Turn
                yield return RunMove(secondUnit, firstUnit, secondUnit.Unit.CurrentMove);
                yield return RunAfterTurn(secondUnit);
                if (state == BattleState.BattleOver) yield break;
            }

        }

        else
        {
            if (playerAction == BattleAction.SwitchPokemon)
            {
                var selectedPokemon = switchMember;
                state = BattleState.Busy;
                yield return SwitchPartyMember(selectedPokemon);
            }

            // Enemy Turn
            var enemyMove = enemyUnit1.Unit.GetRandomMove();
            yield return RunMove(enemyUnit1, partyUnit1, enemyMove);
            yield return RunAfterTurn(enemyUnit1);
            if (state == BattleState.BattleOver) yield break;
        }

        if (state != BattleState.BattleOver)
        {
            ActionSelection();
        }
    }

    IEnumerator RunMove(NewBattleUnit sourceUnit, NewBattleUnit targetUnit, Move move)
    {
        state = BattleState.RunningTurn;

        if(sourceUnit.IsPlayerUnit)
        {
            sourceUnit.Hud.MiniHud();
        }
        
        selection.EnableActionSelector(false);

        bool canRunMove = sourceUnit.Unit.OnBeforeMove();
        if (!canRunMove)
        {
            yield return ShowStatusChanges(sourceUnit.Unit);

            sourceUnit.Unit.OnAfterTurn();
            yield return ShowStatusChanges(sourceUnit.Unit);
            sourceUnit.Hud.UpdateHP();

            if (sourceUnit.Unit.HP <= 0)
            {
                //yield return selection.TypeDialogue($"{sourceUnit.Unit.UnitBase.Name} has been slain");
                yield return new WaitForSeconds(2f);

                CheckForBattleOver(sourceUnit);
            }

            yield break;
        }
        yield return ShowStatusChanges(sourceUnit.Unit);

        sourceUnit.Unit.DecreaseMana(move.ManaCost);

        //yield return selection.TypeDialogue($"{sourceUnit.Unit.UnitBase.Name} used {move.MoveBase.Name}");
        yield return new WaitForSeconds(1f);

        if (CheckIfMoveHits(move, sourceUnit.Unit, targetUnit.Unit))
        {
            if (move.MoveBase.MoveCategory == MoveCategory.Status)
            {
                yield return RunMoveEffects(move.MoveBase.MoveEffects, sourceUnit.Unit, targetUnit.Unit, move.MoveBase.Target);
                sourceUnit.Hud.UpdateMana();
                //sourceUnit.Hud.SetStatusIcon();
                //targetUnit.Hud.SetStatusIcon();
            }
            else
            {
                bool isDead = targetUnit.Unit.TakeDamage(move, sourceUnit.Unit);
                targetUnit.Hud.UpdateHP();
                sourceUnit.Hud.UpdateMana();
            }

            if (move.MoveBase.SecondaryEffects != null && move.MoveBase.SecondaryEffects.Count > 0 && targetUnit.Unit.HP > 0)
            {
                foreach (var secondeffect in move.MoveBase.SecondaryEffects)
                {
                    var random = UnityEngine.Random.Range(1, 101);
                    if (random <= secondeffect.Chance)
                    {
                        yield return RunMoveEffects(secondeffect, sourceUnit.Unit, targetUnit.Unit, secondeffect.Target);
                    }
                }
            }

            if (targetUnit.Unit.HP <= 0)
            {
                //yield return selection.TypeDialogue($"{targetUnit.Unit.UnitBase.Name} has been slain");
                yield return new WaitForSeconds(2f);

                CheckForBattleOver(targetUnit);
            }

        }
        else
        {
            //yield return selection.TypeDialogue($"{sourceUnit.Unit.UnitBase.Name} missed");
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
        if (effects.Status != ConditionID.none)
        {
            if (target.StatusL.Count < 1)
            {
                target.SetStatus(effects.Status);
            }
            else
            {
                //yield return selection.TypeDialogue($"{target.UnitBase.Name} already has a status condition!");
            }
        }

        yield return ShowStatusChanges(source);
        yield return ShowStatusChanges(target);
    }

    IEnumerator RunAfterTurn(NewBattleUnit sourceUnit)
    {
        if (state == BattleState.BattleOver) yield break;
        yield return new WaitUntil(() => state == BattleState.RunningTurn);

        // Status may damage sourceUnit and that can lead to the sourceUnit being dead
        sourceUnit.Unit.OnAfterTurn();
        yield return ShowStatusChanges(sourceUnit.Unit);
        sourceUnit.Hud.UpdateHP();

        if (sourceUnit.Unit.HP <= 0)
        {
            //yield return selection.TypeDialogue($"{sourceUnit.Unit.UnitBase.Name} has been slain");
            yield return new WaitForSeconds(2f);

            CheckForBattleOver(sourceUnit);
        }
    }

    bool CheckIfMoveHits(Move move, Unit source, Unit target)
    {
        if (move.MoveBase.AlwaysHits)
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
        while (unit.StatusChanges.Count > 0)
        {
            var message = unit.StatusChanges.Dequeue();
            //yield return selection.TypeDialogue(message);
            yield return new WaitForSeconds(1.5f);
        }
    }

    void CheckForBattleOver(NewBattleUnit faintedUnit)
    {
        if (faintedUnit.IsPlayerUnit)
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
        if (state == BattleState.ActionSelection)
        {
            HandleActionSelection();
        }
        else if (state == BattleState.MoveSelection)
        {
            HandleMoveSelection();
        }
        else if (state == BattleState.PartyScreen)
        {
            //Debug.Log("Party screen");
            HandlePartySelection();
        }
    }

    void HandleActionSelection()
    {
        partyUnit1.Hud.ShowHud();
        if (Input.GetKeyDown(KeyCode.D))
        {
            currentAction++;
        }
        else if (Input.GetKeyDown(KeyCode.A))
        {
            currentAction--;
        }

        //Debug.Log(currentAction);

        currentAction = Mathf.Clamp(currentAction, 0, 3);

        selection.UpdateActionSelection(currentAction);

        if(Input.GetKeyDown(KeyCode.F) || Input.GetKeyDown(KeyCode.Space))
        {
            if (currentAction == 0)
            {
                // Fight
                MoveSelection();
            }
            else if (currentAction == 1)
            {
                // Item/Bag
                //ItemSelection();
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
            if (currentMove < partyUnit1.Unit.Moves.Count - 1)
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

        selection.UpdateMoveSelection(currentMove, partyUnit1.Unit.Moves[currentMove]);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            selection.EnableMoveSelector(false);
            //selection.EnableDialogueText(true);
            StartCoroutine(RunTurns(BattleAction.Move));
        }
        else if (Input.GetKeyDown(KeyCode.F))
        {
            selection.EnableMoveSelector(false);
            //selection.EnableDialogueText(true);
            ActionSelection();
        }
    }

    void HandlePartySelection()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            currentMember += 1;
        }
        else if (Input.GetKeyDown(KeyCode.W))
        {
            currentMember -= 1;
        }

        currentMember = Mathf.Clamp(currentMember, 0, playerParty.Units.Count - 5);

        //Debug.Log(currentMember);

        partyScreen.UpdateMemberSelection(currentMember);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            //Debug.Log(currentMember);
            //Debug.Log("Before: " + partyScreen.getIndexOfUnit(currentMember));
            
            switchMember = playerParty.Units[partyScreen.getIndexOfUnit(currentMember)];
            partyUnit1.Unit.isInBattle = false;
            switchMember.isInBattle = true;

            //Debug.Log("After: " + partyScreen.getIndexOfUnit(currentMember));

            //Debug.Log(selectedMember.UnitBase.name);

            if (switchMember.HP <= 0)
            {
                return;
            }
            if (switchMember == partyUnit1.Unit)
            {
                Debug.Log("same unit");
                return;
            }

            partyScreen.gameObject.SetActive(false);

            if (prevState == BattleState.ActionSelection)
            {
                prevState = null;
                StartCoroutine(RunTurns(BattleAction.SwitchPokemon));
            }
            else
            {
                Debug.Log("switch");
                state = BattleState.Busy;
                StartCoroutine(SwitchPartyMember(switchMember));
            }
        }
        else if (Input.GetKeyDown(KeyCode.F))
        {
            partyScreen.gameObject.SetActive(false);
            ActionSelection();
        }
    }

    IEnumerator SwitchPartyMember(Unit newUnit)
    {
        if (partyUnit1.Unit.HP > 0)
        {
            //yield return selection.TypeDialogue($"{partyUnit1.Unit.UnitBase.Name} is switching");
            yield return new WaitForSeconds(1f);
        }

        partyUnit1.Setup(newUnit);

        selection.SetMoveNames(newUnit.Moves, 0);

        //yield return selection.TypeDialogue($"Boom waddup {newUnit.UnitBase.Name} here."); 
        yield return new WaitForSeconds(1f);

        state = BattleState.RunningTurn;
    }

    public void ItemSelection()
    {
        Debug.Log("Item selection");
    }

    public enum BattleState
    {
        Start, ActionSelection, MoveSelection, RunningTurn, Busy, PartyScreen, BattleOver
    }

    public enum BattleAction
    {
        Move, SwitchPokemon, UseItem, Run
    }
}
