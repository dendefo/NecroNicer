using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TooltipSystem : MonoBehaviour
{
    private static TooltipSystem current;

    public Tooltip tooltip;

    public static string currentLocation = "Center";

    public void Awake()
    {
        current = this;
    }

    public static void ShowToolTip(string header, string content ="")
    {
        HideToolTip();
        current.tooltip.SetText(header, content);
        current.tooltip.Tool.SetActive(true);
    }
    public static void HideToolTip()
    {
        try { current.tooltip.Tool.SetActive(false); }
        catch { Debug.Log("Implement Tooltip"); }
    }
}
