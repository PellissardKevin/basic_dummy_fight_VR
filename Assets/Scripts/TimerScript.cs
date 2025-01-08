using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimerScript : MonoBehaviour
{
    public Text timerText; // Assign the UI Text element in the Inspector
    public GameSocketScript gameSocketScript;

    private float currentTime;
    public bool timerActive = false;
    private bool actionCalled = false;

    void Start()
    {
    }

    void Update()
    {
        if (timerActive && currentTime > 0)
        {
            currentTime -= Time.deltaTime;
            UpdateTimerText();

            if (currentTime <= 0)
            {
                currentTime = 0;
                UpdateTimerText();
                timerActive = false;

                if (!actionCalled)
                {
                    ValidatePhase();
                }
            }
        }
    }

    void UpdateTimerText()
    {
        int seconds = Mathf.FloorToInt(currentTime % 60);
        int ms = Mathf.FloorToInt(currentTime * 10) % 10;
        timerText.text = $"{seconds}.{ms}";
    }

    public void ValidatePhase()
    {
        // Perform your action here
        Debug.Log("Timer reached zero, action performed!");
        actionCalled = true;
        gameSocketScript.end_Phase();
    }

    public void ResetTimer(float time)
    {
        currentTime = time;
        timerActive = true;
        actionCalled = false;
        UpdateTimerText();
    }

}
