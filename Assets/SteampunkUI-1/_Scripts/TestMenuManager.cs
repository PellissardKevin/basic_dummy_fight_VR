using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SteampunkUI
{
    public class TestMenuManager : MonoBehaviour
    {
        public GameObject toggleButtonsPanel;
        public GameObject squareButtonsPanel;
        public GameObject slidersPanel;
        public GameObject welcomePanel;

        private CanvasGroup toggleButtonsCanvasGroup;
        private CanvasGroup squareButtonsCanvasGroup;
        private CanvasGroup slidersCanvasGroup;
        private CanvasGroup welcomePanelCanvasGroup;

        public List<Sprite> welcomeSprites = new List<Sprite>();

        private void Start()
        {
            toggleButtonsCanvasGroup = toggleButtonsPanel.AddComponent<CanvasGroup>();
            squareButtonsCanvasGroup = squareButtonsPanel.AddComponent<CanvasGroup>();
            slidersCanvasGroup = slidersPanel.AddComponent<CanvasGroup>();
            welcomePanelCanvasGroup = welcomePanel.AddComponent<CanvasGroup>();

            welcomePanel.GetComponent<Image>().sprite = welcomeSprites[Random.Range(0, 2)];


            // Initialize the alpha values
            toggleButtonsCanvasGroup.alpha = 0f;
            squareButtonsCanvasGroup.alpha = 0f;
            slidersCanvasGroup.alpha = 0f;
            welcomePanelCanvasGroup.interactable = false;
            welcomePanelCanvasGroup.blocksRaycasts = false;
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

            yield return new WaitForSeconds(1);//Delay for others to be completely hidden

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
        private void HideAllPanels()
        {
            StartCoroutine(FadeOutCanvasGroup(toggleButtonsCanvasGroup));
            StartCoroutine(FadeOutCanvasGroup(squareButtonsCanvasGroup));
            StartCoroutine(FadeOutCanvasGroup(slidersCanvasGroup));
            StartCoroutine(FadeOutCanvasGroup(welcomePanelCanvasGroup));
        }

        public void OnClickToggleButtons()
        {
            HideAllPanels();
            StartCoroutine(FadeInCanvasGroup(toggleButtonsCanvasGroup));
        }
        public void OnClickSquareButtons()
        {
            HideAllPanels();
            StartCoroutine(FadeInCanvasGroup(squareButtonsCanvasGroup));
        }
        public void OnClickSliders()
        {
            HideAllPanels();
            StartCoroutine(FadeInCanvasGroup(slidersCanvasGroup));
        }
    }
}
