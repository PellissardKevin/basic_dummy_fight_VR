using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEditor.SceneManagement;

public class Play : EditorWindow
{

    private string ArenaScene = "Assets/Scenes/Arena.unity";
    private string MenuScene = "Assets/Scenes/MenuRoomVR_Kevin.unity";
    private string NetworkingScene = "Assets/Scenes/Networking.unity";

    [MenuItem("Window/-----Playtest from Networking Scene-----")]
    public static void ShowWindow()
    {
        Play window = GetWindow<Play>("Playtest Button"); // Open the custom window
        window.Repaint();
    }

    private void OnGUI()
    {
        GUILayout.Label("Playtest Controls", EditorStyles.boldLabel);

        if (GUILayout.Button("Start Playtest on Scene Networking"))
        {
            StartPlayTest();
        }

        if (GUILayout.Button("Switch to Arena"))
        {
            SwitchScene(ArenaScene);
        }

        if (GUILayout.Button("Switch to Menu"))
        {
            SwitchScene(MenuScene);
        }

        if (GUILayout.Button("Switch to Networking"))
        {
            SwitchScene(NetworkingScene);
        }

    }


    private void SwitchScene(string scenePath)
    {
        if (EditorApplication.isPlaying)
        {
            // If play mode is active, prompt to stop it
            if (EditorUtility.DisplayDialog("Stop Play Mode", "Play mode is currently active. Do you want to stop it before switching scenes?", "Yes", "No"))
            {
                // Stop play mode
                EditorApplication.isPlaying = false;

                // Delay the scene switch and play mode start until play mode has been stopped
                EditorApplication.delayCall += () => LoadScene(scenePath);
            }
        }
        else
        {
            // If play mode is not active, directly load the scene
            LoadScene(scenePath);
        }
    }

    private void LoadScene(string scenePath)
    {
        // Open the specified scene
        if (!EditorSceneManager.GetActiveScene().isDirty)
        {
            EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);
        }
    }

    private void StartPlayTest()
    {
        // Specify the scene you want to playtest
        string scenePath = "Assets/Scenes/Networking.unity";
        // Load the scene
        if (!EditorSceneManager.GetActiveScene().isDirty)
        {
            EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);
        }
        // Start play mode
        EditorApplication.delayCall += () =>
        {
            // Start play mode after a delay
            EditorApplication.isPlaying = true;
        };
    }
}
