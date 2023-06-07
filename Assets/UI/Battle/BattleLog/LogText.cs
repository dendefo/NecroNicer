using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;

public class LogText : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [SerializeField] RectTransform rect;
    [SerializeField] TMPro.TMP_Text _text;
    public TooltipUIpromp promp;

    [SerializeField] UnitScript _unitReference;

    [SerializeField] Color EnemyColor;
    public UnitScript UnitReference
    {
        get { return _unitReference; }
        set
        {
            _unitReference = value;
            _text.text = value.EnemyScriptableObjects.Name;
            promp.header = value.EnemyScriptableObjects.Name;
            promp.content = value.EnemyScriptableObjects.Description;
            promp.enabled = true;
            if (value.IsEnemy) _text.color = EnemyColor;
        }
    }
    [SerializeField] Face _faceReference;
    public Face FaceReference
    {
        get { return _faceReference; }
        set
        {
            _faceReference = value;
            _text.text = value.Stats.Name;
            promp.header = value.Stats.Name;
            promp.content = value.Stats.Description;
            promp.enabled = true;
        }
    }
    [SerializeField] Effect _effectReference;
    public Effect EffectReference
    {
        get { return _effectReference; }
        set
        {
            _effectReference = value;
            _text.text = value.Name;
            promp.header = value.Name;
            promp.content = value.Description;
            promp.enabled = true;
            _text.color = value.TextColor;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (UnitReference != null)
        {
            UnitReference.TurnOutline(true);
            
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (UnitReference != null)
        {
            UnitReference.OnMouseExit();
        }
    }

    void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right && ((BattleSceneManager)MySceneManager.Instance).inBattle)
        {
            if (!UnitReference.IsEnemy)
            {
                ((BattleSceneManager)MySceneManager.Instance).PlayerUnitWindow.UnitReference = UnitReference;
            }
            else ((BattleSceneManager)MySceneManager.Instance).EnemyUnitWindow.UnitReference = UnitReference;
        }
    }
}
