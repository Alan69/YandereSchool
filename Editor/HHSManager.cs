using UnityEditor;
using UnityEngine;
using System.IO;
using UnityEditor.SceneManagement;
using UnityEngine.Events;
using UnityEditor.Events;

public class HHSManager : Editor
{
    [SerializeField]
    private static ObjectSaveID[] objectsToSave;
    private static string path;

    [MenuItem("HHS/Rebuild Items Save ID")]
    static void RebuildItems()
    {
        objectsToSave = FindObjectsOfType<ObjectSaveID>();

        for (int i = 0; i < objectsToSave.Length; i++)
        {
            objectsToSave[i].objectID = i;
            EditorUtility.SetDirty(objectsToSave[i]);
        }
        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());

        Debug.Log("<color=green>Rebuild completed!: </color>All saviable objects have individual id");
    }


    [MenuItem("HHS/Delete Game Saves")]
    static void DeleteSaves()
    {
        path = Application.persistentDataPath + "/save.xml";
        if (File.Exists(path))
        {
            File.Delete(path);
            PlayerPrefs.SetInt("LoadGame", 0);
            PlayerPrefs.SetInt("HasSaveGame",0);
            Debug.Log("<color=green>Game Saves deleted!</color>");
        }else
        {
            Debug.Log("<color=red>Game saves not found!</color>");
        }
       
    }


    [MenuItem("HHS/BuildScene")]
    static void BuildScene()
    {
        GameObject psp = PrefabUtility.InstantiatePrefab(Resources.Load("PlayerSpawnPoint")) as GameObject;    
        GameObject esp = PrefabUtility.InstantiatePrefab(Resources.Load("EnemySpawnPoint")) as GameObject;   
        GameObject player = PrefabUtility.InstantiatePrefab(Resources.Load("Player")) as GameObject;     
        GameObject gameController = PrefabUtility.InstantiatePrefab(Resources.Load("GameManager")) as GameObject;       
        GameObject enemy = PrefabUtility.InstantiatePrefab(Resources.Load("Enemy")) as GameObject;        
        GameObject enemyWP = PrefabUtility.InstantiatePrefab(Resources.Load("EnemyWayPoints")) as GameObject;
        GameObject winTrigger = PrefabUtility.InstantiatePrefab(Resources.Load("WinTrigger")) as GameObject;     
        psp.transform.position = new Vector3(0, 1, 0);
        winTrigger.transform.position = new Vector3(-15, 1.5f, -15);
        esp.transform.position = new Vector3(10, 0, 10);


        ////SETUP GAME CONTROLLER
        gameController.GetComponent<GameControll>().player = player.GetComponent<PlayerController>();
        gameController.GetComponent<GameControll>().inventory = player.GetComponent<Inventory>();
        gameController.GetComponent<GameControll>().enemy = enemy.GetComponent<Enemy>();
        gameController.GetComponent<GameControll>().playerSpawnPoint = psp.transform;
        gameController.GetComponent<GameControll>().enemySpawnPoint = esp.transform;
      

        ////PLAYER SETUP
        player.GetComponent<PlayerController>().inventory = player.GetComponent<Inventory>();
        player.GetComponent<PlayerController>().gameControll = gameController.GetComponent<GameControll>();
        player.GetComponent<Inventory>().database = gameController.GetComponent<ItemsDatabase>();
        player.GetComponent<Inventory>().DropButton = gameController.GetComponent<GameControll>().dropImage;
        player.GetComponent<PlayerController>().imageStand = gameController.GetComponent<GameControll>().standImage;
        player.GetComponent<PlayerController>().imageCrouch = gameController.GetComponent<GameControll>().crouchImage;
        player.GetComponent<PlayerController>().imageExitHidePlace = gameController.GetComponent<GameControll>().hidePlaceExitImage;
        player.GetComponent<PlayerController>().cameraTransform.GetComponent<Interact>().interactButton = gameController.GetComponent<GameControll>().interactImage;
        player.transform.position = psp.transform.position;


        ////ENEMY SETUP
        enemy.GetComponent<Enemy>().player = player.GetComponent<PlayerController>();
        enemy.GetComponent<Enemy>().wayPoints = new Transform[enemyWP.transform.childCount];
        enemy.transform.position = esp.transform.position;
 

        ////WINPOINT SETUP
        UnityEventTools.AddPersistentListener(winTrigger.GetComponent<TriggerEvents>().interactEvent, gameController.GetComponent<GameControll>().GameWin);

        for (int i = 0; i < enemyWP.transform.childCount; i++)
        {
            enemy.GetComponent<Enemy>().wayPoints[i] = enemyWP.transform.GetChild(i).transform;
        }

        Debug.Log("<color=green>Your scene is ready!</color>");
        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());

    }



}