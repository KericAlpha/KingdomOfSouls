using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BattleDialogueBox : MonoBehaviour
{
    [SerializeField] int lettersPerSecond;
    [SerializeField] TMP_Text dialogueText;
    [SerializeField] GameObject actionSelector;
    [SerializeField] GameObject moveSelector;
    [SerializeField] GameObject moveDetails;

    [SerializeField] List<TMP_Text> actionTexts;
    [SerializeField] List<TMP_Text> moveTexts;

    [SerializeField] TMP_Text manaText;
    [SerializeField] TMP_Text dmgText;
    [SerializeField] TMP_Text typeText;



    public void SetDialogue(string dText)
    {
        dialogueText.text = dText;
    }


    public IEnumerator TypeDialogue(string dText)
    {
        dialogueText.text = "";
        foreach(var letter in dText.ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(1f / lettersPerSecond);
        }
    }

    public void EnableDialogueText(bool enabled)
    {
        dialogueText.enabled = enabled;
    }

    public void EnableActionSelector(bool enabled)
    {
        actionSelector.SetActive(enabled);
    }

    public void EnableMoveSelector(bool enabled)
    {
        moveSelector.SetActive(enabled);
        moveDetails.SetActive(enabled);
    }

    public void UpdateActionSelection(int selectedAction)
    {
        for(int i = 0; i < actionTexts.Count; i++)
        {
            if (i == selectedAction)
            {
                actionTexts[i].color = Color.red;
            }
            else
                actionTexts[i].color = Color.white;
        }
    }

    public void UpdateMoveSelection(int selectedMove, Move move)
    {
        for (int i = 0; i < moveTexts.Count; i++)
        {
            if (i == selectedMove)
            {
                moveTexts[i].color = Color.red;
            }
            else
            {
                moveTexts[i].color = Color.white;
            }
        }

        manaText.text = $"Mana: {move.ManaCost}";
        dmgText.text = $"Dmg: {move.MoveBase.Power}";
        typeText.text = move.MoveBase.MoveType.ToString();
    }

    public void SetMoveNames(List<Move> moves)
    {
        for(int i = 0; i < moveTexts.Count; i++)
        {
            if(i < moves.Count)
            {
                moveTexts[i].text = moves[i].MoveBase.Name;
            }
            else
            {
                moveTexts[i].text = "-----";
            }
        }
    }


}
