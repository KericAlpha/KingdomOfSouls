using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Selection : MonoBehaviour
{
    [SerializeField] GameObject actionSelector;
    [SerializeField] GameObject moveSelector;
    [SerializeField] GameObject moveDetails;

    [SerializeField] List<TMP_Text> actionTexts;
    [SerializeField] List<TMP_Text> moveTexts;

    [SerializeField] TMP_Text manaText;
    [SerializeField] TMP_Text dmgText;
    [SerializeField] TMP_Text typeText;
    [SerializeField] TMP_Text moveDetailsText;

    List<Move> bmoves;
    // details text


    public void EnableActionSelector(bool enabled)
    {
        this.gameObject.SetActive(enabled);
        actionSelector.SetActive(enabled);
    }

    public void EnableMoveSelector(bool enabled)
    {
        moveSelector.SetActive(enabled);
        moveDetails.SetActive(enabled);
    }

    public void UpdateActionSelection(int selectedAction)
    {
        for (int i = 0; i < actionTexts.Count; i++)
        {
            if (i == selectedAction)
            {
                actionTexts[i].color = Color.cyan;
            }
            else
                actionTexts[i].color = Color.white;
        }
    }

    public void UpdateMoveSelection(int selectedMove, Move move)
    {
        int sm = Mathf.Clamp(selectedMove - 3, 0, 10);
        SetMoveNames(bmoves, sm);

        int cnt = 0;

        for (int i = sm; i < sm+4; i++)
        {
            if (i == selectedMove)
            {
                moveTexts[cnt].color = Color.cyan;
            }
            else
            {
                moveTexts[cnt].color = Color.white;
            }
            cnt++;
        }

        manaText.text = $"Mana: {move.ManaCost}";
        dmgText.text = $"Dmg: {move.MoveBase.Power}";
        typeText.text = move.MoveBase.MoveType.ToString();
        moveDetailsText.text = move.MoveBase.Description;

    }

    public void SetMoveNames(List<Move> moves, int startingMove)
    {
        bmoves = moves;

        for (int i = 0; i < moveTexts.Count; i++)
        {
            if (i < moves.Count)
            {
                moveTexts[i].text = bmoves[startingMove].MoveBase.Name;
            }
            else
            {
                moveTexts[i].text = "-----";
            }
            startingMove++;
        }
    }
}
