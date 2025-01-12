using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Position_Sender : MonoBehaviour
{
    public GameSocketScript gameSocketScript;

    private float timer;
    private Vector3 lastSentPosition;
    private Quaternion lastSentRotation;
    private float sendInterval = 0.5f; // Send position every 0.5 seconds
    private float positionThreshold = 0.1f;
    private float rotationThreshold = 0.1f;

    void Start()
    {
        lastSentPosition = transform.position;
        timer = 0f;
    }

    void Update() //check regularily if need to send position
    {
        timer += Time.deltaTime;

        Vector3 currentPosition = transform.position;
        Quaternion currentRotation = transform.rotation;

        if (timer >= sendInterval || Vector3.Distance(currentPosition, lastSentPosition) > positionThreshold || Quaternion.Angle(currentRotation, lastSentRotation) > rotationThreshold)
        {
            string positionString = SerializePosition(currentPosition, transform.rotation);
            gameSocketScript.send_player_position(positionString);
            lastSentPosition = currentPosition;
            lastSentRotation = currentRotation;
        }
        timer = 0f; // Reset the timer
    }

    private string SerializePosition(Vector3 position, Quaternion rotation)
    {
        return $"{position.x}_{position.y}_{position.z}:{rotation.x}_{rotation.y}_{rotation.z}_{rotation.w}";
    }
}
