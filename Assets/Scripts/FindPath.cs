using UnityEditor;
using UnityEngine;

public class FindPath : MonoBehaviour
{
    public float lenghtDirection;
    public Vector3 direction;

    public void SetDirection()
    {
        SetDirection(Vector3.forward, Color.blue);
        SetDirection(Vector3.back, Color.green);
        SetDirection(Vector3.right, Color.red);
        SetDirection(Vector3.left, Color.yellow);
    }

    void SetDirection(Vector3 temp, Color color)
    {
        Vector3 tempDirection = temp * lenghtDirection;
        Ray ray = new Ray(transform.position, tempDirection);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, lenghtDirection))
        { }
        else
        {
            Debug.DrawRay(transform.position, tempDirection * 2, color, 100);
            direction = tempDirection * 2;
        }
    }
}
