using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static Face.FaceStats;

public class UnitScript : MonoBehaviour
{

    [Header("Components")]
    public UnitsDragDrop DragAndDrop;
    public SpriteRenderer spriteRend;
    public Animator UnitAnimator;
    public GameObject Rig;

    [Header("UI")]
    public GameObject TurnTriangle;
    public GameObject HealthBar;
    [SerializeField] TMPro.TMP_Text HealthText;
    [SerializeField] UnityEngine.UI.Image HealthImage;
    [SerializeField] DamageNuber prefab;

    [Header("Battle Info")]
    public GameObject UnitsDice = null;
    public bool IsEnemy;
    [SerializeField] private Stats _UnitStats;

    public ActiveEffects activeEffects = new();
    public Stats UnitStats
    {
        get { return _UnitStats; }
        set
        {
            _UnitStats = value;
            //promp.content = _UnitStats.Health.ToString();
            HealthImage.fillAmount = (float)_UnitStats.Health / 20.0f;
            HealthText.text = $"{_UnitStats.Health}/20";
            if (value.Health <= 0)
            {
                Die();
            }
            else if (value.Health > EnemyScriptableObjects.BasicStats.Health)
            {
                _UnitStats.Health = EnemyScriptableObjects.BasicStats.Health;

            }
            if (MySceneManager.Instance == null) return;
            if (!((BattleSceneManager)MySceneManager.Instance).InBattle) return;
            if (!IsEnemy)
            {
                if (((BattleSceneManager)MySceneManager.Instance).PlayerUnitWindow.UnitReference == this)
                {
                    ((BattleSceneManager)MySceneManager.Instance).PlayerUnitWindow.UnitReference = this;
                };
            }
            else
            {
                if (((BattleSceneManager)MySceneManager.Instance).EnemyUnitWindow.UnitReference == this)
                {
                    ((BattleSceneManager)MySceneManager.Instance).EnemyUnitWindow.UnitReference = this;
                }
            }

        }
    }


    public static UnitScript Chosen;

    [SerializeField] Material OutlineOn;
    [SerializeField] Material OutlineOff;

    public List<Outline> Outlines;

    [Header("Defenition")]
    public Enemy EnemyScriptableObjects;

    void OnEnable()
    {

        if (IsEnemy)
        {
            UnitStats = EnemyScriptableObjects.BasicStats;
            ((BattleSceneManager)BattleSceneManager.Instance).Enemies.Add(this);
            UnitsDice = Instantiate(GameManager.Instance.DicePrefabs[0], transform);
            UnitsDice.GetComponent<DiceControl>().Generate(EnemyScriptableObjects.enemyFaceChance);
            UnitsDice.SetActive(false);
            //promp.header = EnemyScriptableObjects.Name;
            //promp.content = EnemyScriptableObjects.Description;
            return;
        }
        else GameManager.Instance.PlayersUnits.Add(this);
        GameManager.Instance.Units.Add(this);
        UnitStats = UnitStats;

    }

    public void AfterAnimation()
    {
        InstanceFace.AfterAnimation();
        ((BattleSceneManager)BattleSceneManager.Instance).EndTurn();
    }
    public void Turn()
    {

        UnitStats = UnitStats.ZeroStats(EnemyScriptableObjects.BasicStats);
        activeEffects.Turn(((BattleSceneManager)BattleSceneManager.Instance).turn);
        int index;
        if (activeEffects.FindIndex(EffectType.Fortified, out index))
        {
            BattleLogManager.DefendLog(this, RaiseDefence(activeEffects.effects[index].Value), effectOnUser: EffectType.Fortified);
        }
        if (activeEffects.FindIndex(EffectType.SoulCharged, out index))
        {
            BattleLogManager.DefendLog(this, RaiseMagicDefence(activeEffects.effects[index].Value), effectOnUser: EffectType.SoulCharged);
        }

    }
    public void TurnOutline(bool On)
    {
        if (GameManager.Instance.InventoryWindow.active) return;

        if (On)
        {
            foreach (Outline ou in Outlines)
            {
                try { ou.enabled = true; }
                catch { }
            }
        }
        else
        {
            foreach (Outline ou in Outlines)
            {
                try { ou.enabled = false; }
                catch { }
            }
        }
    }
    public int RollInitiative()
    {
        UnitStats = UnitStats.SetInitiative(Random.Range(0, 100) + EnemyScriptableObjects.BasicStats.Initiative);
        return UnitStats.Initiative;
    }

    //Battle Functions

