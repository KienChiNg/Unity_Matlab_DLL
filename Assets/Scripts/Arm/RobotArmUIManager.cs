using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RobotArmUIManager : MonoBehaviour
{
    public static RobotArmUIManager instance;
    [Header("Thông số ban đầu")]
    public Slider sliderXInit;
    public Slider sliderZInit;
    public Slider sliderXEnd;
    public Slider sliderZEnd;
    public TMP_Text txtXInit;
    public TMP_Text txtZInit;
    public TMP_Text txtXEnd;
    public TMP_Text txtZEnd;
    public ScrollRect scrollRect;
    public GameObject handlerGo;
    public GameObject loadingGo;
    [Header("Kết quả")]
    public TMP_Text txtJoint;

    private float valueXInit;
    private float valueZInit;
    private float valueXEnd;
    private float valueZEnd;
    private float valueY = -0.05f;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }
    private void Start()
    {
        sliderXInit.onValueChanged.AddListener(delegate { OnValueChanged(sliderXInit, out valueXInit, txtXInit, 0); });
        sliderZInit.onValueChanged.AddListener(delegate { OnValueChanged(sliderZInit, out valueZInit, txtZInit, 1); });
        sliderXEnd.onValueChanged.AddListener(delegate { OnValueChanged(sliderXEnd, out valueXEnd, txtXEnd, 2); });
        sliderZEnd.onValueChanged.AddListener(delegate { OnValueChanged(sliderZEnd, out valueZEnd, txtZEnd, 3); });

        sliderXInit.value = -0.40f;
        sliderZInit.value = -0.40f;
        sliderXEnd.value = -0.40f;
        sliderZEnd.value = 0.40f;

        // SetValueTextJoint(0, 0, 0, 0, 0, 0);

    }
    private void OnValueChanged(Slider slider, out float value, TMP_Text txtSlider, int inx)
    {
        value = Mathf.Round(slider.value * 100) / 100f;

        if (ViolaRange(value) && !IsCheckInViolaRange(inx))
        {
            if (value > 0) slider.value = 0.25f;
            else slider.value = -0.25f;
        }
        txtSlider.text = value.ToString();

        if (inx < 2)
            MatlabRobotArmManager.instance.SetPosBox(valueXInit, valueY, valueZInit);
        else
            MatlabRobotArmManager.instance.SetPosTarget(valueXEnd, valueY, valueZEnd);
    }
    private bool IsCheckInViolaRange(int inx)
    {
        switch (inx)
        {
            case 0:
                if (ViolaRange(sliderZInit.value)) return false;
                return true;
            case 1:
                if (ViolaRange(sliderXInit.value)) return false;
                return true;
            case 2:
                if (ViolaRange(sliderZEnd.value)) return false;
                return true;
            case 3:
                if (ViolaRange(sliderXEnd.value)) return false;
                return true;
            default:
                return false;
        }
    }
    private bool ViolaRange(float value) //Kiểm tra khoảng cho phép
    {
        if (value > -0.25 && value < 0.25) return true;
        else return false;
    }

    public void Active()
    {
        Vector3 posObj = new Vector3(valueXInit, valueY, valueZInit);
        Vector3 posDes = new Vector3(valueXEnd, valueY, valueZEnd);
        MatlabRobotArmManager.instance.Handle(posObj, posDes);
    }
    public void SetValueTextJoint(float JA, float JB, int inx)
    {
        txtJoint.text += $"\nJ{inx + 1}:    <color=#0BE8E3>{JA.ToString("0.00")} <voffset=0.2em>→</voffset> {JB.ToString("0.00")}</color>";
        scrollRect.verticalNormalizedPosition = 0f;
    }
    public void SetStateHandle(bool state)
    {
        if (!state) txtJoint.text = "";
        else
        {
            sliderXInit.onValueChanged.RemoveAllListeners();
            sliderXInit.value = sliderXEnd.value;
            valueXInit = Mathf.Round(sliderXEnd.value * 100) / 100f;
            txtXInit.text = valueXInit.ToString();
            sliderXInit.onValueChanged.AddListener(delegate { OnValueChanged(sliderXInit, out valueXInit, txtXInit, 0); });

            sliderZInit.onValueChanged.RemoveAllListeners();
            sliderZInit.value = sliderZEnd.value;
            valueZInit = Mathf.Round(sliderZEnd.value * 100) / 100f;
            txtZInit.text = valueZInit.ToString();
            sliderZInit.onValueChanged.AddListener(delegate { OnValueChanged(sliderZInit, out valueZInit, txtZInit, 1); });

            MatlabRobotArmManager.instance.SetRotBox(0, 0, 0);
            MatlabRobotArmManager.instance.SetPosBox(valueXInit, valueY, valueZInit);
        }
        handlerGo.SetActive(state);
        loadingGo.SetActive(!state);
    }
}
