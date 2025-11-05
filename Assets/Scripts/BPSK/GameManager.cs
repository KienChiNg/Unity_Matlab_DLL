using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public TMP_InputField N;
    public TMP_Dropdown M;
    private int[] M_Init = { 2, 4, 8, 16, 32 };
    private void Start() {
        N.text = $"{10}";
        M.value = 1;

        Draw();
    }
    public void Draw()
    {
        // Đầu vào 1 < N < 1000
        MATLABInterop.Instance.Draw(Int32.Parse(N.text),M_Init[M.value]);
    }
}