    public AttackOutcome Attack(Face Ability, UnitScript Target)
    {
        switch (Ability.Stats.DamageType)
        {
            case DamageTypes.Physical:
                return PhysicalAttack(Ability.Stats.Value, Target);
            case DamageTypes.Magical:
                return MagicalAttack(Ability.Stats.Value, Target);
            default:
                return new();
        }
    }
    private AttackOutcome PhysicalAttack(int Damage, UnitScript Target)
    {
        int crit = CalculateCrit(Damage);
        Damage += crit;
        Damage += UnitStats.Attack;
        Damage = (int)(Damage * EffectInstance.CalculatePhysicalDamage(activeEffects.effects, ((BattleSceneManager)BattleSceneManager.Instance).turn));
        var ret = (Target.TakePhysicalDamage(Damage));
        ret.isCrit = (crit != 0);
        return ret;
    }
    private AttackOutcome MagicalAttack(int MagicDamage, UnitScript Target)
    {
        int crit = CalculateCrit(MagicDamage);
        MagicDamage += crit;
        MagicDamage += UnitStats.Magic;
        MagicDamage = (int)(MagicDamage * EffectInstance.CalculateMagicalDamage(activeEffects.effects, ((BattleSceneManager)BattleSceneManager.Instance).turn));
        var ret = (Target.TakeMagicalDamage(MagicDamage));
        ret.isCrit = (crit != 0);
        return ret;
    }
    private AttackOutcome TakePhysicalDamage(int Damage)
    {

        if (Damage > UnitStats.Defence)
        {
            Damage -= UnitStats.Defence;
            int defLost = UnitStats.Defence;
            if (UnitStats.Defence != 0) BreakPhysicalDefence(-UnitStats.Defence);

            defLost -= UnitStats.Defence;
            Damage = (int)(Damage * EffectInstance.CalculatePhysicalDefence(activeEffects.effects, ((BattleSceneManager)BattleSceneManager.Instance).turn));

            int damageDealt = TakePureDamage(Damage);

            UnitAnimator.SetTrigger("Hit");
            CreateDamageText(damageDealt.ToString(), Color.red);

            return new AttackOutcome(defLost == 0 ? AttackOutcomeType.DealtDamage : AttackOutcomeType.PartlyBlocked, damageDealt, defLost, DamageTypes.Physical);
        }
        else
        {
            CreateDamageText("Blocked", Color.red);

            BreakPhysicalDefence(-Damage);

            return new AttackOutcome(AttackOutcomeType.Blocked, 0, Damage, DamageTypes.Physical);
        }
    }
    private AttackOutcome TakeMagicalDamage(int MagicDamage)
    {
        if (MagicDamage > UnitStats.MagicDefence)
        {
            MagicDamage -= UnitStats.MagicDefence;
            int defLost = UnitStats.MagicDefence;
            if (UnitStats.MagicDefence != 0) BreakMagicDefence(-UnitStats.Defence);

            defLost -= UnitStats.MagicDefence;
            MagicDamage = (int)(MagicDamage * EffectInstance.CalculateMagicalDefence(activeEffects.effects, ((BattleSceneManager)BattleSceneManager.Instance).turn));

            int damageDealt = TakePureDamage(MagicDamage);

            UnitAnimator.SetTrigger("Hit");
            CreateDamageText(damageDealt.ToString(), Color.red);

            return new AttackOutcome(defLost == 0 ? AttackOutcomeType.DealtDamage : AttackOutcomeType.PartlyBlocked, damageDealt, defLost, DamageTypes.Magical);

        }
        else
        {
            CreateDamageText("Blocked", Color.red);
            BreakPhysicalDefence(-MagicDamage);
            return new AttackOutcome(AttackOutcomeType.Blocked, 0, MagicDamage, DamageTypes.Magical);
        }
    }

    public int CalculateCrit(int Damage)
    {
        if (Random.Range(0, 1.0f) > UnitStats.CritChance) return 0;

        return (int)(Damage * UnitStats.CritDamage);
    }
    public int TakePureDamage(int Damage)
    {
        int healthBefore = UnitStats.Health;
        UnitStats = UnitStats.GetDamage(Damage);
        return healthBefore - UnitStats.Health;
    }
    public int RaiseDefence(int Defence)
    {
        int defBefore = UnitStats.Defence;
        UnitStats = UnitStats.AddDefence(Defence);
        return UnitStats.Defence - defBefore;
    }
    public int RaiseMagicDefence(int Defence)
    {
        int defBefore = UnitStats.MagicDefence;
        UnitStats = UnitStats.AddMagicDefence(Defence);
        return UnitStats.MagicDefence - defBefore;
    }
    public void BreakPhysicalDefence(int Amount)
    {
        UnitStats = UnitStats.AddDefence(Amount);
        Debug.Log($"Removed Defence for {Amount}", gameObject);
    }
    public void BreakMagicDefence(int Amount)
    {
        UnitStats = UnitStats.AddMagicDefence(Amount);
        Debug.Log($"Removed Magic Defence for {Amount}", gameObject);
    }
    public int Heal(int Health)
    {
        int beforeHeal = UnitStats.Health;
        UnitStats = UnitStats.AddHealth(Health);
        return UnitStats.Health - beforeHeal;
    }
    private void Die()
    {
        if (IsEnemy) ((BattleSceneManager)BattleSceneManager.Instance).Enemies.Remove(this);
        else GameManager.Instance.PlayersUnits.Remove(this);
        GameManager.Instance.Units.Remove(this);
        BattleSceneManager.RemoveDeadUnit(this);
        Destroy(gameObject);
        Debug.Log("Unit Died");
    }


