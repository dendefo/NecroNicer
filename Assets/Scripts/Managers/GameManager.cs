using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.EventSystems;

public class GameManager : MonoBehaviour
{
    [Header("Managers")]
    [Space]
    public MySceneManager SceneManager;
    public InventoryManager inventoryManager;
    public TutorialManager TutorialManager;

    [Header("UI Objects")]
    [Space]
    public Canvas InventoryCanvas;
    public GameObject InventoryWindow;
    public GameObject DicePanel;
    public GameObject DiceTabs;

    public GameObject DicePanelParent;
    public static GameManager Instance;



    [Header("Other")]
    [Space]
    public Library MainLibrary;
    public EffectLibrary EffectLibraby;
    public int BattleSize = 1300;
    public int InventorySize = 1700;
    public int ChosenSize = 7000;
    public int NumberOfVictories;

    public static GameObject[] Dices = new GameObject[5];
    public GameObject[] Spawners;
    public GameObject[] Panels;
    public static Vector3[] DicePositions = { new Vector3(-90f, -28f, -17.9000092f) , new Vector3(-45f, -28f, -17.5f), new Vector3(0f, -28f, -20.9500122f), new Vector3(45f, -28f, -20.7999954f),
    new Vector3(90.3000183f, -28f, -21)};

    public Transform DicesSpawPoint;
    public GameObject[] DicePrefabs;

    public List<UnitScript> Units = new();
    public List<UnitScript> PlayersUnits = new();

    public GameObject prefab_Unit;



    public static int SelectedFace = 0;
    static int currentDice = 0;

    public Camera camera;
    void Start()
    {
        Instance = this;

        for (int i = 0; i < 5; i++)
        {
            Dices[i] = Instantiate(DicePrefabs[i], Spawners[i].transform);
            Dices[i].transform.localScale = Vector3.one * BattleSize;
            Dices[i].GetComponent<Rigidbody>().isKinematic = true;
        }

        DontDestroyOnLoad(gameObject);
        UnityEngine.SceneManagement.SceneManager.LoadScene(1);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            MySceneManager.Instance.OpenInventory();
        }
    }

    public void OpenInventory()
    {
        TooltipSystem.HideToolTip();
        if (Dices[currentDice].GetComponent<DiceControl>().Unit == null)
        {
            Dices[currentDice].transform.localPosition = Vector3.zero;
            Dices[currentDice].GetComponent<Rigidbody>().isKinematic = true;
            Dices[currentDice].transform.localScale = Vector3.one * InventorySize;
            Dices[currentDice].transform.rotation = Quaternion.EulerAngles(0, 0, 0);
        }

        InventoryWindow.gameObject.SetActive(!InventoryWindow.gameObject.active);
        DicePanel.SetActive(!DicePanel.active);
        if (!InventoryWindow.gameObject.active)
        {
            int i = 0;
            foreach (GameObject Dice in Dices)
            {
                if (Dice.GetComponent<DiceControl>().Unit != null) { i++; continue; }
                Dice.transform.SetParent(DicePanel.transform.GetChild(i));
                Dice.transform.localPosition = Vector3.zero;
                Dice.transform.localScale = Vector3.one * BattleSize;
                Dice.GetComponent<DiceControl>().Dragable = true;
                i++;
            }
        }
        else
        {
            int i = 0;
            foreach (GameObject Dice in Dices)
            {
                if (Dice.GetComponent<DiceControl>().Unit != null) { i++; continue; }
                Dice.transform.parent = DiceTabs.transform.GetChild(i);
                Dice.transform.localPosition = Vector3.zero;
                Dice.transform.localScale = Vector3.one * InventorySize;
                Dice.GetComponent<DiceControl>().Dragable = false;
                i++;
            }
        }
    }

    public void ChooseDice(int number)
    {
        if (!InventoryWindow.gameObject.active) return;
        if (number < 0 || number >= Dices.Length) return;
        if (Dices[number].GetComponent<DiceControl>().Unit == null)
        {
            if (Dices[currentDice].GetComponent<DiceControl>().Unit == null)
            {
                Dices[currentDice].transform.localPosition = Vector3.zero;
                Dices[currentDice].GetComponent<Rigidbody>().isKinematic = true;
                Dices[currentDice].transform.localScale = Vector3.one * InventorySize;
                Dices[currentDice].transform.rotation = Quaternion.EulerAngles(0, 0, 0);
            }
            currentDice = number;
            Dices[currentDice].transform.position = DicesSpawPoint.position;
            Dices[currentDice].GetComponent<Rigidbody>().isKinematic = false;
            Dices[currentDice].transform.localScale = Vector3.one * ChosenSize;
        }

    }
}