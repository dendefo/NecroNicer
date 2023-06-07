using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

//This script is sitting on each tile of inventory
public class InventoryItemHandler : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    [SerializeField] Canvas canvas;
    [SerializeField] RawImage image;
    [SerializeField] private InventoryItem _item;
    [SerializeField] TooltipUIpromp tooltip;
    [SerializeField] ShowFaceWindow FaceWindow;

    //InventoryItem is Information about Face that this inventory tile handles
    public InventoryItem Item
    {
        get { return _item; }
        set
        {
            _item = value;
            //Check if there is more than no items left
            if (value.Amount == 0)
            {
                //if true, clean
                image.texture = null;
                image.color = Color.clear;
                transform.GetChild(0).GetComponent<TMPro.TMP_Text>().text = "";
                //tooltip.enabled = false;
                Frame.enabled = false;
            }
            else
            {
                //tooltip.enabled = true;
                //tooltip.header = value.Face.faceDefenition.Stats.Tier.ToString() + "\n" + value.Face.faceDefenition.Stats.Name.ToString();
                //tooltip.content = value.Face.Uses.ToString();
                GetComponent<RawImage>().texture = value.Face.faceDefenition.material.GetTexture("_Item");
                GetComponent<RawImage>().color = Color.white;
                transform.GetChild(0).GetComponent<TMPro.TMP_Text>().text = value.Amount.ToString();
                Frame.texture = value.Face.faceDefenition.Frame;
                Frame.enabled = true;
                if (Frame.texture == null) Frame.enabled = false;

            }
        }
    }

    public static InventoryItemHandler drag = null;
    public RawImage Frame;
    public bool isDragged = false;

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left) return;
        TooltipSystem.HideToolTip();
        try { if (eventData.pointerEnter.GetComponent<RawImage>().texture == null) return; }
        catch { return; }
        drag = Instantiate(gameObject, canvas.transform).GetComponent<InventoryItemHandler>();
        drag.isDragged = true;
        drag.Item = new(Item.Face, 1);
        drag.image.raycastTarget = false;

        GameManager.Instance.inventoryManager.InventoryAdd(Item.Face, -1);

        drag.GetComponent<RectTransform>().localScale = Vector3.one * 6;
        drag.image.texture = drag.Item.Face.faceDefenition.material.mainTexture;
        Destroy(drag.transform.GetChild(0).gameObject);

    }

    public void OnDrag(PointerEventData eventData)
    {
        if (drag == null) return;
        //drag.transform.localPosition = new Vector3(Input.mousePosition.x - Screen.width / 2, Input.mousePosition.y - Screen.height / 2,0);
        {
            Camera canvasCam = GetComponentInParent<Canvas>().worldCamera;

            Vector2 mousePos = Input.mousePosition;
            Vector3 minWorldPos = canvasCam.ScreenToWorldPoint(new Vector3(0f, 0f, canvasCam.nearClipPlane));
            Vector3 maxWorldPos = canvasCam.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, canvasCam.nearClipPlane));
            Vector3 worldPos = canvasCam.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, canvasCam.nearClipPlane));

            worldPos = new Vector3(Mathf.Clamp(worldPos.x, minWorldPos.x, maxWorldPos.x), Mathf.Clamp(worldPos.y, minWorldPos.y, maxWorldPos.y), 0.3f);
            float pivotX = worldPos.x < (maxWorldPos.x + minWorldPos.x) / 2 ? 0f : 1f;
            float pivotY = worldPos.y > (maxWorldPos.y * 0.75f) ? 1f : 0f;

            if (pivotX == 0 && pivotY == 1)
            {
                pivotX = 1f;
            }

            //rectTransform.pivot = new Vector2(pivotX, pivotY);
            worldPos = new Vector3(worldPos.x, worldPos.y, 0);
            drag.transform.position = worldPos;
        }
        //drag.transform.position = new Vector3(drag.transform.position.x, drag.transform.position.y, 0);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (drag == null) return;
        var target = eventData.pointerCurrentRaycast.gameObject;
        InventoryItemHandler inv = null;
        RaycastHit hit;
        if (Physics.Raycast(GameManager.Instance.camera.ScreenPointToRay(Input.mousePosition), out hit))
        {
            if (hit.collider.gameObject.tag == "Dice" && hit.collider.transform.parent.tag == "Dice")
            {
                var temp = hit.collider.gameObject.GetComponent<DiceFace>().Face;
                hit.collider.gameObject.GetComponent<DiceFace>().Face = drag.GetComponent<InventoryItemHandler>().Item.Face;
                GameManager.Instance.inventoryManager.InventoryAdd(temp);
            }
            else GameManager.Instance.inventoryManager.InventoryAdd(drag.Item.Face);
        }
        else GameManager.Instance.inventoryManager.InventoryAdd(drag.Item.Face);

        Destroy(drag.gameObject);
        drag = null;
    }
}
