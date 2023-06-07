using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RewardWindowScript : MonoBehaviour
{
    [SerializeField] RewardDiceScript rewardDice;
    [SerializeField] FaceWindow RewardWindows;
    [SerializeField] Face[] Rewards;
    [SerializeField] RewardType reward;
    private void Start()
    {
        Rewards = RewardManager.DeclareWhichRewardSystemType(reward, GameManager.Instance.NumberOfVictories, ((BattleSceneManager)MySceneManager.Instance).turn, ((BattleSceneManager)MySceneManager.Instance).enemyList);
        var inst = new InstanceFace();
        for (int i = 0; i < Rewards.Length; i++)
        {
            inst.faceDefenition = Rewards[i];
            inst.Uses = Rewards[i].Stats.MaxUses;
            rewardDice.controlDice.Faces[i].Face = inst;
        }
    }

    public void ChooseReward()
    {
        GameManager.Instance.inventoryManager.InventoryAdd(rewardDice.controlDice.Faces[rewardDice.CurrentSide].Face);
        foreach (UnitScript unit in GameManager.Instance.PlayersUnits)
        {
            for (int i = 0;i< GameManager.Instance.DiceTabs.transform.childCount;i++)
            {
                if (GameManager.Instance.DiceTabs.transform.GetChild(i).childCount == 0)
                {
                    unit.UnitsDice.transform.parent = GameManager.Instance.DiceTabs.transform;
                    unit.UnitsDice.transform.localPosition = Vector3.zero;
                    unit.UnitsDice.transform.localScale = Vector3.one * GameManager.Instance.InventorySize;
                    unit.UnitsDice.SetActive(true);
                    unit.UnitsDice.GetComponent<DiceControl>().body.isKinematic = true;
                    unit.UnitsDice.GetComponent<DiceControl>().Dragable = false;
                    unit.UnitsDice.GetComponent<DiceControl>().Unit = null;
                    break;
                } 
            }
        }
        GameManager.Instance.DicePanelParent.SetActive(true);
        GameManager.Instance.InventoryCanvas.gameObject.SetActive(true);
        GameManager.Instance.OpenInventory();
        GameManager.Instance.Units.Clear();
        GameManager.Instance.PlayersUnits.Clear();
        SceneManager.LoadScene("Battle Scene");

    }
    
}
