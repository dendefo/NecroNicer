using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class TooltipUIpromp : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler  
{
    public string header;
    [Multiline()]
    public string content;

    private IEnumerator tooltipCoroutine;
    [SerializeField] private RectTransform rectTransform;
    private UnitsDragDrop dragDropScript;
    public Vector3 positionChange;

    public RectTransform ParentCanvas;
    public void Start()
    {
        dragDropScript = GetComponent<UnitsDragDrop>();
    }

    public void OnMouseEnter()
    {
        RaycastHit info;
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition),out info))
        {
            TooltipUIpromp a;
            if (!info.collider.TryGetComponent(out a)) return;
        }
            
        if (UnitsDragDrop.Drag) return;
        ActionButtons b;
        if (ActionButtons.Arrow != null && TryGetComponent(out b)) return;
        if (rectTransform == null)
        {
            tooltipCoroutine = ShowTooltipWithDelay();
            StartCoroutine(tooltipCoroutine);
        }
    }

    public void OnMouseOver()
    {
        Tooltip.UpdateMove();
    }

    public void OnMouseExit()
    {
        if (rectTransform == null)
        {
            try
            {
                StopCoroutine(tooltipCoroutine);
                TooltipSystem.HideToolTip();
            }
            catch { }
        }

    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (UnitsDragDrop.Drag) return;
        ActionButtons a;
        if (ActionButtons.Arrow != null && TryGetComponent(out a)) return;
        if (InventoryItemHandler.drag != null) return;
        //if (rectTransform != null)
        {
            Tooltip.Zero();
            tooltipCoroutine = ShowTooltipWithDelay();
            StartCoroutine(tooltipCoroutine);
            Tooltip.Move(transform.position+positionChange);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        //if (rectTransform != null)
        {
            StopCoroutine(tooltipCoroutine);
            TooltipSystem.HideToolTip();
        }
    }

    private IEnumerator ShowTooltipWithDelay()
    {
        yield return new WaitForSeconds(0.5f);

        //if (rectTransform != null)
        //{
        //    Vector2 canvasSize = ParentCanvas.rect.size;

        //    Vector2 canvasPosition;
        //    RectTransformUtility.ScreenPointToLocalPointInRectangle(ParentCanvas, rectTransform.position, null, out canvasPosition);

        //    Vector2 normalizedPosition = new Vector2((canvasPosition.x + canvasSize.x / 2f) / canvasSize.x, (canvasPosition.y + canvasSize.y / 2f) / canvasSize.y);

        //    string location = "";
        //    if (normalizedPosition.x > 0.75f)
        //    {
        //        if (normalizedPosition.y > 0.75f)
        //        {
        //            location = "Top Right Corner";
        //        }
        //        else if (normalizedPosition.y < 0.25f)
        //        {
        //            location = "Bottom Right Corner";
        //        }
        //        else
        //        {
        //            location = "Right Middle";
        //        }
        //    }
        //    else if (normalizedPosition.x < 0.25f)
        //    {
        //        if (normalizedPosition.y > 0.75f)
        //        {
        //            location = "Top Left Corner";
        //        }
        //        else if (normalizedPosition.y < 0.25f)
        //        {
        //            location = "Bottom Left Corner";
        //        }
        //        else
        //        {
        //            location = "Left Middle";
        //        }
        //    }
        //    else
        //    {
        //        if (normalizedPosition.y > 0.75f)
        //        {
        //            location = "Top Middle";
        //        }
        //        else if (normalizedPosition.y < 0.25f)
        //        {
        //            location = "Bottom Middle";
        //        }
        //        else
        //        {
        //            location = "Center";
        //        }
        //    }


        //    TooltipSystem.ShowToolTip(header, content);
        //}
        //else
        {
            TooltipSystem.ShowToolTip(header, content);
        }
    }

}
