using UnityEngine;
using UnityEngine.EventSystems;

public class CardDragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
	private Canvas canvas;
	private RectTransform rectTransform;
	private CanvasGroup canvasGroup;

	private Vector3 originalPosition;

	private void Awake()
	{
		canvas = GetComponentInParent<Canvas>();
		rectTransform = GetComponent<RectTransform>();
		canvasGroup = GetComponent<CanvasGroup>();
	}

	public void OnBeginDrag(PointerEventData eventData)
	{
		originalPosition = rectTransform.position;
		canvasGroup.blocksRaycasts = false; // Empêche les raycasts sur la carte pendant le drag
	}

	public void OnDrag(PointerEventData eventData)
	{
		rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor; // Suivre la position du curseur
	}

	public void OnEndDrag(PointerEventData eventData)
	{
		if (eventData.pointerEnter != null && eventData.pointerEnter.CompareTag("Slot"))
		{
			GameObject slot = eventData.pointerEnter;

			// Informer le CardPlacementManager
			FindObjectOfType<CardPlacementManager>().OnCardPlaced(gameObject, slot);

			// Changer le parent de la carte pour le slot dans le Canvas
			rectTransform.SetParent(slot.transform, false); // "false" pour garder la position locale relative au slot

			rectTransform.localPosition = Vector3.zero; // Centrer la carte sur le slot

			// Réinitialiser la rotation si nécessaire (si vous voulez une rotation spécifique, ajustez ici)
			rectTransform.localRotation = Quaternion.identity;

			// Augmenter le scale de la carte
			rectTransform.localScale = new Vector3(3f, 3f, 3f);
		}
		else
		{
			// Retourner à la position initiale si aucun slot valide
			rectTransform.position = originalPosition;
		}

		canvasGroup.blocksRaycasts = true; // Réactiver les raycasts
	}
}
