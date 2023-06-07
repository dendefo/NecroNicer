using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "New Face", menuName = "Necrodicer/Face")]
public class Face : ScriptableObject
{
    public FaceStats Stats;
    public Material material;
    public Texture Frame;
    public bool Offensive;
    public bool Defensive;
    public bool Buff;
    public bool Debuff;

    /// <summary>
    /// Struct that describes stats of each ability
    /// </summary>
    [System.Serializable]
    public struct FaceStats
    {
        public TierTypes Tier;
        public DiceTypes ParentDice;
        public string Name;

        [TextArea] public string Description;
        [TextArea] public string Bonuses;

        public int Value;

        [Min(-1)] public int MaxUses;
        [Min(-1)] public int DustOnDestroy;
        [Min(-1)] public int DustCost;
        [Min(-1)] public int BloodCost;

        public AbilityType Ability;
        public EffectType[] EffectToReferenceInDescription;
        public EffectType[] EffectsToReferenceInBonuses;
        public DamageTypes DamageType;
        public TargetType Target;

        public static bool operator ==(FaceStats left, FaceStats right)
        {
            return (left.Ability == right.Ability && left.ParentDice == right.ParentDice && left.Tier == right.Tier);
        }
        public static bool operator !=(FaceStats left, FaceStats right)
        {
            return left == right;
        }

        public enum AbilityType
        {
            BasicMelleeAttack = 1,
            DefensiveStance = 2,
            SavageSmash = 3,
            SoulSiphon = 4,
            Carnage = 5,
            EtherealShackles = 6,
            Rage = 7,
            SoulBarrier = 8,
            SoulBlast = 9,

        }

        public enum DiceTypes
        {
            D6,
            D8,
            D10,
            D12,
            D20
        }

        public enum TierTypes
        {
            Corrupted,
            Degraded,
            Balanced,
            Cleansed,
            Pure
        }

        public enum TargetType
        {
            Self,
            Enemy,
            Friend
        }

        public enum DamageTypes
        {
            Physical,
            Magical
        }

    }

#if UNITY_EDITOR
    [CustomPreview(typeof(Face))]
    public class FacePreview : ObjectPreview
    {
        public override void OnPreviewGUI(Rect r, GUIStyle background)
        {
            GUI.DrawTexture(r, ((Face)target).material.mainTexture, ScaleMode.ScaleToFit);
        }
        public override bool HasPreviewGUI()
        {
            return true;
        }

    }
#endif
}