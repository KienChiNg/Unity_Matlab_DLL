using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MathWorks.MATLAB.NET.Arrays;
using System;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Numerics;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using Vector3 = UnityEngine.Vector3;
using UnityEngine.Networking;

public class MatlabRobotArmManager : MonoBehaviour
{
    public static MatlabRobotArmManager instance;
    private double[,] rotList;
    public Robot robot;
    public GameObject box;
    public GameObject target;
    public Transform tfInit;
    private bool isProcess;
    private bool isContraryInit;
    private bool isContraryEnd;
    private string url = "http://127.0.0.1:3000/matlab";
    public bool IsProcess { get => isProcess; set => isProcess = value; }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    void Start()
    {
        // GetValueAngleRotate(0,0);
    }
    public void Handle(Vector3 posObj, Vector3 posDes)
    {
        StartCoroutine(Process(posObj, posDes));
    }
    IEnumerator Process(Vector3 posObj, Vector3 posDes) //Quy trình
    {
        isProcess = true;

        RobotArmUIManager.instance.SetStateHandle(false);

        Vector3 armInit = RobotArmConfig.ARMINIT;

        yield return GetValueAngleRotate(armInit, posObj);
        yield return GrabCube();
        yield return GetValueAngleRotate(posObj, armInit, 2);
        yield return GetValueAngleRotate(armInit, posDes);
        yield return DropCube();
        yield return GetValueAngleRotate(posDes, armInit, 2);

        RobotArmUIManager.instance.SetStateHandle(true);

        isProcess = false;
    }
    IEnumerator GetValueAngleRotate(Vector3 posInit, Vector3 posEnd, int caseInx = 0) //Tính toán trong MATLAB DLL
    {
// #if UNITY_STANDALONE_WIN
        var kcFunction = new RobotArm.Class1();
        MWArray[] result = kcFunction.RobotArm(1,
                            -Math.Abs(posInit.x), posInit.y, posInit.z,
                            -Math.Abs(posEnd.x), posEnd.y, posEnd.z);

        ConvertMWArrayList(result[0]);
// #endif

// #if UNITY_WEBGL || UNITY_EDITOR
        // yield return GetRequest(url,
        //     -Math.Abs(posInit.x),
        //     posInit.y, posInit.z,
        //     -Math.Abs(posEnd.x),
        //     posEnd.y,
        //     posEnd.z);
// #endif
        CheckNumberPositive(posInit.x, posEnd.x);

        yield return StartCoroutine(RotateAuto(caseInx));
    }
    IEnumerator GetRequest(string uri, float X1, float Y1, float Z1, float X2, float Y2, float Z2)
    {
        MyData myData = new MyData();
        myData.code_matlab = $"startup_rvc\r\nclc;\r\nclear all;\r\nclose all;\r\n\r\nX1 = {X1};\r\nY1 = {Y1};\r\nZ1 = {Z1};\r\nX2 = {X2};\r\nY2 = {Y2};\r\nZ2 = {Z2};\r\n\r\ngripping_point = 0.1678;\r\n\r\nL(1) = Revolute('d',0.2358,'a',0,'alpha',-pi/2);\r\nL(2) = Revolute('d',0,'a',0.32,'alpha',-pi);\r\nL(3) = Revolute('d',0,'a',0.0735,'alpha',-pi/2);\r\nL(4) = Revolute('d',-0.25,'a',0,'alpha',pi/2);\r\nL(5) = Revolute('d',0,'a',0,'alpha',-pi/2);\r\nL(6) = Revolute('d',-gripping_point,'a',0,'alpha',pi);\r\nrobot = SerialLink(L);\r\n    \r\nt = [0:0.1:2];\r\nT1 = transl(-X1,-Z1, Y1) * trotx(180, \"deg\");\r\nT = transl(-X2,-Z2, Y2) * trotx(180, \"deg\");\r\nqi1 = robot.ikine(T);\r\nqf1 = robot.ikine(T1);\r\n\r\ng = jtraj(qf1,qi1,t)";

        string jsonData = JsonUtility.ToJson(myData);
        UnityWebRequest request = new UnityWebRequest(url, "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("Access-Control-Allow-Credentials", "true");
        request.SetRequestHeader("Access-Control-Allow-Methods", "GET, POST, OPTIONS");
        request.SetRequestHeader("Access-Control-Allow-Headers", "Accept, X-Access-Token, X-Application-Name, X-Request-Sent-Time");
        request.SetRequestHeader("Access-Control-Allow-Origin", "*");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            MyData2 data2 = JsonUtility.FromJson<MyData2>(request.downloadHandler.text);
            HandleString(data2.res);
        }
        else
        {
            Debug.LogError("Error: " + request.error);
        }
    }
    private void HandleString(string input)
    {
        string[] parts = input.Split(new char[] { ' ', '/' }, StringSplitOptions.RemoveEmptyEntries);

        double[] data = new double[parts.Length];
        for (int i = 0; i < parts.Length; i++)
        {
            // Debug.Log(parts[i]);
            data[i] = double.Parse(parts[i]);
        }

        List<double> list = new List<double>(data);
        list = list.Select(x => x * 180 / Math.PI).ToList();
        ConvertMatlabListToRotList2(list);
    }

