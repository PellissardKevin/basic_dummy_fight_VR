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

    public ActionBasedController XRLeftController;
    public ActionBasedController XRRightController;
    private Transform XRLeftTransform;
    private Transform XRRightTransform;

    public Text debugobj;

    public GameObject Board; //pupitre board with slots
    public GameObject[] Slots = new GameObject[8]; //0-2 equipement, 3-5 trap, 6 action, 7 trash
    private int slot_number = -1; //-1 no selection

    bool isDragging = false; //dragging vars
    GameObject draggedObject;
    Vector3 originalPosition;
    string drag_phase;
    public bool is_VR = false;
    bool is_left = false;

    void Start()
    {
        XRLeftTransform = XRLeftController.gameObject.transform;
        XRRightTransform = XRRightController.gameObject.transform;
    }

    void Update()
    {
        if (isDragging)
        {
            if (is_VR)
                UpdateDragging(null, null);
            else
            {
                UpdateDragging(XRLeftTransform, XRLeftController);
                UpdateDragging(XRRightTransform, XRRightController);
            }
        }
        else
        {
            if (is_VR)
                DetectDragging(null, null);
            else
            {
                DetectDragging(XRLeftTransform, XRLeftController);
                DetectDragging(XRRightTransform, XRRightController);
            }
        }
    }

    private void UpdateDragging(Transform controller, ActionBasedController Controllerscript)
    {
        int previous_slot = slot_number;
        if (!DetectAction())
        {
            StopDragging();
            return;
        }

        Vector3 Origin;
        Vector3 Direction;

        if (is_VR)
        {
            Origin = controller.position;
            Direction = controller.forward;
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

    private void DetectDragging(Transform controller, ActionBasedController Controllerscript)
    {
        if (DetectAction())
        {
            Vector3 Origin;
            Vector3 Direction;

            if (is_VR)
            {
                Origin = controller.position;
                Direction = controller.forward;
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
                        is_left = Controllerscript == XRLeftController;
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
        Debug.Log($"Start dragging {obj.name}");
    }

    private void StopDragging()
    {
        isDragging = false;
        if (!check_if_played())
            draggedObject.transform.position = originalPosition;
        draggedObject = null;
        set_effects(null);
        Debug.Log("Stop dragging");
    }

    private bool check_if_played()
    {
        Debug.Log("Check if played");
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

    public void Test1()
    {
        debugobj.text = "Test1";
        foreach(GameObject card in pupitreScript.hand_cards)
        {
            string card_id = card.name.Substring(0, 3);
            string card_type = TypeManagerScript.GetType1FromID(card_id);
            if(card_type == "Equipement")
            {
                Debug.Log($"Test1: Equipement card found: {card_id}");
                MoveCardToBoard(card_id, 0);
                return;
            }
        }
        Debug.Log("Test1: No Equipement card found");
        debugobj.text = "No Equipement card found";
    }
    public void Test2()
    {
        debugobj.text = "Test2";
        foreach(GameObject card in pupitreScript.hand_cards)
        {
            string card_id = card.name.Substring(0, 3);
            string card_type = TypeManagerScript.GetType1FromID(card_id);
            if(card_type != "Equipement")
            {
                Debug.Log($"Test2: Non Equipement card found: {card_id}");
                MoveCardToBoard(card_id, 3);
                return;
            }
        }
        Debug.Log("Test2: No wrong card found");
        debugobj.text = "No wrong card found";
    }
    public void Test3()
    {
        debugobj.text = "Test3";
        foreach(GameObject card in pupitreScript.hand_cards)
        {
            string card_id = card.name.Substring(0, 3);
            string card_type = TypeManagerScript.GetType1FromID(card_id);
            if(card_type == "Trap")
            {
                Debug.Log($"Test3: Trap card found: {card_id}");
                MoveCardToBoard(card_id, 4);
                return;
            }
        }
        Debug.Log("Test3: No Trap card found");
        debugobj.text = "No Trap card found";
    }
    public void Test4()
    {
        debugobj.text = "Test4";
        foreach(GameObject card in pupitreScript.hand_cards)
        {
            string card_id = card.name.Substring(0, 3);
            string card_type = TypeManagerScript.GetType1FromID(card_id);
            if(card_type != "Trap")
            {
                Debug.Log($"Test4: Non Trap card found: {card_id}");
                MoveCardToBoard(card_id, 4);
                return;
            }
        }
        Debug.Log("Test4: No wrong card found");
        debugobj.text = "No wrong card found";
    }
    public void Test5()
    {
        debugobj.text = "Test5";
        foreach(GameObject card in pupitreScript.hand_cards)
        {
            string card_id = card.name.Substring(0, 3);
            string card_type = TypeManagerScript.GetType1FromID(card_id);
            if(card_type == "Action")
            {
                Debug.Log($"Test5: Action card found: {card_id}");
                MoveCardToBoard(card_id, 6);
                return;
            }
        }
        Debug.Log("Test5: No Action card found");
        debugobj.text = "No Action card found";
    }
    public void Test6()
    {
        debugobj.text = "Test6";
        foreach(GameObject card in pupitreScript.hand_cards)
        {
            string card_id = card.name.Substring(0, 3);
            string card_type = TypeManagerScript.GetType1FromID(card_id);
            if(card_type != "Action")
            {
                Debug.Log($"Test6: Non Action card found: {card_id}");
                MoveCardToBoard(card_id, 2);
                return;
            }
        }
        Debug.Log("Test6: No wrong card found");
        debugobj.text = "No wrong card found";
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
        Debug.Log($"can_move_card: {card_id} in slot {slot_index} on phase {phase} type {type}");

        if (phase == "Action" && slot_index == 6 && type == "Action") //card action can only be played in the action slot an
            return true;
        if (phase != "Preparation" || slot_index == 6 ||type == "Action") //card action should work in condition before
            return false;                                                   //only non action cards on phase preparation should be left
        if (slot_index < 3 && type == "Equipement") //equipement cards can only be played in the first 3 slots
            return true;
        if (slot_index > 2 && type == "Trap")    //trap cards can only be played in the last 3 slots
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
            Debug.Log($"Slot {slot_index} is not available");
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

        for (int i = 0; i < Slots.Length; i++)
        {
            Debug.Log($"Checking slot {i}");
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
                Debug.Log($"Slot {i} is available");
            }
            else
            {
                prevent(Slots[i]);
                Debug.Log($"Slot {i} is not available");
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
            if (is_left && isDragging)
                return XRLeftController.activateAction.action.WasPressedThisFrame();
            if (!is_left && isDragging)
                return XRRightController.activateAction.action.WasPressedThisFrame();
            return XRLeftController.activateAction.action.WasPressedThisFrame() || XRRightController.activateAction.action.WasPressedThisFrame();
        }
        else
            return Input.GetMouseButtonDown(0);
        /*if (XRLeftController != null && XRRightController != null)
            return Input.GetMouseButtonDown(0) || XRLeftController.activateAction.action.WasPressedThisFrame() || XRRightController.activateAction.action.WasPressedThisFrame();
        return Input.GetMouseButtonDown(0) || Input.GetMouseButton(0);*/
    }
}
