using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class RewardManager 
{


    private static float RewardPriceIncrease = 0.01f;
    private static float RewardPriceModifier = 1;
    private static float EnemyCountRewardIncrease = 1;
    private static float EnemyCountRewardModifier = 0;
    private static float TurnRewardModifer = 1;

    public static Face[] DeclareWhichRewardSystemType(RewardType rewardType, int NumberOfVictories = 0, int Turns = 0, List<Enemy> EnemyList = null)
    {
        var Reward = new Face[3];
        Debug.Log("Reward Type: " + rewardType);
        Debug.Log("Num Of Victor: " + NumberOfVictories);
        Debug.Log("Num Of turns: " + Turns);
        Debug.Log("Num Of Enemies: " + EnemyList.Count);

        Debug.Log("The Calculation: " + CalculateEnemyAndTurnRewardModifer(EnemyList, Turns));
        switch (rewardType)
        {
            case RewardType.CombatReward:
                Reward = GenerateRewardFaces(NumberOfVictories);
                //Currently missing the logic here to work After Combat and Before Combat.
                CalculatePrecentOfFaceAfterCombat(Turns, EnemyList,Reward);
                return Reward;
            case RewardType.SolvingQuest:
                GenerateRewardForQuests();
                return Reward;
            case RewardType.Achievement:
                GenerateRewardForAchievement();
                return Reward;
            case RewardType.LoreEncounter:
                GenerateRewardForLoreEncounter();
                return Reward;
        }
        return Reward;
    }

    public static Face[] GenerateRewardFaces(int NumberOfVictories)
    {
        var ChosenThreeRandomFaces = new Face[4];

        var RandomFaceTier = Random.Range(0, 100f);
        var RandomFacePicker = Random.Range(0, 3);

        for (int i = 0; ChosenThreeRandomFaces.Length - 1 > i; i++) //When After POC - Remvoe the minus 1 and Fix the Array - to size of 3!!!!!!!!!! IMPORTENT !!!!!!! IMPORTENT !!!!!!
        {
            var randomIndexForFace = Random.Range(0, GameManager.Instance.MainLibrary.Libraries.Length);
            ChosenThreeRandomFaces[i] = GameManager.Instance.MainLibrary.Libraries[randomIndexForFace].ScriptObjects[1];
            Debug.Log(" The 3 Random Faces without any modifers" + ChosenThreeRandomFaces[i]);//Shows the 3 Random Face

        }
        ChosenThreeRandomFaces[3] = GameManager.Instance.MainLibrary.Libraries[Random.Range(0, GameManager.Instance.MainLibrary.Libraries.Length)].ScriptObjects[Random.Range(0, 5)];
        Debug.Log(CheckAmountOfBattlesWon(NumberOfVictories));
        if (CheckAmountOfBattlesWon(NumberOfVictories) > RandomFaceTier)
        {
            GameManager.Instance.MainLibrary.Upgrade(ChosenThreeRandomFaces[RandomFacePicker]);
            Debug.Log("After Trying to Upgrade With Battle Amount check : " + GameManager.Instance.MainLibrary.Upgrade(ChosenThreeRandomFaces[RandomFacePicker]));
        }

        return ChosenThreeRandomFaces;
    }
    public static Face[] CalculatePrecentOfFaceAfterCombat(int turn, List<Enemy> EnemyList, Face[] Reward)
    {
        foreach (var generatedFaces in Reward)
        {
            var randomGenerator = Random.Range(0, 100f);
            if (CalculateEnemyAndTurnRewardModifer(EnemyList,turn) > randomGenerator)
                GameManager.Instance.MainLibrary.Upgrade(generatedFaces);
            Debug.Log(generatedFaces);
        }
        return Reward;
    }
    private static float CheckAmountOfBattlesWon(int NumberOfVictories)
    {
        if (NumberOfVictories == 0)
            return 0;
        switch (NumberOfVictories)
        {

            case <= 9 when NumberOfVictories > 0:
                RewardPriceIncrease = 0.01f;
                break;
            case <= 19:
                RewardPriceIncrease = 0.011f;
                break;
            case <= 29:
                RewardPriceIncrease = 0.012f;
                break;
            case <= 50:
                RewardPriceIncrease = 0.013f;
                break;
            case > 51:
                RewardPriceModifier = 0.013f;
                break;
        }
        RewardPriceModifier = NumberOfVictories * RewardPriceIncrease * 100;

        if (NumberOfVictories > 50) return RewardPriceModifier = 65;

        return RewardPriceModifier;
    }
    private static float CalculateEnemyAndTurnRewardModifer(List<Enemy> EnemyList,int turns)
    {
        float sum = 0;
        EnemyTier HighestTier = 0;
        switch(EnemyList.Count)
        {
            case 1: EnemyCountRewardIncrease = 0.07f;
                break;
            case 2: EnemyCountRewardIncrease = 0.06f;
                break;
            case 3: EnemyCountRewardIncrease = 0.0565f;
                break; 
            case 4: EnemyCountRewardIncrease = 0.055f;
                break;
            case <= 10: EnemyCountRewardIncrease = 0.05f;
                break;
        }
        EnemyCountRewardModifier = EnemyList.Count * EnemyCountRewardIncrease * 100;

        foreach(var enemy in EnemyList)
        {
            if(enemy.enemyTier > HighestTier) HighestTier = enemy.enemyTier;
            Debug.Log("Enemy Tier: " + enemy.enemyTier);
        }
        switch (turns)
        {
            case < 6 when turns > 0: TurnRewardModifer = 9;
                break;
            case < 10: TurnRewardModifer = 8;
                break;
            case < 16: TurnRewardModifer = 7;
                break;
            case < 20: TurnRewardModifer = 6;
                break;
            case < 30: TurnRewardModifer = 5;
                break;
            case < 35: TurnRewardModifer = -5;
                break;
            case < 40: TurnRewardModifer = -10;
                break;
            case < 50: TurnRewardModifer = -15;
                break;
            case >= 50: TurnRewardModifer = -20;
                break;
        }
        return sum = ((float)HighestTier + EnemyCountRewardModifier) - TurnRewardModifer;

    }

    //These Down Here Are currentl inactive due to missing the feature in game!!!
    private static void GenerateRewardForLoreEncounter()
    {
        throw new System.NotImplementedException();
    }

    private static void GenerateRewardForAchievement()
    {
        throw new System.NotImplementedException();
    }

    private static void GenerateRewardForQuests()
    {
        throw new System.NotImplementedException();
    }
}
public enum RewardType
{
    CombatReward,
    SolvingQuest,
    Achievement,
    LoreEncounter
}