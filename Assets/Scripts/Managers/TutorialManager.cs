using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    public bool DiceDrag = false;
    public GameObject DiceDragWindow;
    public bool UnitClick = false;
    public GameObject UnitClickWindow;
    public bool UnitDrag = false;
    public GameObject UnitDragWindow;
    public bool ThrowDice = false;
    public GameObject ThrowDiceWindow;
    public bool TryRightClick = false;
    public GameObject TryRightClickWindow;
    public bool TryClickOrDrag = false;
    public GameObject TryClickOrDragWindow;
    public bool InventoryDrag = false;
    public GameObject InventoryDragWindow;


    private void Start()
    {
        DiceDragWindow.SetActive(true);
    }
    private void Update()
    {
        try
        {
            if (DiceDrag && !UnitClick) UnitClickWindow.SetActive(true);
            if (UnitClick && !UnitDrag) UnitDragWindow.SetActive(true);
            if (((BattleSceneManager)MySceneManager.Instance).inBattle && !ThrowDice) ThrowDiceWindow.SetActive(true);
            if (ThrowDice && !TryRightClick) TryRightClickWindow.SetActive(true);
            if (((BattleSceneManager)MySceneManager.Instance).inBattle && ThrowDice && !TryClickOrDrag) TryClickOrDragWindow.SetActive(true);
            if (GameManager.Instance.NumberOfVictories != 0 && !InventoryDrag) InventoryDragWindow.SetActive(true);
        }
        catch { }
    }
    public void CloseDrag()
    {
        DiceDrag = true;
        DiceDragWindow.SetActive(false);
    }
    public void CloseUnitClick()
    {
        UnitClick = true;
        UnitClickWindow.SetActive(false);
    }
    public void CloseUnitDrag()
    {
        UnitDrag = true;
        UnitDragWindow.SetActive(false);
    }
    public void ThrowDiceClose()
    {
        ThrowDice = true;
        ThrowDiceWindow.SetActive(false);
    }
    public void TryRightClickClose()
    {
        TryRightClick = true;
        TryRightClickWindow.SetActive(false);
    }
    public void TryClickOrDragClose()
    {
        TryClickOrDrag = true;
        TryClickOrDragWindow.SetActive(false);
    }
    public void InventoryDragClose()
    {
        InventoryDrag = true;
        InventoryDragWindow.SetActive(false);
    }
}
