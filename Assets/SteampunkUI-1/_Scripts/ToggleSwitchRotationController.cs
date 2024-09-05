using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace SteampunkUI
{
    public class ToggleSwitchRotationController : MonoBehaviour
    {
        public bool defaultState = false;
        public bool IsToggled { get; private set; } = false;

        public float onRotationZ = -240f;  // Z rotation value when toggled on
        public float offRotationZ = 0f;   // Z rotation value when toggled off
        public Transform innerSwitch;
        public TMP_Text toggleText;
        public float rotationSpeed = 90f;  // Adjust the speed as needed

        public UnityEvent <bool> OnToggle;

        private Quaternion targetRotation;
        private bool isRotating = false;

        private void Start()
        {
            IsToggled = defaultState;
            targetRotation = Quaternion.Euler(0f, 0f, IsToggled ? onRotationZ : offRotationZ);
            toggleText.text = IsToggled ? "On" : "Off";

            RotateToZRotation(targetRotation);
        }

        public void Toggle()
        {
            if (isRotating)
                return;

            IsToggled = !IsToggled;
            toggleText.text = IsToggled ? "On" : "Off";

            targetRotation = Quaternion.Euler(0f, 0f, IsToggled ? onRotationZ : offRotationZ);
            OnToggle?.Invoke(IsToggled);
            StartCoroutine(RotateToZRotationSmooth(targetRotation));
        }

        private void RotateToZRotation(Quaternion rotation)
        {
            if (innerSwitch != null)
            {
                innerSwitch.localRotation = rotation;
            }
        }

        private IEnumerator RotateToZRotationSmooth(Quaternion targetRotation)
        {
            isRotating = true;
            Quaternion startRotation = innerSwitch.localRotation;

            float elapsedTime = 0f;
            float duration = Quaternion.Angle(startRotation, targetRotation) / rotationSpeed;

            while (elapsedTime < duration)
            {
                innerSwitch.localRotation = Quaternion.Lerp(startRotation, targetRotation, elapsedTime / duration);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            innerSwitch.localRotation = targetRotation;
            isRotating = false;
        }
    }
}
