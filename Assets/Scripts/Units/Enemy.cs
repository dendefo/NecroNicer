using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Face.FaceStats;

[CreateAssetMenu(fileName = "New Enemy", menuName = "Necrodicer/Enemy")]
public class Enemy : ScriptableObject
{
    public string Name;
    [Multiline] public string Description;
    public FaceChance[] enemyFaceChance;
    public Stats BasicStats;
    public GameObject RiggedPrefab;
    public Sprite Miniature;
    public EnemyTier enemyTier;
    
    private void OnValidate()
    {
        foreach (FaceChance cha in enemyFaceChance)
        {
            double summ = 0;
            for (int i = 0; i < cha.Chances.Length; i++)
            {
                summ += cha.Chances[i].Chances;
            }
            if (summ != 1)
            {
                for (int i = 0; i < cha.Chances.Length; i++)
                {
                    cha.Chances[i].Chances /= summ;
                }
            }
        }
    }

}
[System.Serializable]
public class FaceChance
{
    public AbilityType abilityTypeFace;
    public int Amount;




    public FaceAndChance[] Chances;
}
[System.Serializable]
public class FaceAndChance
{
    public TierTypes ChanceTierTypes;
    [Range(0, 1)] public double Chances;
}
public enum EnemyTier
{
    Normal = 10,
    Champion = 15,
    Elite = 25,
    MiniBoss = 35,
    Boss = 50
}