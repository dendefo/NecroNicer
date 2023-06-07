using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Effect Library", menuName = "Necrodicer/EffectLibrary")]
public class EffectLibrary : ScriptableObject
{
    public Effect[] Effects;

    public Effect GiveEffect(EffectType type)
    {
        return Effects[(int)type - 1];
    }
}
