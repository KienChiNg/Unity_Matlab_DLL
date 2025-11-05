using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;

public class Robot : MonoBehaviour
{
    public GameObject J1;
    public float J1Angle;
    public GameObject J2;
    public float J2Angle;
    public GameObject J3;
    public float J3Angle;
    public GameObject J4;
    public float J4Angle;
    public GameObject J5;
    public float J5Angle;
    public GameObject J6;
    public float J6Angle;
    public List<Joint> joints;
    public Animator animGrab;
    public Transform contain;
    private void Awake()
    {

    }
    private void Start()
    {
        SetSpeedAnim(0);
        
    }
    private void Update()
    {
        AdjRotaAngle();
    }
    public void AdjRotaAngle()
    {
        J1.transform.localRotation = Quaternion.AngleAxis(-J1Angle, new Vector3(0, 1, 0));
        J2.transform.localRotation = Quaternion.AngleAxis(-J2Angle, new Vector3(1, 0, 0));
        J3.transform.localRotation = Quaternion.AngleAxis(J3Angle, new Vector3(1, 0, 0));
        J4.transform.localRotation = Quaternion.AngleAxis(J4Angle, new Vector3(0, 0, 1));
        J5.transform.localRotation = Quaternion.AngleAxis(J5Angle, new Vector3(1, 0, 0));
        J6.transform.localRotation = Quaternion.AngleAxis(-J6Angle, new Vector3(0, 1, 0));
    }
    public void SetStateHightlightJoint(int inx = -1)
    {
        foreach (Joint joint in joints)
           joint.SetStateSelectArmInfo(false);

        if (inx >= 0 && inx < 5)
            joints[inx].SetStateSelectArmInfo(true);
    }
    public void SetSpeedAnim(float speed)
    {
        if (speed < 0)
            animGrab.StartPlayback();
        animGrab.speed = speed;
        // animGrab.recorderMode = AnimatorRecorderMode.Record;
    }
    public Transform GetContainPos()
    {
        return contain;
    }
}
