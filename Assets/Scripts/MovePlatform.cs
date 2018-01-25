using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MovePlatform : MonoBehaviour {

    bool drag = false;
    bool move = false;
    int index = 0;
    int divider = 20;

    Vector3 upInc = Vector3.up * 0.25f;
    Camera gameplayCamera;
    Vector3 lastPosition;
    Vector3 nextPosition;
    List<Vector3> trajectory;

    void Start () {}

    void Update()
    {
        if (drag)
            DragPlatform();

        if (move)
            MoveToNextPosition();
    }

    void MoveToNextPosition()
    {
        if (index < trajectory.Count)
        {
            transform.position = trajectory[index];
            index++;
        }
        else
        {
            move = false;
        }
    }

    void DragPlatform()
    {
        Ray cameraToStage = new Ray(gameplayCamera.ScreenToWorldPoint(Input.mousePosition), gameplayCamera.transform.forward);
        RaycastHit hit;
        int layerMask = 1 << LayerMask.NameToLayer("DragCollider");

        if (Physics.Raycast(cameraToStage, out hit, (float)25, layerMask))
            transform.position = hit.point;
    }

    public void CalculateLinearTrajectory(Vector3 currentPosition)
    {
        index = 0;
        trajectory = new List<Vector3>();
        Vector3 direction = nextPosition - currentPosition;
        
        Debug.DrawLine(nextPosition, currentPosition, Color.red, 30);
        Vector3 directionFraction = direction / (float)divider;

        for (int i = 0; i < divider; i++)
            if (i < divider - 1)
                trajectory.Add(currentPosition + direction * ((float)i / (float)divider));
            else
                trajectory.Add(nextPosition);

       // Debug.DrawLine(currentPosition + upInc, currentPosition + direction + upInc, Color.red, 10);
        //Debug.LogError(this + "Pause");
    }

    public void CalculateCuadraticTrajectory(Vector3 currentPosition)
    {
        index = 0;
        trajectory = new List<Vector3>();
        Vector3 direction = nextPosition - currentPosition;

        for (int i = 0; i < divider; i++)
            if (i < divider - 1)
                trajectory.Add(currentPosition + direction * ((float)i / (float)divider));
            else
                trajectory.Add(nextPosition);

        Debug.DrawLine(currentPosition + upInc, currentPosition + direction + upInc, Color.blue, 10);
        //Debug.LogError(this + "Pause");
    }

    public void StartDrag()
    {
        drag = true;
    }

    public void StopDrag()
    {
        drag = false;
    }

    public bool GetDrag()
    {
        return drag;
    }

    public void SetCamera(Camera camera)
    {
        if(gameplayCamera == null)
            gameplayCamera = camera;
    }

    public void SetNextPosition(Vector3 position)
    {
        nextPosition = position;
    }

    public void SetLastPosition(Vector3 position)
    {
        lastPosition = position;
    }

    public Vector3 GetLastPosition()
    {
        return lastPosition;
    }

    public void StartMove()
    {
        move = true;
    }

    public bool GetMove()
    {
        return move;
    }
}
