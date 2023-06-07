using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseInteractionUnit : MonoBehaviour
{
    public UnitScript UnitScript;
    public TooltipUIpromp promp;
    public UnitsDragDrop dragNdrop;
    private void OnMouseEnter()
    {

        promp.OnMouseEnter();
        UnitScript.OnMouseEnter();
    }
    private void OnMouseExit()
    {
        promp.OnMouseExit();
        UnitScript.OnMouseExit();
    }
    private void OnMouseDown()
    {
        dragNdrop.OnMouseDown();

    }
    private void OnMouseUp()
    {
        dragNdrop.OnMouseUp();
    }
    private void OnMouseOver()
    {
        promp.OnMouseOver();
        dragNdrop.OnMouseOver();

    }
}
