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

        PlayerAction();
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

        bool isDead = enemyUnit.Unit.TakeDamage(move, playerUnit.Unit);
        yield return enemyHud.UpdateHP();
        yield return playerHud.UpdateMana();

        if(isDead)
        {
            yield return dialogueBox.TypeDialogue($"{enemyUnit.Unit.UnitBase.Name} is dead");
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
        }
        else
        {
            PlayerAction();
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

        if(Input.GetKeyDown(KeyCode.Z))
        {
            dialogueBox.EnableMoveSelectorText(false);
            dialogueBox.EnableDialogueText(true);
            StartCoroutine(PerformPlayerMove());
        }
    } 
}
