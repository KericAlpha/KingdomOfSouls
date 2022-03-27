using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BattleState { Start, PlayerAction, PlayerMove, EnemyMove, Busy}

public class BattleSystem : MonoBehaviour
{
    [SerializeField] BattleUnit playerUnit;
    [SerializeField] BattleUnit enemyUnit;
    [SerializeField] BattleHud playerHud;
    [SerializeField] BattleHud enemyHud;
    [SerializeField] DialogueBox dialogueBox;

    BattleState state;
    int currentAction;
    int currentMove;

    private void Start()
    {
        StartCoroutine(SetupBattle());
    }

    public IEnumerator SetupBattle()
    {
        playerUnit.Setup();
        enemyUnit.Setup();
        playerHud.SetData(playerUnit.Unit);
        enemyHud.SetData(enemyUnit.Unit);

        dialogueBox.SetMoveNames(playerUnit.Unit.Moves);

        yield return dialogueBox.TypeDialogue(enemyUnit.Unit.UnitBase.Name + ". Who is this random? SLANDER HIM!");
        yield return new WaitForSeconds(1f);

        ChooseFirstTurn();
    }

    public void ChooseFirstTurn()
    {
        if(playerUnit.Unit.Speed >= enemyUnit.Unit.Speed)
        {
            PlayerAction();
        }
        else
        {
            StartCoroutine(EnemyMove());
        }
    }

    public void PlayerAction()
    {
        state = BattleState.PlayerAction;
        StartCoroutine(dialogueBox.TypeDialogue("Choose an action"));
        dialogueBox.EnableActionSelectorText(true);
    }

    public void PlayerMove()
    {
        state = BattleState.PlayerMove;
        dialogueBox.EnableActionSelectorText(false);
        dialogueBox.EnableDialogueText(false);
        dialogueBox.EnableMoveSelectorText(true);
    }

    IEnumerator PerformPlayerMove()
    {
        state = BattleState.Busy;

        var move = playerUnit.Unit.Moves[currentMove];
        yield return dialogueBox.TypeDialogue($"{playerUnit.Unit.UnitBase.Name} used {move.MoveBase.Name}");

        yield return new WaitForSeconds(1f);

        if(move.MoveBase.MoveType == MoveType.Status)
        {
            yield return RunMoveEffects(move, playerUnit.Unit, enemyUnit.Unit);
        }
        
        else
        {
            bool isDead = enemyUnit.Unit.TakeDamage(move, playerUnit.Unit);
            yield return enemyHud.UpdateHP();
            yield return playerHud.UpdateMana();
        }


        if (enemyUnit.Unit.HP <= 0)
        {
            yield return dialogueBox.TypeDialogue($"{enemyUnit.Unit.UnitBase.Name} is dead");
            playerUnit.Unit.OnBattleOver();
        }

        playerUnit.Unit.OnAfterTurn();
        yield return ShowStatusChanges(playerUnit.Unit);
        yield return playerHud.UpdateHP();
        
        // watch video from beginning i would say -> try to implement party asap

        if (playerUnit.Unit.HP <= 0)
        {
            yield return dialogueBox.TypeDialogue($"{playerUnit.Unit.UnitBase.Name} is dead");
            playerUnit.Unit.OnBattleOver();
        }
        else
        {
            StartCoroutine(EnemyMove());
        }
    }

    IEnumerator EnemyMove()
    {
        state = BattleState.EnemyMove;

        var move = enemyUnit.Unit.GetRandomMove();

        yield return dialogueBox.TypeDialogue($"{enemyUnit.Unit.UnitBase.Name} used {move.MoveBase.Name}");

        yield return new WaitForSeconds(1f);

        bool isDead = playerUnit.Unit.TakeDamage(move, enemyUnit.Unit);
        yield return playerHud.UpdateHP();
        yield return enemyHud.UpdateMana();

        if (isDead)
        {
            yield return dialogueBox.TypeDialogue($"{playerUnit.Unit.UnitBase.Name} is dead");
            playerUnit.Unit.OnBattleOver();
        }
        else
        {
            PlayerAction();
        }
    }

    IEnumerator RunMoveEffects(Move move, Unit source, Unit target)
    {
        var effects = move.MoveBase.Effects;

        // Stat Boosting
        if (effects.Boosts != null)
        {
            bool moveSuccess = source.LowerMana(move);

            if (moveSuccess)
            {
                if (move.MoveBase.Target == MoveTarget.Self)
                {
                    source.ApplyBoosts(effects.Boosts);
                }
                else
                {
                    target.ApplyBoosts(effects.Boosts);
                }

                yield return playerHud.UpdateMana();
                yield return enemyHud.UpdateMana();
            }  
        }

        // Status 
        if(effects.Status != ConditionID.NONE)
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

            yield return new WaitForSeconds(0.5f);
        }
    }

    public void Update()
    {
        if (state == BattleState.PlayerAction)
        {
            HandleActionSelection();
        }
        else if(state == BattleState.PlayerMove)
        {
            HandleMoveSelection();
        }
    }

    void HandleActionSelection()
    {
        if (Input.GetKeyDown(KeyCode.D))
        {
            if(currentAction < 1)
            {
                currentAction++;
            }
        }
        else if(Input.GetKeyDown(KeyCode.A))
        {
            if(currentAction > 0)
            {
                currentAction--;
            }
        }

        dialogueBox.UpdateActionSelection(currentAction);

        if(Input.GetKeyDown(KeyCode.Z))
        {
            if (currentAction == 0)
            {
                //Fight
                PlayerMove();
            }

            else if (currentAction == 1)
            {
                //Escape
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
                currentMove --;
            }
        }

        dialogueBox.UpdateMoveSelection(currentMove, playerUnit.Unit.Moves[currentMove]);

        if(Input.GetKeyDown(KeyCode.Z) && playerUnit.Unit.HasEnoughMana(playerUnit.Unit.Moves[currentMove]))
        {
            dialogueBox.EnableMoveSelectorText(false);
            dialogueBox.EnableDialogueText(true);
            StartCoroutine(PerformPlayerMove());
        }
    } 
}
