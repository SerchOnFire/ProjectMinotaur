using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class LevelData : ScriptableObject
{
    public string levelName;
    public List<GameObjectData> baseData;
    public List<GameObjectData> platformData;
    public List<GameObjectData> decorationData;
}
