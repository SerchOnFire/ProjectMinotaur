using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections.Generic;
using System.IO;
using UnityEngine.SceneManagement;

public class JsonManager
{
    JsonContainer jsonContainer;
    ObjectInfo objectInfo;

    GameObject[] tempObjects;
    protected string content;
    public string Content
    {
        get { return content; }
    }

    string extension = ".json";
    string json;
    string tempObjectInfo;
    string version;
    string path;

    private List<ObjectInfo> objectsInfo;
    public List<ObjectInfo> ObjectsInfo
    {
        get { return objectsInfo; }
    }

    public JsonManager()
    {
        objectsInfo = new List<ObjectInfo>();
        path = "Assets/Resources/levels/";
    }

    public void SetInfo(string path, string version)
    {
        this.path = path;
        this.version = version;
    }

    public void SaveGameObjectsData(string name, string version, string data = null)
    {
        jsonContainer = new JsonContainer();
        jsonContainer.JsonName = name;
        jsonContainer.Version = version;
        tempObjects = GameObject.FindObjectsOfType<GameObject>();

        if (data == null)
        {
            for (int i = 0, iEnd = tempObjects.Length; i < iEnd; i++)
            {
                objectInfo = new ObjectInfo();
                objectInfo.GameObjectName = tempObjects[i].name;

                if (tempObjects[i].transform.parent != null)
                    objectInfo.GameObjectParent = tempObjects[i].transform.parent.name.ToString();

                objectInfo.PositionX = tempObjects[i].transform.position.x.ToString();
                objectInfo.PositionY = tempObjects[i].transform.position.y.ToString();
                objectInfo.PositionZ = tempObjects[i].transform.position.z.ToString();

                objectInfo.RotationX = tempObjects[i].transform.rotation.x.ToString();
                objectInfo.RotationY = tempObjects[i].transform.rotation.y.ToString();
                objectInfo.RotationZ = tempObjects[i].transform.rotation.z.ToString();
                objectInfo.RotationW = tempObjects[i].transform.rotation.w.ToString();

                tempObjectInfo = JsonUtility.ToJson(objectInfo);
                jsonContainer.InfoObjects.Add(tempObjectInfo);
            }
            json = JsonUtility.ToJson(jsonContainer);
        }
        else
        {
            json = data;
        }
        SaveFile(name);
    }

    public void SaveSelectedGameObjectsData(string name, string version, GameObject[] gameObjects, string data = null)
    {
        jsonContainer = new JsonContainer();
        jsonContainer.JsonName = name;
        jsonContainer.Version = version;

        if (data == null)
        {
            for (int i = 0, iEnd = gameObjects.Length; i < iEnd; i++)
            {
                objectInfo = new ObjectInfo();
                objectInfo.GameObjectName = gameObjects[i].name;

                if (gameObjects[i].name.Contains("Platform"))
                    objectInfo.Folder = "Platforms";
                if (gameObjects[i].name.Contains("Player") || gameObjects[i].name.Contains("Enemie") || gameObjects[i].name.Contains("Hostage"))
                    objectInfo.Folder = "Characters";

                if (gameObjects[i].transform.parent != null)
                    objectInfo.GameObjectParent = gameObjects[i].transform.parent.name.ToString();

                objectInfo.PositionX = gameObjects[i].transform.position.x.ToString();
                objectInfo.PositionY = gameObjects[i].transform.position.y.ToString();
                objectInfo.PositionZ = gameObjects[i].transform.position.z.ToString();

                objectInfo.RotationX = gameObjects[i].transform.rotation.x.ToString();
                objectInfo.RotationY = gameObjects[i].transform.rotation.y.ToString();
                objectInfo.RotationZ = gameObjects[i].transform.rotation.z.ToString();
                objectInfo.RotationW = gameObjects[i].transform.rotation.w.ToString();

                tempObjectInfo = JsonUtility.ToJson(objectInfo);
                jsonContainer.InfoObjects.Add(tempObjectInfo);
            }
            json = JsonUtility.ToJson(jsonContainer);
        }
        else
        {
            json = data;
        }
        SaveFile(name);
    }

    void SaveFile(string name)
    {
        string file;
        if (name.Contains(extension))
            file = path + name;
        else
            file = path + name + extension;
        Debug.Log(this + "-SaveFile, file: " + file);
        using (FileStream fs = new FileStream(file, FileMode.Create))
        {
            using (StreamWriter writer = new StreamWriter(fs))
            {
                writer.Write(json);
            }
        }
        #if UNITY_EDITOR
        AssetDatabase.Refresh();
        #endif
    }

    public void LoadJson(string jsonName)
    {
        var streamReader = new StreamReader(path + jsonName + extension);
        var data = streamReader.ReadToEnd();
        content = data;
        streamReader.Close();

        jsonContainer = new JsonContainer();
        jsonContainer = JsonUtility.FromJson<JsonContainer>(data);

        for (int i = 0, iEnd = jsonContainer.InfoObjects.Count; i < iEnd; i++)
        {
            objectInfo = new ObjectInfo();
            objectInfo = JsonUtility.FromJson<ObjectInfo>(jsonContainer.InfoObjects[i]);
            objectsInfo.Add(objectInfo);
        }
    }


}