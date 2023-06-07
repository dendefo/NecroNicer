using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Face.FaceStats;

[CreateAssetMenu(fileName = "Main Library", menuName = "Necrodicer/MainLibrary")]
public class Library : ScriptableObject
{
    public FaceLibrary[] Libraries;
    
    public Face Upgrade(Face ToUpgrade)
    {
        return Libraries[(int)ToUpgrade.Stats.Ability - 1].Upgrade(ToUpgrade);
    }
    public Face Downgrade(Face ToDowngrade)
    {
        return Libraries[(int)ToDowngrade.Stats.Ability - 1].DownGrade(ToDowngrade);
    }

    public Face GetFace(AbilityType ability, TierTypes tier)
    {
        try { return Libraries[(int)ability - 1].ScriptObjects[(int)tier]; }
        catch { return Libraries[0].ScriptObjects[2]; }
    }
}
