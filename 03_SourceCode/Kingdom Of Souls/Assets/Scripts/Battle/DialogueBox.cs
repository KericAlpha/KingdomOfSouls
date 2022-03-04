using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueBox : MonoBehaviour
{
    [SerializeField] int lettersPerSecond;
    [SerializeField] Color highlightedColor;

    [SerializeField] Text dialogueText;
    [SerializeField] GameObject actionSelector;
    [SerializeField] GameObject moveSelector;
    [SerializeField] GameObject moveDetails;

    [SerializeField] List<Text> actionTexts;
    [SerializeField] List<Text> moveTexts;

    [SerializeField] Text manaText;
    [SerializeField] Text dmgText;
    [SerializeField] Text typeText;

    public void SetDialogue(string dialogue)
    {
        dialogueText.text = dialogue;
    }

    public IEnumerator TypeDialogue(string dialogue)
    {
        dialogueText.text = "";
        foreach(var letter in dialogue.ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(1F / lettersPerSecond);
        }
    }

    public void EnableDialogueText(bool enabled)
    {
        dialogueText.enabled = enabled;
    }

    public void EnableActionSelectorText(bool enabled)
    {
        actionSelector.SetActive(enabled);
    }

    public void EnableMoveSelectorText(bool enabled)
    {
        moveSelector.SetActive(enabled);
        moveDetails.SetActive(enabled);
    }

    public void UpdateActionSelection(int selectedAction)
    {
        for(int i = 0; i < actionTexts.Count; i++)
        {
            if(i == selectedAction)
            {
                actionTexts[i].color = highlightedColor;
            }
            else
            {
                actionTexts[i].color = Color.white;
            }
        }
    }

    public void UpdateMoveSelection(int selectedMove, Move move)
    {
        for (int i = 0; i < moveTexts.Count; i++)
        {
            if (i == selectedMove)
            {
                moveTexts[i].color = highlightedColor;
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
                moveTexts[i].text = "----";
            }
        }
    }

    
}
