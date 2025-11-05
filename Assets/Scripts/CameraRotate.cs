using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CameraRotate : MonoBehaviour
{

    public float speedH = 4.0f;
    public float speedV = 4.0f;
    public float speedZoom = 4.0f;

    public float yaw = 0.0f;
    public float pitch = 0.0f;

    public float autoRotateSpeed = 0.05f;

    bool isPress = false;

    public GameObject cam;



    // Use this for initialization
    void Start()
    {
        transform.eulerAngles = new Vector3(pitch, yaw, 0.0f);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0)&&(!EventSystem.current.IsPointerOverGameObject()))
        {
            isPress = true;
        }
        if (Input.GetMouseButtonUp(0))
        {
            isPress = false;
        }

        if (isPress)
        {
            yaw = transform.eulerAngles.y;
            yaw += speedH * Input.GetAxis("Mouse X");
            pitch -= speedV * Input.GetAxis("Mouse Y");

            //yaw = Mathf.Clamp(yaw, 110f, 260f);
            pitch = Mathf.Clamp(pitch, 0f, 55f);

            transform.eulerAngles = new Vector3(pitch, yaw, 0.0f);
        }
        else
        {
            transform.eulerAngles += new Vector3(0.0f, autoRotateSpeed, 0.0f);
        }

        if((cam.transform.localPosition.z<=-1f) &&(Input.GetAxis("Mouse ScrollWheel") >0) && (!EventSystem.current.IsPointerOverGameObject()))
        {
            cam.transform.Translate(0, 0, Input.GetAxis("Mouse ScrollWheel") * speedZoom);
        }
        if ((cam.transform.localPosition.z >= -2.0f) && (Input.GetAxis("Mouse ScrollWheel") < 0) && (!EventSystem.current.IsPointerOverGameObject()))
        {
            cam.transform.Translate(0, 0, Input.GetAxis("Mouse ScrollWheel") * speedZoom);
        }
    }

    public void mouseDown()
    {
        isPress = true;
    }

    public void MouseUp()
    {
        isPress = false;
    }


}