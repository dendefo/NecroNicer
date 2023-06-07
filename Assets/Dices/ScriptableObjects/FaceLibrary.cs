using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Library", menuName = "Necrodicer/Library")]
public class FaceLibrary : ScriptableObject
{
    public Face[] ScriptObjects;
    public Face Upgrade(Face ToUpgrade)
    {
        try { return ScriptObjects[(int)ToUpgrade.Stats.Tier + 1]; }
        catch
        {
            return ToUpgrade;
        }
    }
    public Face DownGrade(Face ToDowngrade)
    {
        try { return ScriptObjects[(int)ToDowngrade.Stats.Tier - 1]; }
        catch { return ToDowngrade; }
    }
}
