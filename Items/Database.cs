using UnityEngine;

[System.Serializable]
public class Database {

    [Header("General Settings")]
    [Tooltip("Item name")]
    public string name;
    [Tooltip("Item ID")]
    public int id;
    [Tooltip("Item prefab")]
    public GameObject itemPrefab;
    [Tooltip("Item hand position ID (find needed value in Player/Inventory.cs)")]
    public int handPosID;

}

