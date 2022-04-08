using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleSystem : MonoBehaviour
{
    [SerializeField] BattleUnit playerUnit;
    [SerializeField] BattleUnit enemyUnit;
    [SerializeField] BattleHud playerHud;
    [SerializeField] BattleHud enemyHud;
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
        playerHud.SetData(playerUnit.Unit);
        enemyHud.SetData(enemyUnit.Unit);

        partyScreen.Init();

        dialogueBox.SetMoveNames(playerUnit.Unit.Moves);

        yield return dialogueBox.TypeDialogue($"What is {enemyUnit.Unit.UnitBase.Name} doing here?");
        yield return new WaitForSeconds(1f);

        PlayerAction();
    }

    void PlayerAction()
    {
        state = BattleState.PlayerAction;
        StartCoroutine(dialogueBox.TypeDialogue("Choose an action"));
        dialogueBox.EnableActionSelector(true);
    }

    void OpenPartyScreen()
    {
        state = BattleState.PartyScreen;
        partyScreen.SetPartyData(playerParty.Units);
        partyScreen.gameObject.SetActive(true);  
    }

    void PlayerMove()
    {
        state = BattleState.PlayerMove;
        dialogueBox.EnableActionSelector(false);
        dialogueBox.EnableDialogueText(false);
        dialogueBox.EnableMoveSelector(true);
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

        if (isDead)
        {
            yield return dialogueBox.TypeDialogue($"{enemyUnit.Unit.UnitBase.Name} has been slain");

            yield return new WaitForSeconds(2f);
            OnBattleOver(true);
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
            yield return dialogueBox.TypeDialogue($"{playerUnit.Unit.UnitBase.Name} has been slain");

            yield return new WaitForSeconds(2f);


            var nextUnit = playerParty.GetNotFaintedUnit();
            if(nextUnit != null)
            {
                OpenPartyScreen();
            }
            else
            {
                OnBattleOver(false);
            }
        }
        else
        {
            PlayerAction();
        }
    }

    public void HandleUpdate()
    {
        if(state == BattleState.PlayerAction)
        {
            HandleActionSelection();
        }

        else if(state == BattleState.PlayerMove)
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
                PlayerMove();
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
            StartCoroutine(PerformPlayerMove());
        }
        else if(Input.GetKeyDown(KeyCode.F))
        {
            dialogueBox.EnableMoveSelector(false);
            dialogueBox.EnableDialogueText(true);
            PlayerAction();
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
            PlayerAction(); 
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
        playerHud.SetData(newUnit);

        dialogueBox.SetMoveNames(newUnit.Moves);

        yield return dialogueBox.TypeDialogue($"Boom waddup {newUnit.UnitBase.Name} here.");
        yield return new WaitForSeconds(1f);

        StartCoroutine(EnemyMove());
    }
}



public enum BattleState
{
    Start, PlayerAction, PlayerMove, EnemyMove, Busy, PartyScreen
}
