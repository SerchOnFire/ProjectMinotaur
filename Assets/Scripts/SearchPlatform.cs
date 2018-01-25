using UnityEngine;
using System.Collections;

public class SearchPlatform : MonoBehaviour {

    bool searching = false;
    bool find = false;
    Vector3 nextPosition;
    GameObject otherPlatform;

	void Start () { }
	
	void Update ()
    {
        if(searching)
            Search();
    }

    void Search()
    {
        RaycastHit hit;
        Ray platformToVectorDown = new Ray(gameObject.transform.position + Vector3.up / 2, Vector3.down);

        gameObject.layer += 1;
        int layerMask = 1 << LayerMask.NameToLayer("Platforms");

        if (Physics.Raycast(platformToVectorDown, out hit, (float)2, layerMask))
        {
            otherPlatform = hit.collider.gameObject;
            find = true;
            nextPosition = hit.collider.gameObject.transform.position;
        }
        gameObject.layer -= 1;
    }

    public void StartSearch ()
    {
        searching = true;  
    }

    public void StopSearch()
    {
        searching = false;
    }

    public bool GetSearching()
    {
        return searching;
    }

    public bool GetFind()
    {
        return find;
    }

    public void SetFind(bool value)
    {
        find = value;
    }

    public Vector3 GetNextPosition()
    {
        return nextPosition;
    }

    public GameObject GetOtherPlatform()
    {
        return otherPlatform;
    }
}
