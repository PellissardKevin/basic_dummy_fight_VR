using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEditor.SceneManagement;

public class Play : EditorWindow
{
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
