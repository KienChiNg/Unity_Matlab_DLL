using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MathWorks.MATLAB.NET.Arrays;
using System;
using System.IO;
using Unity.VisualScripting;

public class MATLABInterop : MonoBehaviour
{
    public static MATLABInterop Instance;
    public event Action<int, int> OnDrawLine;
    private List<double> wave1 = new List<double>();
    private List<double> wave2 = new List<double>();
    private List<Vector3> dotChart = new List<Vector3>();
    public LineRenderer lineRendererWave1;
    public LineRenderer lineRendererWave2;
    public GameObject dotChartGOParent;
    public GameObject dotChild;
    public int N = 10;
    public int M = 16;
    private int axisX = 12;
    private int axisY_1 = 3;
    // private int axisY = 2;
    // private int axisXDot = 6;
    // private int axisYDot = 6;
    private float lineRenderWidth = 0.04f;
    private List<GameObject> dotList = new List<GameObject>();
    void Awake()
    {
        Instance = this;


        // SetInitLineRenderer();
        // Draw(N,M); 
        InitDot();
    }
    public void InitDot()
    {
        for (int i = 0; i < 1000; i++)
        {
            GameObject dot = Instantiate(dotChild, transform.position, Quaternion.identity, dotChartGOParent.transform);
            dot.SetActive(false);
            dotList.Add(dot);
        }
    }
    public void Draw(int N, int M)
    {
        this.N = N;
        this.M = M;

        CalInDll();
        SetInitLineRenderer();
        DrawLineWave1();
        DrawLineWave2();
        DrawDotChart();

        OnDrawLine?.Invoke(M, N);
    }
    void CalInDll()
    {
        var kcFunction = new BPSKFuncionSynthesise.KienChiMATLAB();
        MWArray[] result = kcFunction.BPSKFuncionSynthesise(3, N, M);
        // Debug.Log(result);

        string str = "******************* Công thức 1 ******************* \n\n" + result[0].ToString() + "\n\n"
                + "******************* Công thức 2 ******************* \n\n" + result[1].ToString() + "\n\n"
                + "******************* Công thức 3 ******************* \n\n" + result[2].ToString() + "\n\n";

        OutFile(str);

        wave1 = ConvertMWArrayList(result[0]);
        wave2 = ConvertMWArrayList(result[1]);
        dotChart = ConvertMWArrayListV2(result[2]);
    }
    void SetInitLineRenderer()
    {
        lineRendererWave1.useWorldSpace = false;
        lineRendererWave1.positionCount = 2 * (N - 1);
        lineRendererWave1.startWidth = lineRenderWidth;
        lineRendererWave1.endWidth = lineRenderWidth;

        lineRendererWave2.useWorldSpace = false;
        lineRendererWave2.positionCount = N * 100;
        lineRendererWave2.startWidth = lineRenderWidth;
        lineRendererWave2.endWidth = lineRenderWidth;
    }
    void DrawLineWave1()
    {
        int j = 0;
        for (int i = 0; i < lineRendererWave1.positionCount; i++)
        {
            float posX = (float)j / (N - 1) * axisX;
            if (i % 2 != 0)
            {
                float posY = (float)(wave1[j - 1] / M * axisY_1);
                lineRendererWave1.SetPosition(i, new Vector3(posX, posY, 0));
            }
            else
            {
                float posY = (float)(wave1[j] / M * axisY_1);
                lineRendererWave1.SetPosition(i, new Vector3(posX, posY, 0));
                j++;
            }
        }
    }
    void DrawLineWave2()
    {
        for (int i = 0; i < lineRendererWave2.positionCount; i++)
        {
            float j = (float)i * axisX / (N * 100);
            lineRendererWave2.SetPosition(i, new Vector3(j, (float)wave2[i], 0));
        }
    }

    void DrawDotChart()
    {
        for (int i = 0; i < dotList.Count; i++)
        {
            dotList[i].SetActive(false);
        }
        for (int i = 0; i < dotChart.Count; i++)
        {
            dotList[i].SetActive(true);
            //Không chia tỉ lệ vì tỷ lệ tương ứng 3:3
            dotList[i].transform.localPosition = dotChart[i];
        }
    }

    void OutFile(string longMessage)
    {
        string projectPath = Path.Combine(Application.dataPath, "Resources");
        string filePath = Path.Combine(projectPath, "log.txt");
        File.WriteAllText(filePath, longMessage);
        // Debug.Log(projectPath);
    }
    List<double> ConvertMWArrayList(MWArray mWArray)
    {
        if (mWArray is MWNumericArray numericArray)
        {
            double[] data = numericArray.ToVector(MWArrayComponent.Real) as double[];
            List<double> list = new List<double>(data);
            return list;
        }
        else
        {
            throw new ArgumentException("MWArray không phải là loại MWNumericArray");
        }
    }
    List<Vector3> ConvertMWArrayListV2(MWArray mWArray)
    {
        if (mWArray is MWNumericArray numericArray)
        {
            double[] realArray = (double[])numericArray.ToVector(MWArrayComponent.Real);

            double[] imaginaryArray = (double[])numericArray.ToVector(MWArrayComponent.Imaginary);

            List<Vector3> v3 = new List<Vector3>();
            if (realArray.Length == imaginaryArray.Length)
            {
                for (int i = 0; i < realArray.Length; i++)
                {
                    Vector3 vector = new Vector3((float)realArray[i], (float)imaginaryArray[i], 0);
                    v3.Add(vector);
                }
            }
            return v3;
        }
        else
        {
            throw new ArgumentException("MWArray không phải là loại MWNumericArray");
        }
    }
}
