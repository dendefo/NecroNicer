using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using static Face.FaceStats;

public class BattleLogManager : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] GameObject Content;
    [SerializeField] GameObject BasicDamagePrefabRow; //Has 7 text fields
    [SerializeField] RectTransform ButtonCanvas;
    [SerializeField] public GameObject SmallBattleLogCanvas;
    [SerializeField] public GameObject BigBattleLogCanvas;
    static public BattleLogManager Instance;


    [Header("Text Prefabs")]
    public GameObject UnitTextPrefab;
    public GameObject AbilityTextPrefab;
    public GameObject NumberTextPrefab;
    public GameObject BasicTextPrefab;
    public GameObject EffectTextPrefab;

    public void Awake()
    {
        Instance = this;
        CloseBigLog();
    }
    /// <summary>
    /// Creating Message in format 
    /// "<paramref name="attacker"/> used <paramref name="ability"/> dealing <paramref name="outcome"/> damage to <paramref name="defender"/> [causing him to become <paramref name="effectOnDefender"/>.]"
    /// if effectOnAttacker is provided outputs additional message 
    /// "<paramref name="attacker"/> received [<paramref name="effectOnAttacker"/>] [and <paramref name="flatDmg"/> Damage] using <paramref name="ability"/>."
    /// </summary>
    /// <param name="attacker"> Unit that uses <paramref name="ability"/> </param>
    /// <param name="defender"> Target of the <paramref name="ability"/> </param>
    /// <param name="ability"> Ability </param>
    /// <param name="outcome"> Damage that <paramref name="effectOnDefender"/> took </param>
    /// <param name="flatDmg"> Damage that <paramref name="attacker"/> took </param>
    /// <param name="effectOnDefender"> Effect that <paramref name="defender"/> recieved </param>
    /// <param name="effectOnAttacker"> Effect that <paramref name="attacker"/> recieved </param>
    public static void AttackLog(UnitScript attacker, UnitScript defender, Face ability, AttackOutcome outcome, int flatDmg = 0, EffectType effectOnDefender = 0, EffectType effectOnAttacker = 0)
    {

        LogRow firstLog = Instantiate(Instance.BasicDamagePrefabRow, Instance.Content.transform).GetComponent<LogRow>();
        firstLog.ParentCanvas = Instance.ButtonCanvas;



        int FirstBinaryTree = 1;

        if (outcome.outcomeType == AttackOutcomeType.PartlyBlocked) FirstBinaryTree *= 4;
        else if (outcome.outcomeType == AttackOutcomeType.Blocked) FirstBinaryTree = (FirstBinaryTree * 4) - 1;

        FirstBinaryTree *= 2;
        if (effectOnDefender == 0) FirstBinaryTree -= 1;

        FirstBinaryTree *= 2;
        if (!outcome.isCrit) FirstBinaryTree -= 1;

        switch (FirstBinaryTree)
        {
            case 1: //Simple Attack
                firstLog.CreateAnyText(
                "<> attacked with [] hitting <> dealing () " + ability.Stats.DamageType.ToString() + " damage",
                new UnitScript[] { attacker, defender },
                new Face[] { ability },
                null,
                new int[] { outcome.DamageDealt });
                break;
            case 2: //simple attack With crits
                firstLog.CreateAnyText(
                "<> attacked with [] landing a critical strike on <> dealing () " + ability.Stats.DamageType.ToString() + " damage",
                    new UnitScript[] { attacker, defender },
                    new Face[] { ability },
                    null,
                    new int[] { outcome.DamageDealt });
                break;
            case 3://Simple attack with effect:
                firstLog.CreateAnyText(
                "<> attacked with [] hitting <> dealing () " + ability.Stats.DamageType.ToString() + " damage causing <> to become {} ",
                new UnitScript[] { attacker, defender, defender },
                new Face[] { ability },
                new EffectType[] { effectOnDefender },
                new int[] { outcome.DamageDealt });
                break;
            case 4://Simple attack with critical and with effect:
                firstLog.CreateAnyText("<> attacked with [] landing a critical strike on <> dealing () " + ability.Stats.DamageType.ToString() + " damage causing <> to become {} ",
                new UnitScript[] { attacker, defender },
                new Face[] { ability },
                new EffectType[] { effectOnDefender },
                new int[] { outcome.DamageDealt });
                break;
            case 9://Simple attack blocked:
                firstLog.CreateAnyText("<> attacked with [] , but  <> blocked the attack",
                    new UnitScript[] { attacker, defender },
                    new Face[] { ability },
                    null,
                    new int[] { outcome.DamageDealt });
                break;
            case 10://simple attack with critical blocked
                firstLog.CreateAnyText("<> attacked with [] , but  <> blocked the attack",
                    new UnitScript[] { attacker, defender },
                    new Face[] { ability },
                    null,
                    new int[] { outcome.DamageDealt });
                break;
            case 11://Simple attack with effect blocked
                firstLog.CreateAnyText("<> attacked with [] , but  <> blocked the attack",
                    new UnitScript[] { attacker, defender },
                    new Face[] { ability },
                    null,
                    new int[] { outcome.DamageDealt });
                break;
            case 12://Simple Attack with critical and with effect blocked
                firstLog.CreateAnyText("<> attacked with [] , but <> blocked the attack",
                     new UnitScript[] { attacker, defender },
                    new Face[] { ability },
                    null,
                    new int[] { outcome.DamageDealt });
                break;
            case 13://Simple attack Partially blocked
                firstLog.CreateAnyText("<> attacked with [] , but  <> managed to parry receiving only () " + ability.Stats.DamageType.ToString() + " damage",
                    new UnitScript[] { attacker, defender },
                    new Face[] { ability },
                    null,
                    new int[] { outcome.DamageDealt });
                break;
            case 14://Simple attack with critical partially blocked
                firstLog.CreateAnyText("<> attacked with [] landing a critical strike, but  <> managed to mitigate receiving only () " + ability.Stats.DamageType.ToString() + " damage",
                    new UnitScript[] { attacker, defender },
                    new Face[] { ability },
                    null,
                    new int[] { outcome.DamageDealt });
                break;
            case 15://Simple attack with effect partially blocked
                firstLog.CreateAnyText("<> attacked with [] , but  <> managed to parry receiving only () " + ability.Stats.DamageType.ToString() + " damage and became {} ",
                     new UnitScript[] { attacker, defender },
                    new Face[] { ability },
                    new EffectType[] { effectOnDefender },
                    new int[] { outcome.DamageDealt });
                break;
            case 16://Simple attack with critical and effect partially blocked
                firstLog.CreateAnyText("<> attacked with [] landing a critical strike, but  <> managed to mitigate receiving only () " + ability.Stats.DamageType.ToString() + " damage and became {} ",
                    new UnitScript[] { attacker, defender },
                    new Face[] { ability },
                    new EffectType[] { effectOnDefender },
                    new int[] { outcome.DamageDealt });
                break;

            default:
                firstLog.CreateAnyText($"ERROR tree summ is - {FirstBinaryTree}");
                break;
        }


        Instantiate(firstLog, Instance.SmallBattleLogCanvas.transform);

        LogRow secondLog = Instantiate(Instance.BasicDamagePrefabRow, Instance.Content.transform).GetComponent<LogRow>();
        secondLog.ParentCanvas = Instance.ButtonCanvas;

        if (effectOnAttacker != 0 && flatDmg == 0)
        {
            secondLog.CreateAnyText(
                "<> recieved {} using []",
                new UnitScript[] { attacker },
                new Face[] { ability },
                new EffectType[] { effectOnAttacker });
        }
        else if (effectOnAttacker == 0 && flatDmg != 0)
        {
            secondLog.CreateAnyText(
                "<> recieved () damage using []",
                new UnitScript[] { attacker },
                new Face[] { ability },
                new EffectType[] { },
                new int[] { flatDmg });
        }
        else if (effectOnAttacker != 0 && flatDmg != 0)
        {
            secondLog.CreateAnyText(
                "<> recieved {} and () damage using []",
                new UnitScript[] { attacker },
                new Face[] { ability },
                new EffectType[] { effectOnAttacker },
                new int[] { flatDmg });
        }
        else Destroy(secondLog.gameObject);
        Instantiate(secondLog, Instance.SmallBattleLogCanvas.transform);
    }

    public static void DefendLog(UnitScript Defender, int DefenseValue, Face ability = null, EffectType effectOnUser = 0, UnitScript Target = null, EffectType effectOnTarget = 0)
    {
        LogRow firstLog = Instantiate(Instance.BasicDamagePrefabRow, Instance.Content.transform).GetComponent<LogRow>();
        LogRow secondLog = Instantiate(Instance.BasicDamagePrefabRow, Instance.Content.transform).GetComponent<LogRow>();

        firstLog.ParentCanvas = Instance.ButtonCanvas;
        secondLog.ParentCanvas = Instance.ButtonCanvas;
        if (ability == null)
        {
            firstLog.CreateAnyText("<> increased his defence by () because of {}",
                new UnitScript[] { Defender },
                null,
                new EffectType[] { effectOnUser },
                new int[] { DefenseValue });

            secondLog.CreateAnyText("<> " +  (effectOnUser == EffectType.Fortified?"Physical":"Magical") + " resistance is now ()",
                new UnitScript[] { Defender },
                null,
                null,
                new int[] { effectOnUser == EffectType.Fortified ? Defender.UnitStats.Defence : Defender.UnitStats.MagicDefence }
                );
            Instantiate(firstLog, Instance.SmallBattleLogCanvas.transform);
            Instantiate(secondLog, Instance.SmallBattleLogCanvas.transform);
            return;
        }


        int FirstBinaryTree = 1;
        FirstBinaryTree *= 2;
        if (Target == null) FirstBinaryTree--;

        FirstBinaryTree *= 2;
        if (effectOnUser == 0) FirstBinaryTree--;

        FirstBinaryTree *= 2;
        if (effectOnTarget == 0) FirstBinaryTree--;

        switch (FirstBinaryTree)
        {
            case 1: /*<> used [] increasing his Resistance by ().
                      <>, resistance is now <PR>/<MR>.*/
                firstLog.CreateAnyText("<> used [] increasing his Resistance by ()",
                   new UnitScript[] { Defender, Defender },
                   new Face[] { ability },
                   new EffectType[] { effectOnUser },
                   new int[] { DefenseValue, (ability.Stats.DamageType == DamageTypes.Magical ? Defender.UnitStats.MagicDefence : Defender.UnitStats.Defence) });

                secondLog.CreateAnyText("<> " + ability.Stats.DamageType.ToString() + " resistance is now ()",
                    new UnitScript[] { Defender },
                    null,
                    null,
                    new int[] { (ability.Stats.DamageType == DamageTypes.Magical ? Defender.UnitStats.MagicDefence : Defender.UnitStats.Defence) });

                break;
            case 3:/*<> used [] increasing his Resistance by () and became {}.
                     <>,Resistance is now <PR>/<MR>*/
                firstLog.CreateAnyText("<> used [] increasing his Resistance by () and became {}",
                  new UnitScript[] { Defender, Defender },
                  new Face[] { ability },
                  new EffectType[] { effectOnUser },
                  new int[] { DefenseValue });

                secondLog.CreateAnyText("<> " + ability.Stats.DamageType.ToString() + " resistance is now ()",
                    new UnitScript[] { Defender },
                    null,
                    null,
                    new int[] { (ability.Stats.DamageType == DamageTypes.Magical ? Defender.UnitStats.MagicDefence : Defender.UnitStats.Defence) });
                break;
            case 5:/*<> used [] increasing <> Resistance by () and <> resistance by (), Resistance is now <PR>/<MR>*/
                firstLog.CreateAnyText("<> used [] increasing <> Resistance by () and <> Resistance by () Their " + ability.Stats.DamageType.ToString() + " resistance is now ()",
                   new UnitScript[] { Defender, Defender },
                   new Face[] { ability },
                   new EffectType[] { effectOnUser },
                   new int[] { DefenseValue, (ability.Stats.DamageType == DamageTypes.Magical ? Defender.UnitStats.MagicDefence : Defender.UnitStats.Defence) });

                secondLog.CreateAnyText("<> " + ability.Stats.DamageType.ToString() + " resistance is now ()",
                    new UnitScript[] { Defender },
                    null,
                    null,
                    new int[] { (ability.Stats.DamageType == DamageTypes.Magical ? Defender.UnitStats.MagicDefence : Defender.UnitStats.Defence) });
                //Not Implemented yet

                break;
            case 6://<> used [] increasing <> Resistance by () and <> resistance by () and <> became, Resistance is now <PR>/<MR>
                firstLog.CreateAnyText("<> used [] increasing <> Resistance by () and <> Resistance by () , <> Became {} , " + ability.Stats.DamageType.ToString() + " resistance is now ()",
                  new UnitScript[] { Defender, Defender },
                  new Face[] { ability },
                  new EffectType[] { effectOnUser },
                  new int[] { DefenseValue, (ability.Stats.DamageType == DamageTypes.Magical ? Defender.UnitStats.MagicDefence : Defender.UnitStats.Defence) });

                secondLog.CreateAnyText("<> " + ability.Stats.DamageType.ToString() + " resistance is now()",
                    new UnitScript[] { Defender },
                    null,
                    null,
                    new int[] { (ability.Stats.DamageType == DamageTypes.Magical ? Defender.UnitStats.MagicDefence : Defender.UnitStats.Defence) });
                //Not implemented yet
                break;
            case 7://<> used [] increasing <> Resistance by () and <> became {} and increased <> resistance by (), Resistance is now <PR>/<MR>
                firstLog.CreateAnyText("<> used [] increasing <> Resistance by () and <> Resistance by (), <> Became {}, " + ability.Stats.DamageType.ToString() + " resistance is now ()",
                  new UnitScript[] { Defender, Defender },
                  new Face[] { ability },
                  new EffectType[] { effectOnUser },
                  new int[] { DefenseValue, (ability.Stats.DamageType == DamageTypes.Magical ? Defender.UnitStats.MagicDefence : Defender.UnitStats.Defence) });

                secondLog.CreateAnyText("<> " + ability.Stats.DamageType.ToString() + " resistance is now()",
                    new UnitScript[] { Defender },
                    null,
                    null,
                    new int[] { (ability.Stats.DamageType == DamageTypes.Magical ? Defender.UnitStats.MagicDefence : Defender.UnitStats.Defence) });
                //Not implemented yet
                break;
            case 8:
                //Not implemented yet
                break;
            default:
                firstLog.CreateAnyText($"ERROR tree summ is - {FirstBinaryTree}");
                break;



        }
        Instantiate(firstLog, Instance.SmallBattleLogCanvas.transform);
        Instantiate(secondLog, Instance.SmallBattleLogCanvas.transform);

        return;
        //if (ability == null)
        //{
        //    firstLog.GetComponent<LogRow>().CreateAnyText(
        //     "Effect {} of <> activated and gived him ()",
        //     new UnitScript[] { Defender },
        //     new Face[] { },
        //     new EffectType[] { effectOnUser },
        //     new int[] { DefenseValue });
        //    return;
        //}

        //if (effectOnUser == 0)
        //{
        //    firstLog.GetComponent<LogRow>().CreateAnyText(
        //     "<> used [] raising his defence by ()",
        //     new UnitScript[] { Defender },
        //     new Face[] { ability },
        //     null,
        //     new int[] { DefenseValue });
        //}
        //else
        //{
        //    firstLog.GetComponent<LogRow>().CreateAnyText(
        //     "<> used [] raising his defence by () and gaining {}",
        //     new UnitScript[] { Defender },
        //     new Face[] { ability },
        //     new EffectType[] { effectOnUser },
        //new int[] { DefenseValue });
        //}
    }
    public static void HealLog(UnitScript User, Face Ability, int Heal, UnitScript Target = null, EffectType EffectOnUser = 0, EffectType EffectOnTarget = 0)
    {
        LogRow firstLog = Instantiate(Instance.BasicDamagePrefabRow, Instance.Content.transform).GetComponent<LogRow>();
        firstLog.ParentCanvas = Instance.ButtonCanvas;
        if (Target == null && EffectOnUser == 0)
        {
            firstLog.GetComponent<LogRow>().CreateAnyText(
             "<> used [] and healed itself by () health",
             new UnitScript[] { User },
             new Face[] { Ability },
             null,
             new int[] { Heal });
        }
        else if (Target == null && EffectOnUser != 0)
        {
            firstLog.GetComponent<LogRow>().CreateAnyText(
             "<> used [] and healed itself by () health and received {}",
             new UnitScript[] { User },
             new Face[] { Ability },
             new EffectType[] { EffectOnUser },
             new int[] { Heal });
        }
        else if (Target != null && EffectOnTarget == 0)
        {
            firstLog.GetComponent<LogRow>().CreateAnyText(
             "<> used [] and healed <> by () health",
             new UnitScript[] { User, Target },
             new Face[] { Ability },
             null,
             new int[] { Heal });
        }
        else if (Target != null && EffectOnTarget != 0)
        {
            firstLog.GetComponent<LogRow>().CreateAnyText(
             "<> used [] and healed <> by () health and gived {}",
             new UnitScript[] { User, Target },
             new Face[] { Ability },
             new EffectType[] { EffectOnTarget },
             new int[] { Heal });
        }
        Instantiate(firstLog, Instance.SmallBattleLogCanvas.transform);

    }
    public static void FailLog(UnitScript User, Face Ability, UnitScript Target = null)
    {
        LogRow firstLog = Instantiate(Instance.BasicDamagePrefabRow, Instance.Content.transform).GetComponent<LogRow>();
        firstLog.ParentCanvas = Instance.ButtonCanvas;
        if (Target == null)
        {
            firstLog.GetComponent<LogRow>().CreateAnyText(
             "<> tried to use [] and failed",
             new UnitScript[] { User },
             new Face[] { Ability },
             null,
             null);
        }
        else
        {
            firstLog.GetComponent<LogRow>().CreateAnyText(
             "<> tried to use [] on <> and failed",
             new UnitScript[] { User, Target },
             new Face[] { Ability },
             null,
             null);
        }
        Instantiate(firstLog, Instance.SmallBattleLogCanvas.transform);
    }

    public static void StunLog(UnitScript Stunned)
    {
        LogRow firstLog = Instantiate(Instance.BasicDamagePrefabRow, Instance.Content.transform).GetComponent<LogRow>();
        firstLog.ParentCanvas = Instance.ButtonCanvas;
        {
            firstLog.GetComponent<LogRow>().CreateAnyText(
             "<> skips his turn because of {}",
             new UnitScript[] { Stunned },
             new Face[] { },
             new EffectType[] { EffectType.Stun });
        }
        Instantiate(firstLog, Instance.SmallBattleLogCanvas.transform);
    }
    public static void CreateEffectLog(UnitScript User, Face Ability, UnitScript Target = null, int durationForUser = 0, int durationForTarget = 0, EffectType EffectOnUser = 0, EffectType EffectOnTarget = 0)
    {
        LogRow firstLog = Instantiate(Instance.BasicDamagePrefabRow, Instance.Content.transform).GetComponent<LogRow>();
        firstLog.ParentCanvas = Instance.ButtonCanvas;
        int FirstBinaryTree = 1;
        FirstBinaryTree *= 2;
        if (EffectOnUser == 0) FirstBinaryTree--;
        FirstBinaryTree *= 2;
        if (durationForUser == 0) FirstBinaryTree--;
        FirstBinaryTree *= 2;
        if (EffectOnTarget == 0) FirstBinaryTree--;
        FirstBinaryTree *= 2;
        if (durationForTarget == 0) FirstBinaryTree--;


        switch (FirstBinaryTree)
        {
            case 4://  <Caster> used [] Causes <Defender> to become {} for This turn.
                firstLog.CreateAnyText("<> used [] Causing <> to become {} for this turn.",
             new UnitScript[] { User, Target },
             new Face[] { Ability },
             new EffectType[] { EffectOnTarget },
             null);
                break;
            case 5:// <Caster> used [] Causes <Defender> to become {} for () turns
                firstLog.CreateAnyText("<> used [] causing <> to become {} for () turns.",
              new UnitScript[] { User, Target },
              new Face[] { Ability },
              new EffectType[] { EffectOnTarget },
              new int[] { durationForTarget });
                break;
            case 9://   <Caster> used [] Causes Himself to become {} for This turn

                firstLog.CreateAnyText("<> used [] causing himself to become {} for this turn.",
            new UnitScript[] { User },
            new Face[] { Ability },
            new EffectType[] { EffectOnUser },
            null);
                break;
            case 11://     <Caster> used []  Causes Himself to become {} and <Defender> to become {} for This turn

                firstLog.CreateAnyText("<> used [] causing himself to become {} for this turn and <> to become {} for this turn.",
            new UnitScript[] { User, Target },
            new Face[] { Ability },
            new EffectType[] { EffectOnUser, EffectOnTarget },
            null);
                break;
            case 12://     <Caster> used []  Causes Himself to become {} for this turn and <Defender> to become {} for () turns.

                firstLog.CreateAnyText("<> used [] causing himself to become {} for this turn and <> to become {} for () turns.",
            new UnitScript[] { User, Target },
            new Face[] { Ability },
            new EffectType[] { EffectOnUser, EffectOnTarget },
            new int[] { durationForTarget });
                break;
            case 13://     <Caster> used []  Causes Himself to become {} for () turns

                firstLog.CreateAnyText("<> used [] Causing <> to become {} for () turns.",
            new UnitScript[] { User, Target },
            new Face[] { Ability },
            new EffectType[] { EffectOnTarget },
            new int[] { durationForUser });
                break;
            case 15://     <Caster> used []  Causes Himself to become {} for () turns and <Defender> to become {} for This turn

                firstLog.CreateAnyText("<> used [] causing himself to become {} for () turn and <> to become {} for this turn.",
            new UnitScript[] { User, Target },
            new Face[] { Ability },
            new EffectType[] { EffectOnUser, EffectOnTarget },
            new int[] { durationForUser });
                break;
            case 16://     <Caster> used [] Causes Himself to become {} for () turns and <Defender> to become {} for () turns.
                firstLog.CreateAnyText("<> used [] causing himself to become {} for () turns and <> to become {} for () turns.",
            new UnitScript[] { User, Target },
            new Face[] { Ability },
            new EffectType[] { EffectOnUser, EffectOnTarget },
            new int[] { durationForTarget, durationForTarget });
                break;
            default:
                firstLog.CreateAnyText($"ERROR tree summ is - {FirstBinaryTree}");
                break;
        }
        Instantiate(firstLog, Instance.SmallBattleLogCanvas.transform);
    }
    public static void StartTurn(int number)
    {
        Instantiate(Instance.BasicDamagePrefabRow, Instance.Content.transform).GetComponent<LogRow>().CreateAnyText(
                $"START OF THE TURN {number}");
        Instantiate(Instance.BasicDamagePrefabRow, Instance.SmallBattleLogCanvas.transform).GetComponent<LogRow>().CreateAnyText(
              $"START OF THE TURN {number}");
    }

    public void OpenBigLog()
    {
        SmallBattleLogCanvas.transform.parent.parent.gameObject.SetActive(false);
        BigBattleLogCanvas.transform.parent.parent.gameObject.SetActive(true);
        //StartCoroutine(CloseBigLogWithTimer());

    }
    public void CloseBigLog()
    {
        SmallBattleLogCanvas.transform.parent.parent.gameObject.SetActive(true);
        BigBattleLogCanvas.transform.parent.parent.gameObject.SetActive(false);
    }
    public IEnumerator CloseBigLogWithTimer()
    {
        yield return new WaitForSeconds(1f);
        SmallBattleLogCanvas.transform.parent.parent.gameObject.SetActive(true);
        BigBattleLogCanvas.transform.parent.parent.gameObject.SetActive(false);


    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        StopCoroutine(CloseBigLogWithTimer());
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        StartCoroutine(CloseBigLogWithTimer());
    }
}
