using UnityEngine;
using System.Collections;

public class PlatformManager : MonoBehaviour {

    public bool isSEPlatform;
    public FindPath findPath;

	void Start ()
    {
        if(isSEPlatform)
            findPath.SetDirection();
	}
	
}
