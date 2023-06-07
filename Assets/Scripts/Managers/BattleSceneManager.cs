using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Face.FaceStats;

public class BattleSceneManager : MySceneManager
{
    [Header("Battle Scene UI")]
    public GameObject Preparation;
    public GameObject InBattle;
    public GameObject UnitWindow;
    public GameObject AfterBattle;


    [SerializeField] public List<GameObject> EnemiesToSpawn;
    [SerializeField] private Vector3Int spawnPosition;

    public List<UnitScript> Enemies = new();

    private int minEnemies = 1;
    private int maxEnemies = 5;

    public static Queue<UnitScript> TimeLineQueue = new();

    [SerializeField] TimeLinePortrete[] mini;

    public Vector3 DiceShowPosition;
    public Vector3 DiceScale;

    public ActionButtons FirstButton;
    public ActionButtons SecondButton;

    public bool inBattle = false;

    public int turn = 0;

    public UnitWindowScript PlayerUnitWindow;
    public UnitWindowScript EnemyUnitWindow;

    public List<Enemy> enemyList;

    
    public override void OpenInventory()
    {
        if (!inBattle) base.OpenInventory();
    }
    public void CloseWindow()
    {
        UnitWindow.SetActive(false);
        Preparation.SetActive(true);

    }
    public void StartBattle()
    {
        Preparation.SetActive(false);
        InBattle.SetActive(true);
        foreach (UnitScript unit in GameManager.Instance.PlayersUnits)
        {
            unit.DragAndDrop.isDraggable = false;
        }
        EnemySpawn();
        FillTimeline();
        inBattle = true;
    }
    public void FillTimeline()
    {
        turn++;
        BattleLogManager.StartTurn(turn);

        foreach (UnitScript unit in GameManager.Instance.Units)
        {
            unit.Turn();
            unit.RollInitiative();
        }
        static int CompareByInitiative(UnitScript x, UnitScript y)
        {
            if (x.UnitStats.Initiative > y.UnitStats.Initiative) return -1;
            else if (x.UnitStats.Initiative == y.UnitStats.Initiative) return 0;
            else return 1;
        }

        GameManager.Instance.Units.Sort(CompareByInitiative);

        TimeLineQueue = new(GameManager.Instance.Units);

        UpdateMini();
        StartTurn();



    }

    private void StartTurn()
    {

        UnitScript activeUnit = TimeLineQueue.Peek();

        try
        {
            bool enemy = false;
            bool friend = false;
            foreach (UnitScript unit in TimeLineQueue)
            {
                if (unit.IsEnemy && !enemy) { unit.TurnTriangle.SetActive(true); enemy = true; continue; }
                if (!unit.IsEnemy && !friend) { unit.TurnTriangle.SetActive(true); friend = true; continue; }
                if (enemy && friend) break;
            }

        }
        catch { }
        if (activeUnit.UnitStats.Health <= 0) { EndTurn(); return; }
        int index;
        if (activeUnit.activeEffects.FindIndex(EffectType.Stun, out index))
        {
            if (activeUnit.activeEffects[index].InitiationTurn == turn)
            {
                BattleLogManager.StunLog(activeUnit);
                EndTurn();
                return;
            }
        }
        if (activeUnit.IsEnemy)
        {
            EnemyTurn(activeUnit);
        }
        else
        {
            PlayerTurn(activeUnit);
        }
    }
    private void EnemyTurn(UnitScript enemy)
    {

        var a = enemy.UnitsDice.GetComponent<DiceControl>();
        var ability = a.Faces[Random.Range(0, a.Faces.Length)].Face;
        switch (ability.faceDefenition.Stats.Target)
        {
            case TargetType.Self:
                InstanceFace.ActivateAbility(ability, enemy);
                break;

            case TargetType.Enemy:
                if (GameManager.Instance.PlayersUnits.Count == 0) break;
                InstanceFace.ActivateAbility(ability, enemy, GameManager.Instance.PlayersUnits[Random.Range(0, GameManager.Instance.PlayersUnits.Count)]);
                break;

            case TargetType.Friend:
                InstanceFace.ActivateAbility(ability, enemy, Enemies[Random.Range(0, Enemies.Count)]);
                break;
        }
    }

