using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class IndicatorPositions : MonoBehaviour {

    public List<GameObject> indicator;
    public List<EntryType> entry;

    public enum EntryType
    {
        Vertical,
        Horizontal,
        None
    };

    void Start ()
    {
        SetIndicators();
	}
	
    void SetIndicators()
    {
        for (int i = 0, iEnd = indicator.Count; i < iEnd; i++)
        {
            SetPosition(indicator[i].transform, i);
            SetAngle(indicator[i].transform, entry[i], i);
        }
    }

    void SetAngle(Transform indicator, EntryType entry, int index)
    {
        float eulerY = 0;
        if (entry == EntryType.Horizontal)
            eulerY = (index == 0 || index == 3) ? 0 : 180;

        if (entry == EntryType.Vertical)
            eulerY = (index == 0 || index == 1) ? 90 : 270;

        if (entry == EntryType.None)
            eulerY = 90 * index;

        indicator.localRotation = Quaternion.Euler(new Vector3(indicator.eulerAngles.x, eulerY, indicator.eulerAngles.z));
    }

    void SetPosition(Transform indicator, int index)
    {
        float posX = (index == 0 || index == 3) ? -0.25f : 0.25f;
        float posY = (index == 2 || index == 3) ? -0.25f : 0.25f;

        indicator.localPosition = new Vector3(posX, indicator.localPosition.y, posY);
    }
    
}
