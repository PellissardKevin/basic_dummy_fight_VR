using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinCondition : MonoBehaviour
{
    public GameObject VictoryText;
    public GameObject DefeatText;
    public GameObject DrawText;
    public GameObject MenuButton;

    public void test_victory(string game_status)
    {
        if (game_status == "you win")
        {
            Debug.Log("Victory");
            VictoryText.SetActive(true);
            MenuButton.SetActive(true);
        }
        else if (game_status == "you lose")
        {
            Debug.Log("Defeat");
            DefeatText.SetActive(true);
            MenuButton.SetActive(true);
        }
        else if (game_status == "Draw")
        {
            Debug.Log("Draw");
            DrawText.SetActive(true);
            MenuButton.SetActive(true);
        }
    }

    [ContextMenu("Test Victory")]
    public void TestVictory()
    {
        test_victory("you win");
    }
    [ContextMenu("Test Defeat")]
    public void TestDefeat()
    {
        test_victory("you lose");
    }
    [ContextMenu("Test Draw")]
    public void TestDraw()
    {
        test_victory("Draw");
    }
    [ContextMenu("Test None")]
    public void TestNone()
    {
        test_victory("None");
    }
}
