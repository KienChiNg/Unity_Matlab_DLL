using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Joint : MonoBehaviour
{
    public GameObject armInfo;
    // private bool isSelected;
    private void OnMouseEnter()
    {
        if (!MatlabRobotArmManager.instance.IsProcess)
            SetStateSelectArmInfo(true);
    }
    private void OnMouseExit()
    {
        if (!MatlabRobotArmManager.instance.IsProcess)
            SetStateSelectArmInfo(false);
    }
    public void SetStateSelectArmInfo(bool isSelected)
    {
        // this.isSelected = isSelected;
        armInfo.SetActive(isSelected);
    }
}
