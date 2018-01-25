using UnityEngine;
using UnityEditor;

public class IntersectionManager : MonoBehaviour {

    public CharacterManager playerManager;
    public FindPath findPath;

    Vector3 direction = new Vector3();

    void OnTriggerEnter(Collider collider)
    {
        //Debug.Log(collider.name);

        if (collider.name == "Indicator")
        {
            findPath = collider.transform.GetComponent<FindPath>();

            if (gameObject.name == "Player")                
                findPath.SetDirection();

            direction = findPath.direction + (findPath.transform.position - transform.position);
            playerManager.moveForward.SetDirection(direction);
        }

        if (collider.name == "Box")
        {
           // Debug.Log(collider.name);
           // Detener contador 
        }

        if (collider.name.Contains("Platform"))
        {
            // Debug.Log(collider.name);
            // Comenzar contador para detectar box collider
        }
    }
}
