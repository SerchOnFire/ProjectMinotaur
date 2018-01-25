using UnityEngine;
using System.Collections;

public class BoxInteraction : MonoBehaviour {

    //Stage
     LevelBuilder levelBuilder;
    public Camera gameplayCamera;
    Vector3 firstMousePosition;
    bool firstClick = false;
    float distance;
    float minDistanceToMove = 5;

    //Platform
    GameObject gameObjectSelected;
    SearchPlatform searchPlatform;
    PlatformRotation platformRotation;
    MovePlatform movePlatform;

    void Start ()
    {
        levelBuilder = GetComponent<LevelBuilder>();
    }
	
	void Update () {
        /*if (levelBuilder.Loaded)
        {
            if (Input.GetMouseButtonDown(0))
                OnMouseButtonDown();

            if (Input.GetMouseButtonUp(0))
                OnMouseButtonUp();  

            if (Input.GetMouseButton(0))
                OnMouseButtonHeldDown();
        }*/

        if (Input.GetMouseButtonDown(0))
            OnMouseButtonDown();

        if (Input.GetMouseButtonUp(0))
            OnMouseButtonUp();

        if (Input.GetMouseButton(0))
            OnMouseButtonHeldDown();
    }

    void OnMouseButtonDown()
    {
        Debug.Log(this + "OnMouseButtonDown A");
        Ray cameraToStage = new Ray(gameplayCamera.ScreenToWorldPoint(Input.mousePosition), gameplayCamera.transform.forward);
        RaycastHit hit;
        int layerMask = 1 << LayerMask.NameToLayer("Platforms");

        if (Physics.Raycast(cameraToStage, out hit, 500, layerMask))
        {
            Debug.Log(this + "OnMouseButtonDown B");
            gameObjectSelected = hit.collider.gameObject;
            SetScripts();
            firstMousePosition = Input.mousePosition;
            SetFirstClick(true);
        }
    }

    void SetScripts()
    {
        Debug.Log(this + "SetScripts, gameObjectSelected: " + gameObjectSelected.name);
        searchPlatform = gameObjectSelected.GetComponent<SearchPlatform>();
        platformRotation = gameObjectSelected.GetComponent<PlatformRotation>();
        movePlatform = gameObjectSelected.GetComponent<MovePlatform>();
        movePlatform.SetLastPosition(gameObjectSelected.transform.position);
        movePlatform.SetCamera(gameplayCamera);
    }

    void OnMouseButtonUp()
    {
        Debug.Log(this + "OnMouseButtonUp A");
        if (GetFirstClick() && !movePlatform.GetMove() && !platformRotation.GetRotate())
        {
            Debug.Log(this + "OnMouseButtonUp B");
            if (searchPlatform != null && searchPlatform.GetSearching())
                searchPlatform.StopSearch();

            if (platformRotation != null && distance < minDistanceToMove)
                platformRotation.StartRotation();

            if (movePlatform != null && movePlatform.GetDrag())
            {
                Debug.Log(this + "OnMouseButtonUp C");
                movePlatform.StopDrag();

                if (searchPlatform != null && searchPlatform.GetFind())
                    ChangePositions();
                else
                    BackToLastPosition();
            }
            ClearScripts();
        }
        SetFirstClick(false);
    }

    void BackToLastPosition()
    {
        searchPlatform.SetFind(false);
        MovePlatformToPosition(movePlatform, movePlatform.GetLastPosition(), gameObjectSelected.transform.position);
    }

    void ChangePositions()
    {
        searchPlatform.SetFind(false);
        MovePlatformToPosition(movePlatform, searchPlatform.GetNextPosition(), gameObjectSelected.transform.position);

        GameObject otherPlatform = searchPlatform.GetOtherPlatform();
        MovePlatform otherMovePlatform = otherPlatform.GetComponent<MovePlatform>();

        MovePlatformToPosition(otherMovePlatform, movePlatform.GetLastPosition(), otherPlatform.transform.position);
    }

    void MovePlatformToPosition(MovePlatform moveTemp, Vector3 nextPosition, Vector3 currentPosition)
    {
        moveTemp.SetNextPosition(nextPosition);
        moveTemp.CalculateLinearTrajectory(currentPosition);
        moveTemp.StartMove();
    }

    void ClearScripts()
    {
        gameObjectSelected = null;
        platformRotation = null;
        searchPlatform = null;
        movePlatform = null;
    }

    void OnMouseButtonHeldDown()
    {
        if(GetFirstClick() && !movePlatform.GetMove() && !platformRotation.GetRotate())
        {
            distance = Vector3.Distance(firstMousePosition, Input.mousePosition);
            if(distance > minDistanceToMove)
                MoveGameObject();
        }
    }

    void MoveGameObject()
    {
        if (movePlatform != null && !movePlatform.GetDrag())
            movePlatform.StartDrag();

        if (searchPlatform != null && !searchPlatform.GetSearching())
            searchPlatform.StartSearch();
    }

    void SetFirstClick(bool value)
    {
        firstClick = value;
    }

    public bool GetFirstClick()
    {
        return firstClick;
    }
}
