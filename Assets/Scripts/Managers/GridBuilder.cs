using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridBuilder : MonoBehaviour
{
    public Grid grid;
    [SerializeField] GameObject prefabTile;

    public List<GameObject> PlayersGrid;
    public static bool _p;
    bool _pp;

    public List<GameObject> EnemiesGrid;
    public static bool _e;
    bool _ee;

    [SerializeField] int highlightLayer;
    [SerializeField] int FloorLayer;

    [SerializeField] Light HighLight;

    public static GridBuilder Instance;
    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
        int StartX = 0;
        int StartY = 1;
        for (Vector3Int xAndY = new Vector3Int(0, StartY, 0); xAndY.x <= 17; xAndY.x++)
        {
            for (; xAndY.y >= -12; xAndY.y--)
            {
                Vector3 a = grid.CellToWorld(xAndY);
                if (a.x > 3.5 || a.y > -0.46 || a.y < -2.1) continue;
                var go = Instantiate(prefabTile, grid.CellToWorld(xAndY), Quaternion.identity, transform);
                try
                {
                    var comp = go.GetComponentInChildren<UnitPlacable>();
                    comp.GridPosition = grid.WorldToCell(go.transform.position);

                    if (go.transform.GetChild(0).position.y > -0.5) continue;
                    if (comp.GridPosition.x - comp.GridPosition.y <= 0) comp.floorType = FloorType.Neutral;
                    else if (comp.GridPosition.x - comp.GridPosition.y == 9) comp.floorType = FloorType.Neutral;
                    else if (comp.GridPosition.x - comp.GridPosition.y >= 18) comp.floorType = FloorType.Neutral;
                    else if (comp.GridPosition.x + comp.GridPosition.y <= 3) comp.floorType = FloorType.Neutral;
                    else if (comp.GridPosition.x - comp.GridPosition.y > 9 && comp.GridPosition.x - comp.GridPosition.y < 18 && comp.GridPosition.x + comp.GridPosition.y > 3)
                    {
                        comp.floorType = FloorType.Enemy;
                        EnemiesGrid.Add(go);
                    }
                    else
                    {
                        PlayersGrid.Add(go);
                        comp.floorType = FloorType.Player;
                    }
                    

                }
                catch { }
            }
            xAndY.y = StartY + xAndY.x + 1;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (_p!=_pp)
        {
            foreach (GameObject go in PlayersGrid)
            {
                go.transform.GetChild(0).gameObject.layer = (_p? highlightLayer : FloorLayer);
                HighLight.color = Color.green;
            }
            _pp = _p;
        }
        if (_e != _ee)
        {
            foreach (GameObject go in EnemiesGrid)
            {
                go.transform.GetChild(0).gameObject.layer = (_e ? highlightLayer : FloorLayer);
                HighLight.color = Color.red;
            }
            _ee = _e;
        }
    }
}
