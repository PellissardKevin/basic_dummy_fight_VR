using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace SteampunkUI
{
    public class ToggleSwitchPosiionController : MonoBehaviour
    {
        public bool defaultState = false;
        public bool IsToggled { get; private set; } = true;

        public Vector3 onPosition;
        public Vector3 offPosition;
        public GameObject innerSwitch;
        public TMP_Text toggleText;
        public float moveSpeed = 5f;  // Adjust the speed as needed

        public UnityEvent<bool> OnToggle;

        private Vector3 targetPosition;
        private bool isMoving = false;

        private void Start()
        {
            IsToggled = defaultState;
            targetPosition = IsToggled ? onPosition : offPosition;
            toggleText.text = IsToggled ? "On" : "Off";
            MoveToPosition(targetPosition);
        }

        public void Toggle()
        {
            if (isMoving)
                return;

            IsToggled = !IsToggled;
            toggleText.text = IsToggled ? "On" : "Off";
            targetPosition = IsToggled ? onPosition : offPosition;
            OnToggle?.Invoke(IsToggled);
            StartCoroutine(MoveToPositionSmooth(targetPosition));
        }

        private void MoveToPosition(Vector3 position)
        {
            if (innerSwitch != null)
            {
                innerSwitch.transform.localPosition = position;
            }
        }

        private IEnumerator MoveToPositionSmooth(Vector3 target)
        {
            isMoving = true;
            float startTime = Time.time;
            Vector3 startPosition = innerSwitch.transform.localPosition;

            while (Time.time - startTime < 1 / moveSpeed)
            {
                float t = (Time.time - startTime) * moveSpeed;
                innerSwitch.transform.localPosition = Vector3.Lerp(startPosition, target, t);
                yield return null;
            }

            innerSwitch.transform.localPosition = target;
            isMoving = false;
        }
    }
}
