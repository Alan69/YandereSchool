using UnityEngine;

public class DoorSiders : MonoBehaviour {

    [Tooltip("Side ID")]
    public int doorSideID;
    [Tooltip("Door gameobject")]
    public Door genDoor;


    public void InteractWithDoor()
    {
        genDoor.InteractDoor(doorSideID);
    }


}
