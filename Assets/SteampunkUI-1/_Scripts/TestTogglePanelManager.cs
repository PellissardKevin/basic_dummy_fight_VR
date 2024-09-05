using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace SteampunkUI
{
    public class TestTogglePanelManager : MonoBehaviour
    {

        public GameObject toggle1;
        public GameObject toggle2;
        public GameObject toggle3;

        public GameObject extra1;
        public GameObject extra2;

        public GameObject title;

        private CanvasGroup toggle1CanvasGroup;
        private CanvasGroup toggle2CanvasGroup;
        private CanvasGroup toggle3CanvasGroup;

        private CanvasGroup extra1CanvasGroup;
        private CanvasGroup extra2CanvasGroup;

        private CanvasGroup titleCanvasGroup;
        private void Start()
        {

            toggle1CanvasGroup = toggle1.AddComponent<CanvasGroup>();
            toggle2CanvasGroup = toggle2.AddComponent<CanvasGroup>();
            toggle3CanvasGroup = toggle3.AddComponent<CanvasGroup>();

            extra1CanvasGroup = extra1.AddComponent<CanvasGroup>();
            extra2CanvasGroup = extra2.AddComponent<CanvasGroup>();

            titleCanvasGroup = title.AddComponent<CanvasGroup>();


            extra1CanvasGroup.alpha = 0;
            extra2CanvasGroup.alpha = 0;

        }
        private IEnumerator FadeOutCanvasGroup(CanvasGroup _group)
        {
            float startAlpha = _group.alpha;
            float targetAlpha = 0f;
            float duration = 1.0f; // 1 second

            float elapsedTime = 0f;

            while (elapsedTime < duration)
            {
                _group.alpha = Mathf.Lerp(startAlpha, targetAlpha, elapsedTime / duration);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            _group.alpha = targetAlpha;
            _group.interactable = false;
            _group.blocksRaycasts = false;
        }
        private IEnumerator FadeInCanvasGroup(CanvasGroup _group)
        {
            float startAlpha = _group.alpha;
            float targetAlpha = 1.0f; // Fully visible
            float duration = 1.0f; // 1 second

            float elapsedTime = 0f;

            //yield return new WaitForSeconds(1);//Delay for others to be completely hidden

            while (elapsedTime < duration)
            {
                _group.alpha = Mathf.Lerp(startAlpha, targetAlpha, elapsedTime / duration);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            _group.alpha = targetAlpha;
            _group.interactable = true;
            _group.blocksRaycasts = true;

        }

        public void OnToggleOne(bool _val)//Hide others
        {
            if (_val)
            {
                StartCoroutine(FadeOutCanvasGroup(toggle2CanvasGroup));
                StartCoroutine(FadeOutCanvasGroup(toggle3CanvasGroup));
            }
            else
            {
                StartCoroutine(FadeInCanvasGroup(toggle2CanvasGroup));
                StartCoroutine(FadeInCanvasGroup(toggle3CanvasGroup));
            }
        }
        public void OnToggleTwo(bool _val)
        {
            if (_val)
            {
                StartCoroutine(FadeInCanvasGroup(extra1CanvasGroup));
                StartCoroutine(FadeInCanvasGroup(extra2CanvasGroup));
            }
            else
            {
                StartCoroutine(FadeOutCanvasGroup(extra1CanvasGroup));
                StartCoroutine(FadeOutCanvasGroup(extra2CanvasGroup));
            }

        }
        public void OnToggleThree(bool _val)
        {

            if (_val)
            {
                StartCoroutine(FadeOutCanvasGroup(titleCanvasGroup));
            }
            else
            {
                StartCoroutine(FadeInCanvasGroup(titleCanvasGroup));
            }

        }
    }
}
