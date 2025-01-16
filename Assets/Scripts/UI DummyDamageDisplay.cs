using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIDummyDamageDisplay : MonoBehaviour
{
    public Transform player;
    public bool lockYAxis = true;

    public Canvas canvasPlayer;
    public Canvas canvasOpponent;

     public Text textPrefab;

    void Update()
    {
        LookAtPlayer(canvasPlayer);
        LookAtPlayer(canvasOpponent);
    }


    void LookAtPlayer(Canvas canvas)
    {
        Vector3 direction = player.position - canvas.transform.position;

        // Optionally lock the Y-axis to keep the canvas upright
        if (lockYAxis)
            direction.y = 0;

        Quaternion lookRotation = Quaternion.LookRotation(direction);
        canvas.transform.rotation = Quaternion.Slerp(canvas.transform.rotation, lookRotation, Time.deltaTime * 5f);
    }

    public void CreateFloatingText(string stat, int value, bool isPlayer)
    {
        if (textPrefab == null)
        {
            Debug.LogWarning("Text prefab is not assigned.");
            return;
        }

        Canvas targetCanvas;
        if (isPlayer)
            targetCanvas = canvasPlayer;
        else
            targetCanvas = canvasOpponent;

        // Instantiate the text prefab
        Text floatingText = Instantiate(textPrefab, targetCanvas.transform);
        floatingText.text = value.ToString();
        floatingText.gameObject.SetActive(true);

        if (value > 0)
            floatingText.color = Color.green;
        else
            floatingText.color = Color.red;

        // Start the animation to move up and fade out
        StartCoroutine(FloatingTextAnimation(floatingText));
    }

    private System.Collections.IEnumerator FloatingTextAnimation(Text floatingText)
    {
        float duration = 3f; // Duration of the animation
        float elapsedTime = 0f;

        Color originalColor = floatingText.color;
        Vector3 originalPosition = floatingText.transform.position;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;

            // Move the text upward
            floatingText.transform.position = originalPosition + Vector3.up * (elapsedTime / duration);

            // Fade out the text
            float alpha = Mathf.Lerp(1f, 0f, elapsedTime / duration);
            floatingText.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);

            yield return null;
        }

        // Destroy the text object after the animation
        Destroy(floatingText.gameObject);
    }

    [ContextMenu("Test Floating Text")]
    void TestFloatingText()
    {
        Debug.Log("Test Floating Text");
        CreateFloatingText("Damage", -10, true);
        CreateFloatingText("Heal", 5, false);
    }
}
