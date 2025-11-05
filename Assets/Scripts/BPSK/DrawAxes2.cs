using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using TMPro;
using UnityEngine;

public class DrawAxes2 : MonoBehaviour
{
    public List<LineRenderer> lineRenderersY;
    public Transform transformCanvas;
    private float xLength = 12;
    private float yLength = 2f;
    private int labelXTotal = 12;
    public GameObject labelPrefab; // Prefab có TextMeshPro để làm nhãn
    int M, N;
    private List<Vector3> v3AxesX = new List<Vector3>();
    private List<GameObject> txtXList = new List<GameObject>();
    private List<GameObject> txtYList = new List<GameObject>();

    void Awake()
    {
        MATLABInterop.Instance.OnDrawLine += CreateLabels;

        InitText();
        // CreateLabels(4, 2);
    }
    void DrawAxis(LineRenderer lineRenderer, float width = 0.04f)
    {
        lineRenderer.positionCount = 2;
        lineRenderer.useWorldSpace = false;
        lineRenderer.startWidth = width;
        lineRenderer.endWidth = width;
    }

    void CreateLabels(int M, int N)
    {
        this.M = M;
        this.N = N * 100;

        CalLabelPos();

        DrawLabelX();

    }
    void InitText()
    {
        for (int i = 0; i <= labelXTotal; i++)
        {
            GameObject label = Instantiate(labelPrefab, Vector3.zero, Quaternion.identity, transformCanvas);
            label.SetActive(false);
            txtXList.Add(label);
        }
        for (int i = 0; i < lineRenderersY.Count; i++)
        {
            GameObject label = Instantiate(labelPrefab, Vector3.zero, Quaternion.identity, transformCanvas);
            label.SetActive(false);
            txtYList.Add(label);
        }
        DrawLabelY();
    }
    void DrawLabelX()
    {
        for (int i = 0; i <= labelXTotal; i++)
        {
            txtXList[i].SetActive(false);
        }

        int t = N;
        int k = 0;

        while (t > 100)
        {
            t /= 10;
            k++;
        }
        
        // Debug.Log(k + " " + t);
        if (t <= labelXTotal)
        {
            for (int i = 1; i <= t; i++)
            {
                int label = (int)(i * Math.Pow(10, k));
                CreateLabel(txtXList[i], v3AxesX[label], $"{label}");
            }
        }
        else
        {
            int divide = 0;
            int tt = 0;

            if (t < 25)
            {
                divide = 2;
            }
            else
            {
                divide = 5;
            }

            if (t / divide > labelXTotal)
                divide = 10;

            tt = t / divide;

            for (int i = 1; i <= tt; i++)
            {
                int label = (int)(i * divide * Math.Pow(10, k));
                CreateLabel(txtXList[i], v3AxesX[label], $"{label}");
            }
        }
        CreateLabel(txtXList[0], v3AxesX[0], $"{0}");
    }
    void DrawLabelY()
    {
        for (int i = 0; i < lineRenderersY.Count; i++)
        {
            if (i == 0 || i == lineRenderersY.Count - 1)
            {
                DrawAxis(lineRenderersY[i], 0.08f);
            }
            else
            {
                DrawAxis(lineRenderersY[i], 0.02f);
            }
            lineRenderersY[i].SetPosition(0, new Vector3(0, i - yLength / 2, 0));
            lineRenderersY[i].SetPosition(1, new Vector3(xLength, i - yLength / 2, 0));
            CreateLabel(txtYList[i], new Vector3(-0.2f, i - yLength / 2, 0), $"{i - yLength / 2}");

        }
    }
    void CalLabelPos()
    {
        v3AxesX.Clear();

        Vector3 pos = new Vector3(0.5f, -1.5f, 0);
        // v3AxesX.Add(pos);

        for (int i = 0; i <= N; i++)
        {
            pos = new Vector3(i * xLength / (N) + 0.05f, -1.5f, 0);
            v3AxesX.Add(pos);
        }
    }
    void CreateLabel(GameObject label, Vector3 position, string text)
    {
        label.SetActive(true);
        label.transform.localPosition = position;
        label.GetComponent<TMP_Text>().text = text;
    }
}
