using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour {

    [Header("Inventory Settings")]
    public Transform[] handsPos;
    [Header("Items database script ")]
    public ItemsDatabase database;
    [HideInInspector]
    public int CurrentItemID;
    private GameObject CurrentItem;
    [Tooltip("Layer ID for item camera culling mask (10 - Item) When we pick up item we make layer 10 (Item) for culling")]
    public int itemCameraLayerID;
    [Tooltip("Layer ID for interact (8 - Interact layer) When we drop item we make layer of this item to 8 (Interact) if we wanna pick up item later")]
    public int itemLayerID;
    private int currentHandPos;

    [Header("UI Settings")]
    public Image DropButton;




    private void Awake()
    {
        DropButton.enabled = false;
        CurrentItemID = -1;
    }

    public void AddItem(int ID, GameObject ItemGO)
    {
        if(CurrentItem)
        {
            DropItem();
        }

        CurrentItemID = ID;
        CurrentItem = ItemGO;
        CurrentItem.GetComponent<Rigidbody>().isKinematic = true;
        int dataBaseID = GetDatabaseID(CurrentItemID);
        currentHandPos = database.Items[dataBaseID].handPosID;
        CurrentItem.transform.parent = handsPos[currentHandPos];
        CurrentItem.transform.localPosition = Vector3.zero;
        CurrentItem.transform.rotation = handsPos[currentHandPos].rotation;
        CurrentItem.layer = itemCameraLayerID;
        DropButton.enabled = true;

    }

    public void PlayItemAnim(string animName)
    {
        handsPos[currentHandPos].GetComponent<Animation>().Play(animName);
    }

    public void DropItem()
    {
        if (CurrentItem != null)
        {
            CurrentItem.layer = itemLayerID;
            CurrentItemID = -1;
            CurrentItem.transform.parent = null;
            CurrentItem.GetComponent<Rigidbody>().isKinematic = false;
            DropButton.enabled = false;
            CurrentItem = null;
        }
    }
	
    public void RemoveItem()
    {
       
            if (CurrentItem != null)
            {
                Destroy(CurrentItem);
                CurrentItemID = -1;
                DropButton.enabled = false;
                CurrentItem = null;
            }
        
    }

    private int GetDatabaseID(int id)
    {
        for (int i = 0; i < database.Items.Count; i++)
        {
            if(database.Items[i].id == id)
            {
                return i;
            }
        }

        return -1;
    }
}
