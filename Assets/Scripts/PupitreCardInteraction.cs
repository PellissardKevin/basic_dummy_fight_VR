using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;

public class PupitreCardInteraction : MonoBehaviour
{
    //script refs
    public GameSocketScript gameSocketScript;
    public PupitreScript pupitreScript;
    public CardTypesManagerScript TypeManagerScript;

    public Text debugobj;

    public GameObject Board; //pupitre board with slots
    public GameObject[] Slots = new GameObject[8]; //0-2 equipement, 3-5 trap, 6 action, 7 trash
    private int slot_number = -1; //-1 no selection

    bool isDragging = false; //dragging vars
    GameObject draggedObject;
    Vector3 originalPosition;
    string drag_phase;

    public bool is_VR = false;
    public ActionBasedController XRControllerScript;
    public LineRenderer lineRenderer;

    void Start()
    {

    }

    void Update()
    {
        if (isDragging)
        {
            UpdateDragging();
        }
        else
        {
            DetectDragging();
        }
    }

    private void UpdateDragging()
    {
        int previous_slot = slot_number;
        if (!DetectAction())
        {
            //Debug.Log("Stop dragging detec action false");
            StopDragging();
            return;
        }

        Vector3 Origin;
        Vector3 Direction;

        if (is_VR)
        {
            Origin = gameObject.transform.position;
            Direction = gameObject.transform.forward;
        }
        else
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Origin = ray.origin;
            Direction = ray.direction;
        }

        if (Physics.Raycast(Origin, Direction, out RaycastHit hit, Mathf.Infinity, LayerMask.GetMask("PupitreLayer")))
        {
            if (hit.collider.gameObject == Board.gameObject)
            {
                draggedObject.transform.position = hit.point + new Vector3(0, 0, -0.1f);
                slot_number = -1;
            }

            foreach (GameObject obj in Slots)
            {
                if (hit.collider.gameObject == obj)
                {
                    draggedObject.transform.position = obj.transform.position + new Vector3(0, 0, -0.01f);
                    slot_number = System.Array.IndexOf(Slots, obj);
                    break;
                }
            }
        }

        if (drag_phase != gameSocketScript.current_phase) //if phase changed, reset effects
        {
            set_effects(draggedObject.name.Substring(0, 3));
            return;
        }
    }

    private void DetectDragging()
    {
        if (DetectAction())
        {
            Vector3 Origin;
            Vector3 Direction;

            if (is_VR)
            {
                Origin = gameObject.transform.position;
                Direction = gameObject.transform.forward;
            }
            else
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                Origin = ray.origin;
                Direction = ray.direction;
            }

            if (Physics.Raycast(Origin, Direction, out RaycastHit hit, 10)) // If we hit something
            {
                foreach (GameObject obj in pupitreScript.hand_cards)
                {
                    if (hit.collider.gameObject == obj)
                    {
                        StartDragging(obj);
                        break;
                    }
                }
            }
        }
    }

    private void StartDragging(GameObject obj)
    {
        isDragging = true;
        draggedObject = obj;
        originalPosition = obj.transform.position;
        drag_phase = gameSocketScript.current_phase;
        set_effects(draggedObject.name.Substring(0, 3));
        //Debug.Log($"Start dragging {obj.name}");
    }

    private void StopDragging()
    {
        isDragging = false;
        if (!check_if_played())
            draggedObject.transform.position = originalPosition;
        draggedObject = null;
        set_effects(null);
        //Debug.Log("Stop dragging");
    }

    private bool check_if_played()
    {
        //Debug.Log("Check if played");
        if (slot_number != -1)
            if (can_move_card(draggedObject.name.Substring(0, 3), slot_number))
            {
                MoveCardToBoard(draggedObject.name.Substring(0, 3), slot_number);
                return true;
            }
        return false;
    }

    void Validate_Card(string card_id, int card_position, string phase)
    {
        if (gameSocketScript.SocketScript != null)
           gameSocketScript.SocketScript.Validate_Card(card_id, card_position, phase);
        else
            Debug.Log("SocketScript is null");
    }

    private bool IsSlotAvailable(int slot_index)
    {
        return (pupitreScript.board_cards[slot_index] == null);
    }

    private bool can_move_card(string card_id, int slot_index)
    {
        if(!IsSlotAvailable(slot_index))
            return false;
        string type = TypeManagerScript.GetType1FromID(card_id);
        string phase = gameSocketScript.current_phase;
        //Debug.Log($"can_move_card: {card_id} in slot {slot_index} on phase {phase} type {type}");

        if (phase == "Action" && slot_index == 6 && type == "Action") //card action can only be played in the action slot an
            return true;
        if (phase != "Preparation" || slot_index == 6 ||type == "Action") //card action should work in condition before
            return false;                                                   //only non action cards on phase preparation should be left
        if (slot_index < 3 && type == "Equipement") //equipement cards can only be played in the first 3 slots
            return true;
        if (slot_index > 2 && (type == "Trap" || type == "Piège"))    //trap cards can only be played in the last 3 slots
            return true;

        return false;   //rest is invalid
    }

    private void MoveCardToBoard(string card_id, int slot_index)
    {
        if(can_move_card(card_id, slot_index))
        {
            pupitreScript.MoveCardToBoard(card_id, slot_index);
            Validate_Card(card_id, slot_index, gameSocketScript.current_phase);
        }
        else
            //Debug.Log($"Slot {slot_index} is not available");
        {}
    }

    private void set_effects(string card_id)
    {
        if(card_id == null)
        {
            foreach(GameObject slot in Slots)
                deactivate(slot);
            return;
        }
        string type = TypeManagerScript.GetType1FromID(card_id);
        string phase = gameSocketScript.current_phase;
        //Debug.Log($"Setting effects for card {card_id} type {type} phase {phase}");

        for (int i = 0; i < Slots.Length; i++)
        {
            //Debug.Log($"Checking slot {i}");
            if (i == 7)
            {
                if (phase == "Discard")
                    authorize(Slots[i]);
                else
                    prevent(Slots[i]);
                continue;
            }
            if (can_move_card(card_id, i))
            {
                authorize(Slots[i]);
                //Debug.Log($"Slot {i} is available");
            }
            else
            {
                prevent(Slots[i]);
                //Debug.Log($"Slot {i} is not available");
            }
        }
    }

    private void authorize(GameObject obj)
    {
        obj.transform.GetChild(0).gameObject.SetActive(false);
        obj.transform.GetChild(1).gameObject.SetActive(true);
    }

    private void prevent(GameObject obj)
    {
        obj.transform.GetChild(0).gameObject.SetActive(true);
        obj.transform.GetChild(1).gameObject.SetActive(false);
    }
    private void deactivate(GameObject obj)
    {
        obj.transform.GetChild(0).gameObject.SetActive(false);
        obj.transform.GetChild(1).gameObject.SetActive(false);
    }

    private bool DetectAction()
    {
        if (is_VR)
        {
            return XRControllerScript.activateAction.action.IsPressed();
        }
        else
        {
            return Input.GetMouseButton(0);
        }
    }
}
