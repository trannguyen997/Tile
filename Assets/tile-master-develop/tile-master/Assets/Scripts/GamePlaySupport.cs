using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEditor;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;

public class GamePlaySupport : MonoBehaviour
{
    [SerializeField] Button buttonUndo;
    [SerializeField] Button buttonSuggest;
    [SerializeField] Button buttonReload;



    void Start()
    {
        buttonUndo.onClick.AddListener(Undo);
        buttonSuggest.onClick.AddListener(Hint);
        buttonReload.onClick.AddListener(Refresh);
    }

    public void Undo()
    {
        // Check Count Undo
        // Todo Code

        if(BoardManager.instance.CountUndo > 0)
        {
            bool temp = BoardManager.instance.UndoRecord();
            if (temp == false)
                return;
            BoardManager.instance.CountUndo--;
            if (BoardManager.instance.CountUndo <= 0)
            {
                float r = (float)int.Parse("72", System.Globalization.NumberStyles.HexNumber) / 255;
                float g = (float)int.Parse("72", System.Globalization.NumberStyles.HexNumber) / 255;
                float b = (float)int.Parse("72", System.Globalization.NumberStyles.HexNumber) / 255;

                buttonUndo.image.color = new Color(r, g, b);
                buttonUndo.interactable = false;
            }
        }

        // Save Count Undo
        // Todo Code
    }

    public void Hint()
    {
        // Check Count Undo
        // Todo Code
        if(BoardManager.instance.CountSuggest > 0)
        {
            BoardManager.instance.Hint();
            BoardManager.instance.CountSuggest--;
            if (BoardManager.instance.CountSuggest <= 0)
            {
                float r = (float)int.Parse("72", System.Globalization.NumberStyles.HexNumber) / 255;
                float g = (float)int.Parse("72", System.Globalization.NumberStyles.HexNumber) / 255;
                float b = (float)int.Parse("72", System.Globalization.NumberStyles.HexNumber) / 255;

                buttonSuggest.image.color = new Color(r, g, b);
                buttonSuggest.interactable = false;
            }
        }
        // Save Count Undo
        // Todo Code
    }

    public void Refresh()
    {
        // Check Count Undo
        // Todo Code

        if (BoardManager.instance.CountSuggest > 0)
        {
            BoardManager.instance.Refresh();
            BoardManager.instance.CountSuggest--;
            if (BoardManager.instance.CountSuggest <= 0)
            {
                float r = (float)int.Parse("72", System.Globalization.NumberStyles.HexNumber) / 255;
                float g = (float)int.Parse("72", System.Globalization.NumberStyles.HexNumber) / 255;
                float b = (float)int.Parse("72", System.Globalization.NumberStyles.HexNumber) / 255;

                buttonReload.image.color = new Color(r, g, b);
                buttonReload.interactable = false;
            }
        }

        // Save Count Undo
        // Todo Code
    }
}
