using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using static Face.FaceStats;

public class ActionButtons : MonoBehaviour, IPointerClickHandler,IBeginDragHandler,IDragHandler,IEndDragHandler
{
    [SerializeField] TooltipUIpromp promp;
    [SerializeField] RawImage image;
    [SerializeField] InstanceFace _face;

    public ActionButtons Dragged;
    public InstanceFace face
    {
        get { return _face; }
        set 
        { 
            _face = value;
            promp.header = _face.faceDefenition.Stats.Tier.ToString() +"\n"+ _face.faceDefenition.Stats.Name.ToString();
            image.texture = _face.faceDefenition.material.GetTexture("_Item");
        }
    }

    [SerializeField] GameObject ArrowPrefab;
    public static GameObject Arrow;
    public void OnPointerClick(PointerEventData eventData)
    {
        if (face.faceDefenition.Stats.Target == TargetType.Self)
        {
            InstanceFace.ActivateAbility(face, BattleSceneManager.TimeLineQueue.Peek());
        }
        TooltipSystem.HideToolTip();
    }

    void IBeginDragHandler.OnBeginDrag(PointerEventData eventData)
    {
        Dragged = this;
        if (face.faceDefenition.Stats.Target == TargetType.Self) return;
        Arrow = Instantiate(ArrowPrefab, transform);
        foreach (UnitScript unit in face.faceDefenition.Stats.Target ==TargetType.Enemy? ((BattleSceneManager)BattleSceneManager.Instance).Enemies :GameManager.Instance.PlayersUnits)
        {
            unit.TurnOutline(true);
        }
        TooltipSystem.HideToolTip();
    }

    void IDragHandler.OnDrag(PointerEventData eventData)
    {
        //Something
    }

    void IEndDragHandler.OnEndDrag(PointerEventData eventData)
    {
        Destroy(Arrow);
        foreach (UnitScript unit in face.faceDefenition.Stats.Target == TargetType.Enemy ? ((BattleSceneManager)BattleSceneManager.Instance).Enemies : GameManager.Instance.PlayersUnits)
        {
            unit.TurnOutline(false);
        }
        if (UnitScript.Chosen == null) return;

        if (Dragged.face.faceDefenition.Stats.Target == TargetType.Enemy && UnitScript.Chosen.IsEnemy)
        {
            InstanceFace.ActivateAbility(Dragged.face, BattleSceneManager.TimeLineQueue.Peek(), UnitScript.Chosen);
        }
        else if (Dragged.face.faceDefenition.Stats.Target == TargetType.Friend && !UnitScript.Chosen.IsEnemy)
        {
            InstanceFace.ActivateAbility(Dragged.face, BattleSceneManager.TimeLineQueue.Peek(), UnitScript.Chosen);
        }
    }


    
}
