using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class lookAtCam : MonoBehaviour
{
    Transform target;


    // Start is called before the first frame update
    void Start()
    {
        target = Camera.main.transform;

    }

    // Update is called once per frame
    void Update()
    {
        Vector3 temp = new Vector3(target.transform.position.x, target.transform.position.y, target.transform.position.z);
        

        transform.rotation = Quaternion.LookRotation(transform.position - temp);

    }
}
