using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Settings : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown Dropdown;
    public void ChangeResolution()
    {
        float aspectRatio16by9 = 16f / 9f;
        float aspectRatio16by10 = 16f / 10f;
        float aspectRatio4by3 = 4f / 3f;

        switch (Dropdown.value)
        {
            case 0:
                Screen.SetResolution(2560, (int)(1440.0f * aspectRatio16by9), true);

                break;
            case 1:
                Screen.SetResolution(1920, (int)(1080 * aspectRatio16by9), true);
                break;
            case 2:
                Screen.SetResolution(1680, (int)(1050 * aspectRatio16by10), true);
                break;
            case 3:
                Screen.SetResolution(1600, (int)(900 * aspectRatio16by9), true);
                break;
            case 4:
                Screen.SetResolution(1440, (int)(900 * aspectRatio16by10), true);
                break;
            case 5:
                Screen.SetResolution(1400, (int)(1050 * aspectRatio4by3), true);
                break;
            case 6:
                Screen.SetResolution(1366, 768, true);
                break;
            case 7:
                Screen.SetResolution(1360, 768, true);
                break;
            case 8:
                Screen.SetResolution(1280, 1024, true);
                break;
            case 9:
                Screen.SetResolution(1280, (int)(960 * aspectRatio4by3), true);
                break;
            case 10:
                Screen.SetResolution(1280, (int)(900 * aspectRatio16by10), true);
                break;
            case 11:
                Screen.SetResolution(1280, (int)(800 * aspectRatio16by10), true);
                break;
            case 12:
                Screen.SetResolution(1280, 768, true);
                break;
            case 13:
                Screen.SetResolution(1280, (int)(720 * aspectRatio16by9), true);
                break;
            case 14:
                Screen.SetResolution(1280, 600, true);
                break;
            case 15:
                Screen.SetResolution(1152, (int)(864 * aspectRatio4by3), true);
                break;
            case 16:
                Screen.SetResolution(1024, (int)(768 * aspectRatio4by3), true);
                break;
            case 17:
                Screen.SetResolution(800, (int)(600 * aspectRatio4by3), true);
                break;

        }
    }
}
