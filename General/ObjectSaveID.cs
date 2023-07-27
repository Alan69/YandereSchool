using UnityEngine;
using System.Xml.Linq;


public enum ObjectType {item, door, switchable, puzzle,  locks, triggerEvent}

public class ObjectSaveID : MonoBehaviour {


    [Tooltip("id of object for save manager")]
    public int objectID;
    [Tooltip("Type of object (item,door etc)")]
    public ObjectType objectType = ObjectType.door;

    private SaveGameManager saveManager;

    private void Awake()
    {
        saveManager = GameObject.FindObjectOfType<SaveGameManager>();
        saveManager.objectsToSave.Add(this);
    }


    private void OnDestroy()
    {
        saveManager.objectsToSave.Remove(this);
    }

    public XElement GetElement()
    {
        if (objectType == ObjectType.item)
        {
            XAttribute id = new XAttribute("id", objectID);
            XAttribute x = new XAttribute("x", transform.position.x);
            XAttribute y = new XAttribute("y", transform.position.y);
            XAttribute z = new XAttribute("z", transform.position.z);
            XAttribute rx = new XAttribute("rx", transform.eulerAngles.x);
            XAttribute ry = new XAttribute("ry", transform.eulerAngles.y);
            XAttribute rz = new XAttribute("rz", transform.eulerAngles.z);
            XElement element = new XElement("instance", id, x, y, z, rx, ry, rz);
            return element;
        }

        if (objectType == ObjectType.door)
        {
            XAttribute id = new XAttribute("id", objectID);
            XAttribute locked = new XAttribute("locked", GetComponent<Door>().locked);
            XAttribute state = new XAttribute("state", GetComponent<Door>().state);
            XAttribute locks = new XAttribute("locks", GetComponent<Door>().locksCount);
            XElement element = new XElement("instance", id,locked,state,locks);
            return element;
        }

        if (objectType == ObjectType.switchable)
        {
            XAttribute id = new XAttribute("id", objectID);         
            XElement element = new XElement("instance", id);
            return element;
        }

        if (objectType == ObjectType.puzzle)
        {
            XAttribute id = new XAttribute("id", objectID);        
            XAttribute puzzleID = new XAttribute("puzzleID", GetComponent<PuzzleBlock>().ID);
            XAttribute x = new XAttribute("x", transform.position.x);
            XAttribute y = new XAttribute("y", transform.position.y);
            XAttribute z = new XAttribute("z", transform.position.z);
            XElement element = new XElement("instance", id, x, y, z, puzzleID);
            return element;
        }

        if (objectType == ObjectType.locks)
        {
           
            XAttribute id = new XAttribute("id", objectID);
            XAttribute state = new XAttribute("lockState", GetComponent<Lock>().isOpen);
            XAttribute x = new XAttribute("x", transform.position.x);
            XAttribute y = new XAttribute("y", transform.position.y);
            XAttribute z = new XAttribute("z", transform.position.z);
            XAttribute rx = new XAttribute("rx", transform.eulerAngles.x);
            XAttribute ry = new XAttribute("ry", transform.eulerAngles.y);
            XAttribute rz = new XAttribute("rz", transform.eulerAngles.z);
            XElement element = new XElement("instance", id, x, y, z, rx, ry, rz,state);
            return element;
        }

        if (objectType == ObjectType.triggerEvent)
        {

            XAttribute id = new XAttribute("id", objectID);
            XAttribute state = new XAttribute("state", GetComponent<TriggerEvents>().activated);
            XElement element = new XElement("instance", id, state);
            return element;
        }

        return null;
    }




}
