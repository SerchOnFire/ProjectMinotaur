using UnityEngine;
using System.Collections;

public class MoveForward : MonoBehaviour {

    public bool move;
    public float wait;
    public float time;
    public Vector3 direction;

    void Update ()
    {
        if (move)
        {
            direction = transform.forward * Time.deltaTime / time;
            transform.position = transform.position + direction * 10;
        }
    }

    public void StartMovement()
    {
        StartCoroutine(ChangeState());
    }

    IEnumerator ChangeState()
    {
        yield return new WaitForSeconds(wait);
        move = true;
    }

    public void SetDirection(Vector3 newDirection)
    {
        Debug.DrawLine(transform.position, transform.position + newDirection, Color.cyan, 10);
        transform.LookAt(transform.position + newDirection);
    }
}
