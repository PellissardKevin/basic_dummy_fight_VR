using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PreventBugWhenOffline : MonoBehaviour
{
    public MonoBehaviour[] scriptsToDeactivate;
    public PupitreScript pupitreScript;

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
            pupitreScript.DebugDraw();
        }
    }
}
