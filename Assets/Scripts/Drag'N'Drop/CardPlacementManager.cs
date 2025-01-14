using UnityEngine;

public class CardPlacementManager : MonoBehaviour
{
	[SerializeField] private Canvas worldSpaceCanvas; // Référence au Canvas World Space
	[SerializeField] private GameObject card3DPrefab; // Préfabriqué pour l'objet 3D

	public void OnCardPlaced(GameObject card, GameObject screenSpaceSlot)
	{
		// Trouver le slot correspondant dans le World Space
		string slotID = screenSpaceSlot.GetComponent<SlotController>().slotID;
		Transform worldSpaceSlot = FindWorldSpaceSlotByID(slotID);

		if (worldSpaceSlot != null)
		{
			// Instancier un objet 3D au bon emplacement
			GameObject card3D = Instantiate(card3DPrefab, worldSpaceSlot.position, card3DPrefab.transform.rotation);

			// Parent l'objet 3D pour qu'il soit un enfant du slot en World Space
			card3D.transform.SetParent(worldSpaceSlot, true);

			// Appliquer la rotation selon l'ID du slot
			if (slotID == "0" || slotID == "1" || slotID == "2")
			{
				// Placer la carte face ouverte
				card3D.transform.localRotation = Quaternion.Euler(0, 0, -180); // Réinitialise complètement la rotation locale
				Debug.Log($"Carte 3D placée sur le slot {slotID} face ouverte.");
			}
			else
			{
				// Placer la carte face cachée
				card3D.transform.rotation = worldSpaceSlot.rotation * Quaternion.Euler(0, 180, 180); // Appliquer la rotation en World Space
				Debug.Log($"Carte 3D placée sur le slot {slotID} face cachée.");
			}
		}
		else
		{
			Debug.LogError($"Slot avec l'ID {slotID} introuvable dans le Canvas World Space !");
		}
	}

	private Transform FindWorldSpaceSlotByID(string slotID)
	{
		// Rechercher tous les slots dans le Canvas World Space
		SlotController[] worldSlots = worldSpaceCanvas.GetComponentsInChildren<SlotController>();

		foreach (SlotController slot in worldSlots)
		{
			if (slot.slotID == slotID)
			{
				return slot.transform;
			}
		}

		return null;
	}
}
