using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using UnityEngine.UI;

public class Interact : MonoBehaviour {


    [Header("Interact Settings")]
    [Tooltip("Distance of ray to interact")]
    public float rayDistance;
    [Tooltip("Layers to interact (default as obstacle)")]
    public LayerMask interactLayers;
    [Tooltip("Tags for interact")]
    public string interactTag;
    [Tooltip("Inventory script")]
    public Inventory inventory;
    [Header("UI Settings")]
    [Tooltip("UI interactButton for mobile only")]
    public Image interactButton;
    private PlayerController player;

    private void Awake()
    {
        player = inventory.gameObject.GetComponent<PlayerController>();
    }

    private void Update()
    {
        if (RayCastCheck() != null)
        {
            interactButton.enabled = true;
            if (CrossPlatformInputManager.GetButtonDown("Interact"))
            {
                if(RayCastCheck().GetComponent<InteractCallEvent>())
                {
                    RayCastCheck().GetComponent<InteractCallEvent>().InteractCall();
                }
                else
                  if(RayCastCheck().GetComponent<Item>())
                  {
                    AudioSource.PlayClipAtPoint(RayCastCheck().GetComponent<Item>().pickupSound, transform.position);
                    inventory.AddItem(RayCastCheck().GetComponent<Item>().itemID, RayCastCheck());
                   
                  }
                else
                if (RayCastCheck().GetComponent<Lock>())
                {
                    if (inventory.CurrentItemID == RayCastCheck().GetComponent<Lock>().needItem)
                    {
                        if (!RayCastCheck().GetComponent<Lock>().isOpen)
                        {
                            if(RayCastCheck().GetComponent<Lock>().removeAfterOpen)
                            {
                                inventory.RemoveItem();
                            }  
                            
                            if(RayCastCheck().GetComponent<Lock>().playItemAnim)
                            {
                                player.inventory.PlayItemAnim(RayCastCheck().GetComponent<Lock>().itemAnimation);
                            }
                            RayCastCheck().GetComponent<Lock>().UnlockLock();
                        }
                    }

                }
                else
                if (RayCastCheck().GetComponent<DoorSiders>())
                     {
                       if (!RayCastCheck().GetComponent<DoorSiders>().genDoor.locked)
                       {
                        RayCastCheck().GetComponent<DoorSiders>().InteractWithDoor();
                       }else
                       {
                          if(inventory.CurrentItemID == RayCastCheck().GetComponent<DoorSiders>().genDoor.keyID)
                          {
                            inventory.RemoveItem();
                            RayCastCheck().GetComponent<DoorSiders>().genDoor.UnlockDoor();
                          }else
                          {
                            RayCastCheck().GetComponent<DoorSiders>().InteractWithDoor();
                          }
                     
                       }
                     
                     }              
            }
            
        }else
        {
            interactButton.enabled = false;
        }
      
    }

    private GameObject RayCastCheck()
    {
        RaycastHit hit;
        Vector3 fwd = transform.TransformDirection(Vector3.forward);

        if (Physics.Raycast(transform.position, fwd, out hit, rayDistance, interactLayers))
        {
            if (hit.transform.gameObject.tag == interactTag)
            {
                return hit.transform.gameObject;
            }


        }

        return null;
    }

}