    private void CheckNumberPositive(float startNum, float endNum)//Kiểm tra trường hợp vs góc quay ngược 180 đổi với trục X
    {
        if (startNum > 0)
            isContraryInit = true;
        else
            isContraryInit = false;

        if (endNum > 0)
            isContraryEnd = true;
        else
            isContraryEnd = false;
    }
    IEnumerator RotateAuto(int caseInx)//Xét trường hợp ưu tiên của các trục xoay
    {
        switch (caseInx)
        {
            case 0:
                yield return StartCoroutine(SetUpJAngle(0));
                for (int i = 5; i > 0; i--)
                {
                    yield return StartCoroutine(SetUpJAngle(i));
                    // yield return new WaitForSeconds(0.08f);
                }
                break;
            case 1:
                for (int i = 0; i < 6; i++)
                {
                    yield return StartCoroutine(SetUpJAngle(i));
                    // yield return new WaitForSeconds(0.08f);
                }
                break;
            case 2:
                yield return StartCoroutine(SetUpJAngle(1));
                for (int i = 0; i < 6; i++)
                {
                    if (i != 1)
                        yield return StartCoroutine(SetUpJAngle(i));
                    // yield return new WaitForSeconds(0.08f);
                }
                break;
        }
    }
    IEnumerator SetUpJAngle(int inx)// Xét góc quay
    {
        // robot.J1Angle = (float)rotList[i, 0];
        // robot.J2Angle = (float)rotList[i, 1];
        // robot.J3Angle = (float)rotList[i, 2];
        // robot.J4Angle = (float)rotList[i, 3];
        // robot.J5Angle = (float)rotList[i, 4];
        // robot.J6Angle = (float)rotList[i, 5];

        robot.SetStateHightlightJoint(inx);

        float joint = 0;

        for (int i = 0; i < rotList.GetLength(0); i++)
        {
            if ((isContraryEnd || isContraryInit) && (inx == 0 || inx == 5))
            {
                float angleEnd = 0;
                float divide = 0;

                if (isContraryInit && !isContraryEnd)
                {
                    angleEnd = (float)rotList[0, inx];
                    divide = rotList.GetLength(0) - i - 1;
                }
                else if (!isContraryInit && isContraryEnd)
                {
                    angleEnd = (float)rotList[rotList.GetLength(0) - 1, inx];
                    divide = i + 1;

                }
                // Debug.Log(divide);
                joint = (180 - Math.Abs(angleEnd)) * (angleEnd / Math.Abs(angleEnd)) * divide / rotList.GetLength(0);

            }
            else
                joint = (float)rotList[i, inx];


            GetJoint(inx, joint);

            yield return new WaitForSeconds(0.04f);
        }

        RobotArmUIManager.instance.SetValueTextJoint(
            (float)rotList[0, inx],
            (float)rotList[rotList.GetLength(0) - 1, inx],
            inx);

        robot.SetStateHightlightJoint();
    }

    void GetJoint(int inx, float angle)
    {
        switch (inx)
        {
            case 0:
                robot.J1Angle = angle;
                break;
            case 1:
                robot.J2Angle = angle;
                break;
            case 2:
                robot.J3Angle = angle;
                break;
            case 3:
                robot.J4Angle = angle;
                break;
            case 4:
                robot.J5Angle = angle;
                break;
            case 5:
                robot.J6Angle = angle;
                break;
        }
    }
    void ConvertMatlabListToRotList(List<double> matlabList)// Phân loại dữ liệu
    {
        int countChild = matlabList.Count / 6;
        rotList = new double[countChild, 6];
        for (int i = 0; i < countChild; i++)
        {
            for (int j = 0; j < 6; j++)
            {
                rotList[i, j] = matlabList[countChild * j + i];
                if (j == 1) rotList[i, j] += 90;
            }
        }
    }
    void ConvertMatlabListToRotList2(List<double> matlabList)// Phân loại dữ liệu
    {
        int countChild = matlabList.Count / 6;
        rotList = new double[countChild, 6];
        for (int i = 0; i < countChild; i++)
        {
            for (int j = 0; j < 6; j++)
            {
                rotList[i, j] = matlabList[6 * i + j];
                if (j == 1) rotList[i, j] += 90;
            }
        }
    }
    void ConvertMWArrayList(MWArray mWArray)// Chuyển đổi kiểu dữ liệu MW -> list
    {
        if (mWArray is MWNumericArray numericArray)
        {
            double[] data = numericArray.ToVector(MWArrayComponent.Real) as double[];
            List<double> list = new List<double>(data);
            list = list.Select(x => x * 180 / Math.PI).ToList();
            ConvertMatlabListToRotList(list);
        }
        else
        {
            throw new ArgumentException("MWArray không phải là loại MWNumericArray");
        }
    }
    IEnumerator GrabCube() //Gắp
    {
        robot.SetSpeedAnim(1);
        yield return new WaitForSeconds(3.5f);
        robot.SetSpeedAnim(0);

        SetBoxParent(robot.GetContainPos());
    }
    IEnumerator DropCube()//Thả
    {

        robot.SetSpeedAnim(-1);
        yield return new WaitForSeconds(3.5f);
        robot.SetSpeedAnim(0);

        SetBoxParent(tfInit);
    }
    private void SetBoxParent(Transform transform)
    {
        box.transform.parent = transform;
    }
    public void SetPosBox(float valueX, float valueY, float valueZ)
    {
        box.transform.localPosition = new Vector3(valueX, valueY, valueZ);
    }
    public void SetRotBox(float valueX, float valueY, float valueZ)
    {
        box.transform.eulerAngles = new Vector3(valueX, valueY, valueZ);
    }
    public void SetPosTarget(float valueX, float valueY, float valueZ)
    {
        target.transform.localPosition = new Vector3(valueX, valueY, valueZ);
    }
}
