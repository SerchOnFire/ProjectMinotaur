using UnityEngine;
using System.Collections;

public class PlatformRotation : MonoBehaviour {

    bool rotate = false;
    float currentYAngle;
    float nextYAngle;

    void Start ()
    {
        SetYAngle();
        SetNextYAngle();
    }
	
	void Update ()
    {
        if (rotate)
            RotatePlatform();
    }

    void RotatePlatform()
    {
        if (currentYAngle < nextYAngle && nextYAngle - currentYAngle < 300)
        {
            transform.eulerAngles = new Vector3(transform.eulerAngles.x, (transform.eulerAngles.y + 360 * Time.deltaTime * 1.5f), transform.eulerAngles.z);
            SetYAngle();
        }
        else
        {
            if(nextYAngle == 360)
               nextYAngle = 0;
            transform.eulerAngles = new Vector3(transform.eulerAngles.x, nextYAngle, transform.eulerAngles.z);
            SetYAngle();
            SetNextYAngle();
            rotate = false;
        }
    }

    public void StartRotation()
    {
        rotate = true;
    }

    void SetYAngle()
    {
        currentYAngle = transform.eulerAngles.y;
    }

    void SetNextYAngle()
    {
        nextYAngle = transform.eulerAngles.y + 90;
    }

    public bool GetRotate()
    {
        return rotate;
    }

}
