using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

[ExecuteInEditMode()]
public class Tooltip : MonoBehaviour
{
    public TextMeshProUGUI HeaderField;
    public TextMeshProUGUI ContentField;

    public LayoutElement LayoutElement;

    public int CharacterWrapLimit;

    public RectTransform rectTransform;

    public static Tooltip tooltip;
    public GameObject Tool;

    public void Awake()
    {
        tooltip = this;
        rectTransform = GetComponent<RectTransform>();
    }

    public void SetText(string header, string content = "")
    {
        if (string.IsNullOrEmpty(header))
        {
            HeaderField.gameObject.SetActive(false);
        }
        else
        {
            HeaderField.gameObject.SetActive(true);
            HeaderField.text = header;
        }
        ContentField.text = content;
        
        int HeaderLength = HeaderField.text.Length;
        int ContentLength = ContentField.text.Length;

        LayoutElement.enabled = (HeaderLength > CharacterWrapLimit || ContentLength > CharacterWrapLimit) ? true : false;
    }

    public static void Move(Vector3 position)
    {
        tooltip.Tool.transform.position = position;
        tooltip.Tool.transform.position -= new Vector3(0,0,position.z);
    }

    public static void UpdateMove()
    {
        Camera canvasCam = tooltip.Tool.GetComponentInParent<Canvas>().worldCamera;

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

        tooltip.Tool.GetComponent<RectTransform>().pivot = new Vector2(pivotX, pivotY);
        tooltip.Tool.transform.position = worldPos;
    }
    public static void Zero()
    {
        tooltip.Tool.GetComponent<RectTransform>().pivot = Vector2.one;
    }
}
