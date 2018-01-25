using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class LevelBuilder : MonoBehaviour {

    JsonManager jsonManager;
    List<ObjectInfo> objectsInfo;

    private bool loaded = false;
    public bool Loaded
    {
        get { return loaded; }
    }

    void Start()
    {
        LoadGameObjectsInfo("Level001");
        LoadGameObjects();
    }

    void LoadGameObjectsInfo(string name)
    {
        jsonManager = new JsonManager();
        jsonManager.LoadJson(name);
        objectsInfo = jsonManager.ObjectsInfo;
        jsonManager = null;
    }

    void LoadGameObjects()
    {
        for (int i = 0, iEnd = objectsInfo.Count; i < iEnd; i++)
        {
            Debug.Log("-------------------------");

            Debug.Log(this + "objectsInfo[i].GameObjectName: " + objectsInfo[i].Folder + "/" + objectsInfo[i].GameObjectName);

            if (!objectsInfo[i].GameObjectName.Contains("Exit") && !objectsInfo[i].GameObjectName.Contains("Entrance") && objectsInfo[i].GameObjectName.Contains("Platform"))
                objectsInfo[i].GameObjectName = "Platform01";

            Debug.Log(this + "objectsInfo[i].GameObjectName: " + objectsInfo[i].Folder + "/" + objectsInfo[i].GameObjectName);
            GameObject temp = Instantiate(Resources.Load(objectsInfo[i].Folder + "/" + objectsInfo[i].GameObjectName) as GameObject);
            temp.name = objectsInfo[i].GameObjectName;

            temp.transform.position = new Vector3(
                float.Parse(objectsInfo[i].PositionX)*10, 
                float.Parse(objectsInfo[i].PositionY), 
                float.Parse(objectsInfo[i].PositionZ) * 10);

            temp.transform.rotation = new Quaternion(
                float.Parse(objectsInfo[i].RotationX), 
                float.Parse(objectsInfo[i].RotationY), 
                float.Parse(objectsInfo[i].RotationZ), 
                float.Parse(objectsInfo[i].RotationW));
        }
        loaded = true;
        objectsInfo.Clear();
        objectsInfo = null;
    }

}
