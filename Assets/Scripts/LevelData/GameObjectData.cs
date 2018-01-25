using UnityEngine;
using ProjectEnums;

[System.Serializable]
public class GameObjectData
{
    public string platformName;
    public string parentName;
    public string id;

    public bool isRotationAvailable;
    public bool isDragAvailable;

    public Vector3 localPosition;
    public Quaternion localRotation;
    public Vector3 localScale;

    public Style platformStyle;
}
