using TMPro;
using UnityEngine;
using UnityEngine.UI;
namespace SteampunkUI
{
    public class SliderAdditionalController : MonoBehaviour
    {
        public Slider slider;
        public TMP_Text SliderValue;
        public GameObject needle;

        //Values for Needle Roation
        public float minRotation = 0f;
        public float maxRotation = -240f;


        private void Start()
        {
            SliderValue.text = slider.value.ToString("00");
        }
        public void OnSliderValueChange(float _val)
        {
            SliderValue.text = _val.ToString("00");

            float normalizedValue = slider.normalizedValue;

            float rotation = Mathf.Lerp(minRotation, maxRotation, normalizedValue);

            // Apply the rotation to the needles object.
            needle.transform.rotation = Quaternion.Euler(0f, 0f, rotation);
        }

    }
}
