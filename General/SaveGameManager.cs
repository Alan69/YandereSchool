using System.Collections.Generic;
using UnityEngine;
using System.Xml.Linq;
using System.IO;

public class SaveGameManager : MonoBehaviour {


    private GameControll gameControll;

    private string path;

  
    public List<ObjectSaveID> objectsToSave = new List<ObjectSaveID>();

    private void Awake()
    {
        path = Application.persistentDataPath + "/save.xml";
        gameControll = GetComponent<GameControll>();
      
    }

    private void Start()
    {
        if (PlayerPrefs.HasKey("LoadGame"))
        {
            if (PlayerPrefs.GetInt("LoadGame") == 1 && (File.Exists(path)))
            {
                Load();
                Debug.Log("GameLoaded!");
            }
        }
    }


    public void Save()
    {

        XElement gen = new XElement("gen");
        foreach (ObjectSaveID obj in objectsToSave)
        {
            gen.Add(obj.GetElement());
        }

        gen.Add(new XElement("lifes",gameControll.lifeCount));

        XDocument doc = new XDocument(gen);

        File.WriteAllText(path,doc.ToString());
        Debug.Log(path);
        gameControll.MainMenuExit(1);

    }


    public void Load()
    {
        XElement gen = null;

        if (File.Exists(path))
        {
            
            gen = XDocument.Parse(File.ReadAllText(path)).Element("gen");
        }else
        {
            Debug.Log("Save file not found!");
            return;
        }

        LoadSceneData(gen);


    }

    private void LoadSceneData(XElement root)
    {
        int lf = int.Parse(root.Element("lifes").Value);
        Debug.Log(lf);
        gameControll.lifeCount = lf;

        for (int i = 0; i < objectsToSave.Count; i++)
            {
            XElement inst = CheckObjectID(i, root);

              if (inst != null)
              {
                if (objectsToSave[i].objectType == ObjectType.item)
                {
                    Vector3 pos = Vector3.zero;
                    Vector3 rot = Vector3.zero;
                    pos.x = float.Parse(inst.Attribute("x").Value);
                    pos.y = float.Parse(inst.Attribute("y").Value);
                    pos.z = float.Parse(inst.Attribute("z").Value);
                    rot.x = float.Parse(inst.Attribute("rx").Value);
                    rot.y = float.Parse(inst.Attribute("ry").Value);
                    rot.z = float.Parse(inst.Attribute("rz").Value);
                    objectsToSave[i].transform.position = pos;
                    objectsToSave[i].transform.eulerAngles = rot;
                }


                if (objectsToSave[i].objectType == ObjectType.door)
                {
                    bool lck = bool.Parse(inst.Attribute("locked").Value);
                    int state = int.Parse(inst.Attribute("state").Value);
                    int locks = int.Parse(inst.Attribute("locks").Value);
                    objectsToSave[i].GetComponent<Door>().locked = lck;
                    objectsToSave[i].GetComponent<Door>().state = state;
                    objectsToSave[i].GetComponent<Door>().locksCount = locks;
                    objectsToSave[i].GetComponent<Door>().LoadState();
                }

                if(objectsToSave[i].objectType == ObjectType.switchable)
                {

                }

                if (objectsToSave[i].objectType == ObjectType.puzzle)
                {

                    Vector3 pos = Vector3.zero;
                    pos.x = float.Parse(inst.Attribute("x").Value);
                    pos.y = float.Parse(inst.Attribute("y").Value);
                    pos.z = float.Parse(inst.Attribute("z").Value);
                    int puzzleID = int.Parse(inst.Attribute("puzzleID").Value);
                    objectsToSave[i].transform.position = pos;
                    objectsToSave[i].GetComponent<PuzzleBlock>().ID = puzzleID;
                }

                if (objectsToSave[i].objectType == ObjectType.locks)
                {

                    bool state = bool.Parse(inst.Attribute("lockState").Value);
                    Vector3 pos = Vector3.zero;
                    Vector3 rot = Vector3.zero;
                    pos.x = float.Parse(inst.Attribute("x").Value);
                    pos.y = float.Parse(inst.Attribute("y").Value);
                    pos.z = float.Parse(inst.Attribute("z").Value);
                    rot.x = float.Parse(inst.Attribute("rx").Value);
                    rot.y = float.Parse(inst.Attribute("ry").Value);
                    rot.z = float.Parse(inst.Attribute("rz").Value);
                    objectsToSave[i].transform.position = pos;
                    objectsToSave[i].transform.eulerAngles = rot;
                    objectsToSave[i].GetComponent<Lock>().isOpen = state;
                    objectsToSave[i].GetComponent<Lock>().LoadState();
                }


                if (objectsToSave[i].objectType == ObjectType.triggerEvent)
                {

                    bool state = bool.Parse(inst.Attribute("state").Value);
                    objectsToSave[i].GetComponent<TriggerEvents>().activated = state;
                    objectsToSave[i].GetComponent<TriggerEvents>().LoadState();
                }

            }
            else
              {
                Destroy(objectsToSave[i].gameObject);
              }
            }

        
    }


    private XElement CheckObjectID(int i,XElement root)
    {
       
       foreach (XElement inctance in root.Elements("instance"))
       {
        int id = int.Parse(inctance.Attribute("id").Value);
          if (id == objectsToSave[i].objectID)
          {
           return inctance;
          }
       }
        return null;

    }
}
