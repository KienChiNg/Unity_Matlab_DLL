using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using TMPro;
using UnityEngine;

public class DrawAxes3 : MonoBehaviour
{
    public List<LineRenderer> lineRenderersX;
    public List<LineRenderer> lineRenderersY;
    public Transform transformCanvas;
    private float xLength = 6f;
    private float yLength = 6f;
    public GameObject labelPrefab; // Prefab có TextMeshPro để làm nhãn
    int M, N;

    void Start()
    {
        // DrawAxisX();
        DrawInit();
        CreateLabels();
    }
    void DrawInit()
    {
        for (int i = 0; i < lineRenderersX.Count; i++)
        {
            if (i == 0 || i == lineRenderersX.Count - 1)
            {
                DrawAxis(lineRenderersX[i], 0.08f);
                DrawAxis(lineRenderersY[i], 0.08f);
            }
            else
            {
                DrawAxis(lineRenderersX[i], 0.02f);
                DrawAxis(lineRenderersY[i], 0.02f);
            }
            lineRenderersX[i].SetPosition(0, new Vector3(i - yLength / 2, -yLength / 2 - 0.04f, 0));
            lineRenderersX[i].SetPosition(1, new Vector3(i - yLength / 2, yLength / 2 + 0.04f, 0));
            CreateLabel(new Vector3(i - yLength / 2 + 0.1f, -yLength / 2 - 0.3f, 0), $"{i - yLength / 2}");

            lineRenderersY[i].SetPosition(0, new Vector3(-xLength / 2, i - xLength / 2, 0));
            lineRenderersY[i].SetPosition(1, new Vector3(xLength / 2, i - xLength / 2, 0));
            CreateLabel(new Vector3(-yLength / 2 - 0.2f, i - yLength / 2, 0), $"{i - yLength / 2}");

        }
    }
    void DrawAxis(LineRenderer lineRenderer, float width = 0.04f)
    {
        lineRenderer.positionCount = 2;
        lineRenderer.useWorldSpace = false;
        lineRenderer.startWidth = width;
        lineRenderer.endWidth = width;
        // lineRenderer.SetPosition(0, new Vector3(0, -0.2f, 0));
        // lineRenderer.SetPosition(1, new Vector3(xLength, -0.2f, 0));
    }

    void CreateLabels()
    {
        M = MATLABInterop.Instance.M;
        N = MATLABInterop.Instance.N;

    }

    void CreateLabel(Vector3 position, string text)
    {
        GameObject label = Instantiate(labelPrefab, Vector3.zero, Quaternion.identity, transformCanvas);
        label.transform.localPosition = position;
        label.GetComponent<TMP_Text>().text = text;
    }
}
