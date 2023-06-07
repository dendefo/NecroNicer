using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ShowFaceWindow : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [SerializeField] InventoryItemHandler item;
    [SerializeField] FaceWindow windowPrefab;
    static FaceWindow activeWindow = null;
    void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
    {
        if (InventoryManager.Instance.transform.GetChild(0).gameObject.active&& activeWindow==null)
        {
            if (item.Item.Face == null) return;
            activeWindow = Instantiate(windowPrefab, GameManager.Instance.InventoryWindow.transform);
            activeWindow.Reference = item.Item.Face.faceDefenition;
            //var thisPos = Input.mousePosition;
            //var ScreenVector = new Vector3(Screen.width, Screen.height);
            
            
            //if (thisPos.x <= ScreenVector.x / 2)
            //{
            //    if (thisPos.y <= ScreenVector.y / 3)
            //    {
            //        activeWindow.rectTransform.pivot = new(0,1);
            //    }
            //    else if (thisPos.y <= (ScreenVector.y / 3) * 2&& thisPos.y > ScreenVector.y / 3)
            //    {
            //        activeWindow.rectTransform.pivot = new(0, 0.5f);
            //    }
            //    else
            //    {
            //        activeWindow.rectTransform.pivot = new(0, 0);
            //    }
            //}
            //else
            //{
            //    if (thisPos.y <= ScreenVector.y / 3)
            //    {
            //        activeWindow.rectTransform.pivot = new(1, 1);
            //    }
            //    else if (thisPos.y <= (ScreenVector.y / 3) * 2 && thisPos.y > ScreenVector.y / 3)
            //    {
            //        activeWindow.rectTransform.pivot = new(1, 0.5f);
            //    }
            //    else
            //    {
            //        activeWindow.rectTransform.pivot = new(1, 0);
            //    }
            //}
            
        }
        else if (activeWindow!=null) activeWindow.Reference = item.Item.Face.faceDefenition;
        Debug.Log(eventData.button.ToString());
    }

    void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
    {
        return;
    }

    void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
    {

        return;
    }
    
}