    private void PlayerTurn(UnitScript Unit)
    {
        Unit.UnitsDice.SetActive(true);
        Unit.UnitsDice.transform.position = DiceShowPosition;
        Unit.UnitsDice.transform.localScale = DiceScale;
        var a = Unit.UnitsDice.GetComponent<DiceControl>();
        a.body.isKinematic = false;
    }


    public void ThrowTheDice(DiceControl dice)
    {
        int first = Random.Range(0, dice.Faces.Length);
        int second = Random.Range(0, dice.Faces.Length);
        while (second == first)
        {
            second = Random.Range(0, dice.Faces.Length);
        }
        FirstButton.gameObject.SetActive(true);
        FirstButton.face = dice.Faces[first].Face;
        SecondButton.gameObject.SetActive(true);
        SecondButton.face = dice.Faces[second].Face;
        dice.transform.localPosition = new Vector3(0, -1, 0);
        dice.gameObject.SetActive(false);
    }
    public void UpdateMini()
    {
        var a = TimeLineQueue.ToArray();
        for (int i = 0; i < mini.Length; i++)
        {
            if (i >= a.Length)
            {
                mini[i].gameObject.SetActive(false);
                mini[i].unit = null;
                continue;
            }
            mini[i].gameObject.SetActive(true);
            mini[i].image.sprite = a[i].EnemyScriptableObjects.Miniature;
            mini[i].unit = a[i];
            mini[i].image.gameObject.transform.eulerAngles = new Vector3(0, (a[i].IsEnemy ? 0 : 180), 0);
            mini[i].PortreteFrame.color = a[i].IsEnemy ?Color.red:Color.green;
        }
    }

    public void EndTurn()
    {

        try
        {
            bool enemy = false;
            bool friend = false;
            foreach (UnitScript unit in TimeLineQueue)
            {
                if (unit.IsEnemy && !enemy) { unit.TurnTriangle.SetActive(false); enemy = true; continue; }
                if (!unit.IsEnemy && !friend) { unit.TurnTriangle.SetActive(false); friend = true; continue; }
                if (enemy && friend) break;
            }

        }
        catch { }
        TimeLineQueue.Dequeue();
        if (TimeLineQueue.Count == 0)
        {

            FillTimeline();
            return;
        }
        UpdateMini();
        StartTurn();

    }
    void EnemySpawn()
    {
        int numEnemies = GameManager.Instance.NumberOfVictories+1;

        for (int i = 0; i < numEnemies; i++)
        {
            bool validPosition = false;
            while (!validPosition)
            {
                var go = GridBuilder.Instance.EnemiesGrid[Random.Range(0, GridBuilder.Instance.EnemiesGrid.Count)];
                if (go.transform.GetChild(0).childCount > 0) continue;
                bool tooClose = false;
                foreach (UnitScript enemy in Enemies)
                {
                    if (Vector3Int.Distance(GridBuilder.Instance.grid.WorldToCell(go.transform.position), GridBuilder.Instance.grid.WorldToCell(enemy.gameObject.transform.parent.parent.position)) < 2) tooClose = true;
                }
                if (tooClose) continue;
                validPosition = true;
                var en = Instantiate(EnemiesToSpawn[Random.Range(0, EnemiesToSpawn.Count)], go.transform.GetChild(0));

                en.transform.localPosition = en.transform.parent.GetComponent<UnitPlacable>().UnitPosition;
                GameManager.Instance.Units.Add(en.GetComponent<UnitScript>());
                enemyList.Add(en.GetComponent<UnitScript>().EnemyScriptableObjects);


            }

        }


    }

    static public void RemoveDeadUnit(UnitScript unit)
    {
        if (((BattleSceneManager)Instance).Enemies.Count == 0)
        {
            ((BattleSceneManager)Instance).InBattle.SetActive(false);
            ((BattleSceneManager)Instance).AfterBattle.SetActive(true);
            GameManager.Instance.NumberOfVictories++;
            return;
        }


        if (!TimeLineQueue.Contains(unit)) return;
        for (int i = 0; i < TimeLineQueue.Count; i++)
        {
            var u = TimeLineQueue.Dequeue();
            if (u == unit) continue;
            TimeLineQueue.Enqueue(u);
        }
        ((BattleSceneManager)Instance).UpdateMini();

    }
}
