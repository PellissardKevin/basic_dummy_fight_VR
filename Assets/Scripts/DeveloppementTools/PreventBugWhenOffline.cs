using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PreventBugWhenOffline : MonoBehaviour
{
    public MonoBehaviour[] scriptsToDeactivate;
    public PupitreScript pupitreScript;
    public FieldManager FieldManagerScript;
    public FieldManager FieldManagerScriptOponent;

    void Awake()
    {
        Scene currentScene = SceneManager.GetActiveScene();

        if (currentScene.buildIndex != 0)
        {
            Debug.Log("You did not start the game from the networking scene. Deactivating scripts to prevent bugs.");
            foreach (MonoBehaviour script in scriptsToDeactivate)
            {
                if (script != null)
                    script.enabled = false;
            }
            pupitre_debug_draw(pupitreScript);
            DebugDraw();
            DebugDraw2();

        }
    }

    public void DebugDraw()
    {
        FieldManagerScript.Spawn_Deck(20);
        FieldManagerScript.MoveCardToHand("5");
        FieldManagerScript.MoveCardToHand("9");
        FieldManagerScript.MoveCardToHand("12");
        FieldManagerScript.MoveCardToHand("3");
        FieldManagerScript.MoveCardToHand("21");

        FieldManagerScript.PlaceCardOnBoard("005", 0);

    }
    public void pupitre_debug_draw(PupitreScript pupitreScript)
    {
        pupitreScript.Spawn_Deck(20);
        pupitreScript.MoveCardToHand("5");
        pupitreScript.MoveCardToHand("9");
        pupitreScript.MoveCardToHand("12");
        pupitreScript.MoveCardToHand("3");
        pupitreScript.MoveCardToHand("21");
        pupitreScript.MoveCardToHand("1");
        pupitreScript.MoveCardToHand("16");
        pupitreScript.MoveCardToHand("19");
    }
    public void DebugDraw2()
    {
        FieldManagerScriptOponent.Spawn_Deck(20);
        FieldManagerScriptOponent.MoveCardToHand("6");
        FieldManagerScriptOponent.MoveCardToHand("8");
        FieldManagerScriptOponent.MoveCardToHand("13");
        FieldManagerScriptOponent.MoveCardToHand("2");
        FieldManagerScriptOponent.MoveCardToHand("22");
    }
}
