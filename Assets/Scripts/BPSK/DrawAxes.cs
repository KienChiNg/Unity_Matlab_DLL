using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using TMPro;
using UnityEngine;

public class DrawAxes : MonoBehaviour
{
    public List<LineRenderer> lineRenderersY;
    public List<LineRenderer> lineRenderersYChild;
    public Transform transformCanvas;
    private float xLength = 12;
    private float yLength = 3f;
    private int labelXTotal = 12;
    public GameObject labelPrefab; // Prefab có TextMeshPro để làm nhãn
    // private float lineRenderWidth = 0.04f;
    int M, N;
    private List<Vector3> v3AxesX = new List<Vector3>();
    private List<Vector3> v3AxesY = new List<Vector3>();
    private List<GameObject> txtXList = new List<GameObject>();
    private List<GameObject> txtYList = new List<GameObject>();

    void Awake()
    {
        MATLABInterop.Instance.OnDrawLine += CreateLabels;

        DrawInit();
    }
    void InitText()
    {
        for (int i = 0; i < labelXTotal; i++)
        {
            GameObject label = Instantiate(labelPrefab, Vector3.zero, Quaternion.identity, transformCanvas);
            label.SetActive(false);
            txtXList.Add(label);
        }

        for (int i = 0; i < lineRenderersYChild.Count; i++)
        {
            GameObject label = Instantiate(labelPrefab, Vector3.zero, Quaternion.identity, transformCanvas);
            label.SetActive(false);
            txtYList.Add(label);
        }
    }
    void DrawInit()
    {
        InitText();

        DrawAxis(lineRenderersY[0], 0.08f);
        DrawAxis(lineRenderersY[1], 0.08f);
        lineRenderersY[0].SetPosition(0, new Vector3(0, -0.2f, 0));
        lineRenderersY[0].SetPosition(1, new Vector3(xLength, -0.2f, 0));
        lineRenderersY[1].SetPosition(0, new Vector3(0, yLength, 0));
        lineRenderersY[1].SetPosition(1, new Vector3(xLength, yLength, 0));
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
        this.N = N;

        CalLabelPos();

        DrawLabelX();
        DrawLabelY();

    }
    void DrawLabelX()
    {
        for (int i = 0; i < labelXTotal; i++)
        {
            txtXList[i].SetActive(false);
        }

        int t = N;
        int k = 0;

        while (t >= 100)
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
                CreateLabel(txtXList[i - 1], v3AxesX[label], $"{label}");
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
                CreateLabel(txtXList[i - 1], v3AxesX[label], $"{label}");
            }
        }
    }
    void DrawLabelY()
    {
        int t = 0;

        for (int i = 0; i < lineRenderersYChild.Count; i++)
        {
            txtYList[i].SetActive(false);
            lineRenderersYChild[i].enabled = true;
        }

        if (M >= 4)
            t = (int)M / (lineRenderersYChild.Count - 1);
        else
            t = (int)M / (lineRenderersYChild.Count - 2);

        for (int i = 0; i < lineRenderersYChild.Count; i++)
        {
            if (i >= M)
            {
                lineRenderersYChild[i].enabled = false;
            }
            else
            {
                DrawAxis(lineRenderersYChild[i], 0.02f);
                lineRenderersYChild[i].SetPosition(0, new Vector3(0, v3AxesY[t * i].y, 0));
                lineRenderersYChild[i].SetPosition(1, new Vector3(xLength, v3AxesY[t * i].y, 0));
                CreateLabel(txtYList[i], v3AxesY[t * i], $"{t * i}");
            }
        }
    }
    void CalLabelPos()
    {
        v3AxesX.Clear();
        v3AxesY.Clear();

        Vector3 pos = Vector3.zero;
        v3AxesX.Add(pos);
        for (int i = 0; i <= N - 1; i++)
        {
            pos = new Vector3(i * xLength / (N - 1) + 0.05f, -0.5f, 0);
            v3AxesX.Add(pos);
        }

        for (int i = 0; i < M; i++)
        {
            pos = new Vector3(-0.2f, i * yLength / M, 0);
            v3AxesY.Add(pos);
        }
    }
    void CreateLabel(GameObject label, Vector3 position, string text)
    {
        label.SetActive(true);
        label.transform.localPosition = position;
        label.GetComponent<TMP_Text>().text = text;
    }
}
