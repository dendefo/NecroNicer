using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitPlacable : MonoBehaviour
{
    public FloorType floorType;
    public Vector3 UnitPosition;
    public Vector3 GridPosition;
    public void Place(GameObject Unit)
    {
        if (floorType !=FloorType.Player) throw new("Cannot place here");
        if (transform.childCount > 0) throw new("There is Already Unit on This Cell");
        Unit.transform.parent = transform;
        Unit.transform.localPosition = UnitPosition;
    }
}
public enum FloorType
{
    Neutral,
    Enemy,
    Player
}
