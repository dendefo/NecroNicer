using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


public class UnitsDragDrop : MonoBehaviour
{
    public delegate void DragEndedDelagte(UnitsDragDrop dragableunit);
    public DragEndedDelagte DragendedCallback;
    [SerializeField] UnitScript unitScript;

    private TooltipUIpromp tooltipUIpromp;
    public bool isDraggable = false;
    public bool isDragging = false;

    static public bool Drag = false;

    public GameObject gridObject;

    public Vector3 StartPosition;
    public Vector3 LastPosition;
    public Vector3 offset;

    public ContactFilter2D filter2D;

    void Start()
    {
        StartPosition = transform.localPosition;
        LastPosition = transform.localPosition;
        tooltipUIpromp = GetComponent<TooltipUIpromp>();
    }

    void Update()
    {
        if (isDragging)
        {
            Vector3 temp = Camera.main.ScreenToWorldPoint(Input.mousePosition) + offset;
            transform.position = new Vector3(temp.x,temp.y,-2);

            if (tooltipUIpromp != null && !Input.GetMouseButton(0))
            {
                tooltipUIpromp.enabled = true;
            }
        }
    }

    
    public void OnMouseDown()
    {
        
        if (!isDraggable) return;
        if (GameManager.Instance.InventoryWindow.active) return;
        offset = transform.position - Camera.main.ScreenToWorldPoint(Input.mousePosition);
        isDragging = true;
        Drag = true;
        GridBuilder._p = true;
        try { gridObject.SetActive(true); }
        catch { }

        TooltipSystem.HideToolTip();

        if (tooltipUIpromp != null)
        {
            tooltipUIpromp.enabled = false;
        }

    }

    public void OnMouseUp()
    {
        if (!isDraggable) return;
        if (GameManager.Instance.InventoryWindow.active) return;
        isDragging = false;
        Drag = false;
        //DragendedCallback(this);
        RaycastHit2D hit;
        hit = Physics2D.GetRayIntersection(GameManager.Instance.camera.ScreenPointToRay(GameManager.Instance.camera.WorldToScreenPoint(transform.position)),100f ,filter2D.layerMask);
        try 
        {
            hit.collider.transform.GetChild(0).GetComponent<UnitPlacable>().Place(gameObject);
            LastPosition = transform.position;
            unitScript.UnitAnimator.enabled = true;
            
        }
        catch
        {
            transform.position = LastPosition;
        }
        GridBuilder._p = false;
        foreach (UnitScript Unit in GameManager.Instance.PlayersUnits)
        {
            if (Unit.DragAndDrop.StartPosition != Unit.transform.position) continue;
            else return;
        }
        ((BattleSceneManager)BattleSceneManager.Instance).CloseWindow();
        try
        {
            gridObject.SetActive(false);
        }
        catch { }
        if (tooltipUIpromp != null && !Input.GetMouseButton(0))
        {
            tooltipUIpromp.enabled = true;
        }
    }

    public void OnMouseOver()
    {

        if (Input.GetMouseButtonDown(1) && GetComponent<UnitScript>().UnitsDice != null&& !((BattleSceneManager)BattleSceneManager.Instance).inBattle)
        {
            isDraggable = true;
            GetComponent<UnitScript>().UnitsDice.SetActive(false);
            
        }
    }
}
