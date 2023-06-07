using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TimeLinePortrete : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [SerializeField] UnitScript _unit;
    public UnitScript unit
    {
        get { return _unit; }
        set { _unit = value; try { promp.content = _unit.EnemyScriptableObjects.Name; } catch { Debug.Log("Here it is!"); } }
    }
    public Image image;
    public TooltipUIpromp promp;
    public Image PortreteFrame;

    void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
    {
        unit.TurnOutline(true);


    }
    public void OnPointerExit(PointerEventData eventData)
    {
        unit.TurnOutline(false);
    }

    void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right && ((BattleSceneManager)MySceneManager.Instance).inBattle)
        {
            if (!unit.IsEnemy)
            {
                ((BattleSceneManager)MySceneManager.Instance).PlayerUnitWindow.UnitReference = unit;
            }
            else ((BattleSceneManager)MySceneManager.Instance).EnemyUnitWindow.UnitReference = unit;
        }
    }
}
