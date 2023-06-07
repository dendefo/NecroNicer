using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "New Effect", menuName = "Necrodicer/Effect")]
public class Effect : ScriptableObject
{
    public EffectType EffectType;
    public string Name;
    [Multiline] public string Description;
    public Color TextColor;
    public TMPro.TMP_SpriteAsset Icons;

    public override string ToString()
    {
        return $"<sprite={(int)EffectType-1}>";
    }

#if UNITY_EDITOR
    [CustomPreview(typeof(Effect))]
    public class EffectPreview : ObjectPreview
    {
        public override void OnPreviewGUI(Rect r, GUIStyle background)
        {
            int id = (int)((Effect)target).EffectType-1;
            var temp = ((Effect)target).Icons.spriteGlyphTable[(int)((Effect)target).Icons.spriteCharacterTable[id].glyphIndex].sprite;
            GUI.DrawTextureWithTexCoords(r, temp.texture, new Rect(temp.rect.x / temp.texture.width, temp.rect.y / temp.texture.height, temp.rect.width/ temp.texture.width, temp.rect.height / temp.texture.height),true);

        }
        public override bool HasPreviewGUI()
        {
            return true;
        }

    }
#endif
}

[System.Serializable]
public struct EffectInstance
{
    public EffectType Type;
    public int InitiationTurn;
    public int Length;
    public int Value;

    public EffectInstance(EffectType type, int start, int length, int value = 0)
    {
        Type = type;
        InitiationTurn = start;
        Length = length;
        Value = value;
    }
    public void AddLength(int length)
    {
        this.Length += length;
    }

    static public float CalculatePhysicalDamage(List<EffectInstance> Attacker, int CurrentTurn)
    {
        float end = 1;
        foreach (var ef in Attacker)
        {
            switch (ef.Type)
            {
                case EffectType.Rage:
                    if (ef.InitiationTurn == CurrentTurn) break;
                    end += 0.5f;
                    break;
                case EffectType.Carnage:
                    if (ef.InitiationTurn == CurrentTurn) break;
                    end += 1;
                    break;
            }
        }
        return end;
    }
    static public float CalculateMagicalDamage(List<EffectInstance> Attacker, int CurrentTurn)
    {
        float final = 1;
        return final;
    }
    static public float CalculateMagicalDefence(List<EffectInstance> Defender, int CurrentTurn)
    {
        float final = 1;
        return final;
    }
    static public float CalculatePhysicalDefence(List<EffectInstance> Defender, int CurrentTurn)
    {
        float end = 1;
        foreach (var ef in Defender)
        {
            switch (ef.Type)
            {
                case EffectType.Vulnerable:
                    if (ef.InitiationTurn == CurrentTurn) end += 0.5f;
                    break;
                case EffectType.Exhausted:
                    if (ef.InitiationTurn == CurrentTurn) break;
                    end += 0.5f;
                    break;
            }
        }
        return end;
    }
}

public enum EffectType
{
    None,
    Backfire,
    Vulnerable,
    Fortified,
    Exhausted,
    Rage,
    Carnage,
    SoulDischarge,
    SoulCharged,
    Stun,
}