    private void CreateDamageText(string toPrint, Color color)
    {
        var text = Instantiate(prefab, HealthBar.transform);

        text.m_TextMeshPro.text = toPrint;
        text.m_TextMeshPro.color = color;
        if (!IsEnemy)
        {
            text.transform.localEulerAngles =  new Vector3(0, 180, 0);
        }
    }
    public void OnMouseEnter()
    {
        Chosen = this;
        TurnOutline(true);
    }
    private void OnMouseOver()
    {

        if (Input.GetMouseButtonDown(1) && ((BattleSceneManager)MySceneManager.Instance).inBattle)
        {
            if (!IsEnemy)
            {
                ((BattleSceneManager)MySceneManager.Instance).PlayerUnitWindow.UnitReference = this;
            }
            else ((BattleSceneManager)MySceneManager.Instance).EnemyUnitWindow.UnitReference = this;
        }

    }
    public void OnMouseExit()
    {
        if (Chosen == this) Chosen = null;
        TurnOutline(false);
    }
}

[System.Serializable]
public struct Stats
{
    [Min(0)] public int Health;
    [Range(0, 100)] public int Initiative;
    public int Attack;
    public int Range;
    public int Magic;
    public int Defence;
    public int MagicDefence;

    [Min(0)] public float CritDamage;
    [Range(0.0f, 1.0f)] public float CritChance;
    public ElementPower Elements;

    public Stats SetInitiative(int initiative)
    {
        Initiative = initiative;
        return this;
    }

    public Stats ZeroStats(Stats baseStats)
    {
        baseStats.Health = this.Health;
        return baseStats;
    }
    public Stats GetDamage(int damage)
    {
        Health -= damage;
        if (Health < 0) Health = 0;
        return this;
    }

    public Stats AddDefence(int defence)
    {
        Defence += defence;
        if (Defence < 0) Defence = 0;
        return this;
    }
    public Stats AddMagicDefence(int defence)
    {
        MagicDefence += defence;
        if (MagicDefence < 0) MagicDefence = 0;
        return this;
    }
    public Stats AddHealth(int heal)
    {
        Health += heal;
        return this;
    }
    public Stats(int health = 0, int initiative = 0, int attack = 0, int range = 0, int magic = 0, int defence = 0, int magicDefence = 0, float dodgeChance = 0, float critChance = 0, ElementPower elements = new())
    {
        Health = health;
        Initiative = initiative;
        Attack = attack;
        Range = range;
        Magic = magic;
        Defence = defence;
        MagicDefence = magicDefence;
        CritDamage = dodgeChance;
        CritChance = critChance;
        Elements = elements;
    }
    static public bool operator ==(Stats left, Stats right)
    {
        return (left.Health == right.Health && left.Initiative == right.Initiative && left.Attack == right.Attack && left.Range == right.Range && left.Magic == right.Magic &&
            left.Defence == right.Defence && left.MagicDefence == right.MagicDefence && left.CritDamage == right.CritDamage && left.CritChance == right.CritChance && left.Elements == right.Elements);
    }
    static public bool operator !=(Stats left, Stats right)
    {
        return !(left == right);
    }

}

[System.Serializable]
public struct ElementPower
{
    public int Fire;
    public int Water;
    public int Air;
    public int Earth;

    public ElementPower(int fire = 0, int water = 0, int air = 0, int earth = 0)
    {
        Fire = fire;
        Water = water;
        Earth = earth;
        Air = air;
    }
    static public bool operator ==(ElementPower left, ElementPower right)
    {
        return (left.Fire == right.Fire && left.Water == right.Water && left.Air == right.Air && left.Earth == right.Earth);
    }
    static public bool operator !=(ElementPower left, ElementPower right)
    {
        return !(left == right);
    }
}

[System.Serializable]
public class ActiveEffects
{
    [SerializeField] public List<EffectInstance> effects;

    public EffectInstance this[int index]
    {
        get { return effects[index]; }
    }
    public ActiveEffects()
    {
        effects = new();
    }
    public bool AddEffect(EffectType efect, int turns, int currentTurn, int value = 0)
    {
        int index = effects.FindIndex(ef => ef.Type == efect);
        if (index < 0) { effects.Add(new(efect, currentTurn, turns, value)); }
        else effects[index].AddLength(turns);
        return true;
    }
    public void Turn(int currentTurn)
    {
        for (int i = 0; i < effects.Count; i++)
        {
            if (effects[i].InitiationTurn + effects[i].Length < currentTurn)
            {
                effects.RemoveAt(i);
            }
        }
    }

    public bool FindIndex(EffectType efect, out int index)
    {
        index = effects.FindIndex(ef => ef.Type == efect);
        return index >= 0;
    }

}

public struct AttackOutcome
{
    public AttackOutcomeType outcomeType;
    public int DamageDealt;
    public int DefenceLost;
    public DamageTypes DamageType;
    public bool isCrit;
    public AttackOutcome(AttackOutcomeType type, int damage, int defence, DamageTypes damageType, bool crit = false)
    {
        outcomeType = type;
        DamageDealt = damage;
        DefenceLost = defence;
        DamageType = damageType;
        isCrit = crit;
    }
}
public enum AttackOutcomeType
{
    Blocked,
    PartlyBlocked,
    DealtDamage
}