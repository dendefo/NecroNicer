using System.Collections.Generic;
using UnityEngine;
using static Face.FaceStats;

[System.Serializable]
public struct InstanceFace
{
    public const int ENEMY_UNIT_RANDOM_HEAL = 7;
    public Face faceDefenition;
    [Range(0, 10)] public int Uses;

    static InstanceFace LastUse;
    static UnitScript LastUser;
    static UnitScript LastTarget;

    public static void AfterAnimation()
    {
        switch (LastUse.faceDefenition.Stats.Ability)
        {
            case AbilityType.BasicMelleeAttack:
                BasicMeleeAttack(LastUse, LastUser, LastTarget);
                break;
            case AbilityType.DefensiveStance:
                DefensiveStance(LastUse, LastUser);
                break;
            case AbilityType.SavageSmash:
                SavageSmash(LastUse, LastUser, LastTarget);
                break;
            case AbilityType.SoulSiphon:
                SoulSiphon(LastUse, LastUser);
                break;
            case AbilityType.SoulBlast:
                SoulBlast(LastUse, LastUser, LastTarget);
                break;
            case AbilityType.SoulBarrier:
                SoulBarrier(LastUse, LastUser, LastTarget);
                break;
            case AbilityType.Rage:
                Rage(LastUse, LastUser, LastTarget);
                break;
            case AbilityType.Carnage:
                Carnage(LastUse, LastUser);
                break;
            case AbilityType.EtherealShackles:
                EtherealShackles(LastUse, LastUser, LastTarget);
                break;

        }
        LastUse.Use();
    }
    public void Use()
    {
        Uses--;
        if (Uses == 0)
        {
            faceDefenition = GameManager.Instance.MainLibrary.Downgrade(faceDefenition);
            Uses = faceDefenition.Stats.MaxUses;
        }
    }
    static public void ActivateAbility(InstanceFace instance, UnitScript from, UnitScript target = null)
    {


        ((BattleSceneManager)BattleSceneManager.Instance).FirstButton.gameObject.SetActive(false);
        ((BattleSceneManager)BattleSceneManager.Instance).SecondButton.gameObject.SetActive(false);
        switch (instance.faceDefenition.Stats.Ability)
        {
            case AbilityType.Carnage:
                from.UnitAnimator.SetTrigger("Ability");
                break;
            case AbilityType.BasicMelleeAttack:
                from.UnitAnimator.SetTrigger("Attack");
                break;
            case AbilityType.DefensiveStance:
                from.UnitAnimator.SetTrigger("Defence");
                break;
            case AbilityType.SavageSmash:
                from.UnitAnimator.SetTrigger("Attack");
                break;
            case AbilityType.SoulSiphon:
                from.UnitAnimator.SetTrigger("Ability");
                break;
            case AbilityType.EtherealShackles:
                from.UnitAnimator.SetTrigger("Ability");
                break;
            case AbilityType.Rage:
                from.UnitAnimator.SetTrigger("Ability");
                break;
            case AbilityType.SoulBarrier:
                from.UnitAnimator.SetTrigger("Ability");
                break;
            case AbilityType.SoulBlast:
                from.UnitAnimator.SetTrigger("Ability");
                break;
        }

        LastUse = instance;
        LastUser = from;
        LastTarget = target;

    }
    static void BasicMeleeAttack(InstanceFace instance, UnitScript Attacker, UnitScript Defender)
    {
        var outcome = Attacker.Attack(instance.faceDefenition, Defender);
        int flatdmg = 0;
        switch (instance.faceDefenition.Stats.Tier)
        {
            case TierTypes.Corrupted:
                if (Random.value > 0.25f) flatdmg = Attacker.TakePureDamage(instance.faceDefenition.Stats.Value / 2);
                break;
            case TierTypes.Degraded:
                if (Random.value > 0.5f) flatdmg = Attacker.TakePureDamage(instance.faceDefenition.Stats.Value / 2);
                break;
            case TierTypes.Pure:
                //Plus three if target one grid away
                break;
        }

        BattleLogManager.AttackLog(Attacker, Defender, instance.faceDefenition, outcome, flatdmg, effectOnAttacker: flatdmg != 0 ? EffectType.Backfire : EffectType.None);
    }
    static void DefensiveStance(InstanceFace instance, UnitScript User)
    {
        int currentTurn = ((BattleSceneManager)BattleSceneManager.Instance).turn;
        int def = User.RaiseDefence(instance.faceDefenition.Stats.Value);
        bool positiveEffect = false;
        bool negativeEffect = false;
        switch (instance.faceDefenition.Stats.Tier)
        {
            case TierTypes.Corrupted:
                negativeEffect = User.activeEffects.AddEffect(EffectType.Exhausted, 1, currentTurn);
                break;
            case TierTypes.Degraded:
                if (Random.value > 0.5f) negativeEffect = User.activeEffects.AddEffect(EffectType.Exhausted, 1, currentTurn);
                break;
            case TierTypes.Pure:
                positiveEffect = User.activeEffects.AddEffect(EffectType.Fortified, 1, currentTurn, instance.faceDefenition.Stats.Value / 2);
                break;
        }
        BattleLogManager.DefendLog(User, def, instance.faceDefenition, negativeEffect ? EffectType.Exhausted : (positiveEffect ? EffectType.Fortified : 0));
    }
    static void SavageSmash(InstanceFace instance, UnitScript Attacker, UnitScript Defender)
    {
        int currentTurn = ((BattleSceneManager)BattleSceneManager.Instance).turn;
        bool EffectAdded = false;
        AttackOutcome outcome;
        switch (instance.faceDefenition.Stats.Tier)
        {
            case TierTypes.Corrupted:
                if (Random.value > 0.5f)
                {
                    BattleLogManager.FailLog(Attacker, instance.faceDefenition, Defender);
                    return;
                }
                outcome = Attacker.Attack(instance.faceDefenition, Defender);
                break;
            case TierTypes.Degraded:
                if (Random.value > 0.75f)
                {
                    BattleLogManager.FailLog(Attacker, instance.faceDefenition, Defender);
                    return;
                }
                outcome = Attacker.Attack(instance.faceDefenition, Defender);
                break;
            case TierTypes.Pure:
                outcome = Attacker.Attack(instance.faceDefenition, Defender);
                EffectAdded = Defender.activeEffects.AddEffect(EffectType.Exhausted, 1, currentTurn);
                break;
            default:
                outcome = Attacker.Attack(instance.faceDefenition, Defender);
                break;
        }
        if (EffectAdded) BattleLogManager.AttackLog(Attacker, Defender, instance.faceDefenition, outcome, effectOnDefender: EffectType.Exhausted);
        else BattleLogManager.AttackLog(Attacker, Defender, instance.faceDefenition, outcome);
    }
    static void SoulSiphon(InstanceFace instance, UnitScript User)
    {
        int health2 = 0;
        UnitScript healed = null;

        List<UnitScript> ListOfFriendly = User.IsEnemy ? ((BattleSceneManager)BattleSceneManager.Instance).Enemies : GameManager.Instance.PlayersUnits;
        List<UnitScript> ListOfEnemies = User.IsEnemy ? GameManager.Instance.PlayersUnits : ((BattleSceneManager)BattleSceneManager.Instance).Enemies;
        switch (instance.faceDefenition.Stats.Tier)
        {
            case TierTypes.Corrupted:
                if (ListOfEnemies.Count <= 0) break;

                healed = ListOfEnemies[Random.Range(0, ListOfEnemies.Count)];
                health2 = healed.Heal(ENEMY_UNIT_RANDOM_HEAL);
                break;

            case TierTypes.Degraded:
                if (Random.value > 0.5f)
                {
                    BattleLogManager.FailLog(User, instance.faceDefenition);
                    return;
                }
                break;

            case TierTypes.Pure:
                if (Random.value > 0.5f) break;
                if (ListOfFriendly.Count <= 1) break;

                do healed = ListOfFriendly[Random.Range(0, ListOfFriendly.Count)];
                while (healed == User);

                health2 = healed.Heal(instance.faceDefenition.Stats.Value);
                break;

        }
        int health = User.Heal(instance.faceDefenition.Stats.Value);
        BattleLogManager.HealLog(User, instance.faceDefenition, health);
        if (health2 != 0) BattleLogManager.HealLog(User, instance.faceDefenition, health2, healed);
    }
    static void SoulBlast(InstanceFace instance, UnitScript Attacker, UnitScript Defender)
    {
        int currentTurn = ((BattleSceneManager)BattleSceneManager.Instance).turn;
        var outcome = Attacker.Attack(instance.faceDefenition, Defender);
        int flatdmg = 0;
        bool EffectAdded = false;
        switch (instance.faceDefenition.Stats.Tier)
        {
            case TierTypes.Corrupted:
                if (Random.value > 0.25f) flatdmg = Attacker.TakePureDamage(instance.faceDefenition.Stats.Value / 2);
                break;
            case TierTypes.Degraded:
                if (Random.value > 0.5f) flatdmg = Attacker.TakePureDamage(instance.faceDefenition.Stats.Value / 2);
                break;
            case TierTypes.Pure:
                EffectAdded = Defender.activeEffects.AddEffect(EffectType.SoulDischarge, 1, currentTurn);
                break;

        }
        if (EffectAdded) BattleLogManager.AttackLog(Attacker, Defender, instance.faceDefenition, outcome, effectOnDefender: EffectType.SoulDischarge);
        else BattleLogManager.AttackLog(Attacker, Defender, instance.faceDefenition, outcome, flatdmg, effectOnAttacker: flatdmg != 0 ? EffectType.Backfire : EffectType.None);
    }
    static void SoulBarrier(InstanceFace instance, UnitScript User, UnitScript Friendly)
    {

        int CurrentTurn = ((BattleSceneManager)BattleSceneManager.Instance).turn;
        int def = Friendly.RaiseMagicDefence(instance.faceDefenition.Stats.Value);
        int flatdmg = 0;
        bool isBackFire = false;
        bool isSoulDischargeEffect = false;
        bool isPositive = false;
        switch (instance.faceDefenition.Stats.Tier)
        {
            case TierTypes.Corrupted:
                if (Random.value > 0.25f)
                {
                    flatdmg = User.TakePureDamage(instance.faceDefenition.Stats.Value / 2);
                    isBackFire = true;
                }
                break;
            case TierTypes.Degraded:
                if (Random.value > 0.50f) isSoulDischargeEffect = User.activeEffects.AddEffect(EffectType.SoulDischarge, 1, CurrentTurn);
                break;
            case TierTypes.Pure:
                isPositive = Friendly.activeEffects.AddEffect(EffectType.SoulCharged, 1, CurrentTurn);
                break;
        }
        if (isPositive) BattleLogManager.DefendLog(Friendly, def, instance.faceDefenition, EffectType.SoulCharged);
        else BattleLogManager.DefendLog(Friendly, def, instance.faceDefenition, isBackFire ? EffectType.Backfire : (isSoulDischargeEffect ? EffectType.SoulDischarge : 0));
    }
    static void Rage(InstanceFace instance, UnitScript User, UnitScript Target)
    {
        int currentTurn = ((BattleSceneManager)BattleSceneManager.Instance).turn;
        UnitScript RagedEnemyUnit = null;
        List<UnitScript> ListOfEnemies = User.IsEnemy ? GameManager.Instance.PlayersUnits : ((BattleSceneManager)BattleSceneManager.Instance).Enemies;
        switch (instance.faceDefenition.Stats.Tier)
        {
            case TierTypes.Corrupted:
                RagedEnemyUnit = ListOfEnemies[Random.Range(0, ListOfEnemies.Count)];
                User.activeEffects.AddEffect(EffectType.Rage, 1, currentTurn);
                RagedEnemyUnit.activeEffects.AddEffect(EffectType.Rage, 1, currentTurn);
                BattleLogManager.CreateEffectLog(User, instance.faceDefenition, RagedEnemyUnit, 1, 1, EffectType.Rage, EffectType.Rage);
                return;
            case TierTypes.Degraded:
                if (Random.value > 0.25f)
                {
                    RagedEnemyUnit = ListOfEnemies[Random.Range(0, ListOfEnemies.Count)];
                    User.activeEffects.AddEffect(EffectType.Rage, 1, currentTurn);
                    RagedEnemyUnit.activeEffects.AddEffect(EffectType.Rage, 1, currentTurn);
                    BattleLogManager.CreateEffectLog(User, instance.faceDefenition, RagedEnemyUnit, 1, 1, EffectType.Rage, EffectType.Rage);
                    return;

                }
                else
                {
                    BattleLogManager.FailLog(User, instance.faceDefenition);
                    return;
                }
            case TierTypes.Balanced:
                if (Random.value > 0.50f)
                {
                    User.activeEffects.AddEffect(EffectType.Rage, 1, currentTurn);
                    BattleLogManager.CreateEffectLog(User, instance.faceDefenition, null, 1, 0, EffectType.Rage);

                    return;
                }
                else
                {
                    BattleLogManager.FailLog(User, instance.faceDefenition);
                    return;
                }
            case TierTypes.Cleansed:
                if (Random.value > 0.25f)
                {
                    User.activeEffects.AddEffect(EffectType.Rage, 1, currentTurn);
                    BattleLogManager.CreateEffectLog(User, instance.faceDefenition, null, 1, 0, EffectType.Rage);
                    return;
                }
                else
                {
                    BattleLogManager.FailLog(User, instance.faceDefenition);
                    return;
                }
            case TierTypes.Pure:

                User.activeEffects.AddEffect(EffectType.Rage, Random.value > 0.5f ? 2 : 1, currentTurn);
                BattleLogManager.CreateEffectLog(User, instance.faceDefenition, null, 1, 0, EffectType.Rage); //Need to add Either for this or next (1,2)
                return;

        }
    }
    static void Carnage(InstanceFace instance, UnitScript User)
    {
        int currentTurn = ((BattleSceneManager)BattleSceneManager.Instance).turn;
        UnitScript RagedEnemyUnit = null;
        List<UnitScript> ListOfEnemies = User.IsEnemy ? GameManager.Instance.PlayersUnits : ((BattleSceneManager)BattleSceneManager.Instance).Enemies;
        switch (instance.faceDefenition.Stats.Tier)
        {
            case TierTypes.Corrupted:
                RagedEnemyUnit = ListOfEnemies[Random.Range(0, ListOfEnemies.Count)];
                User.activeEffects.AddEffect(EffectType.Carnage, 1, currentTurn);
                RagedEnemyUnit.activeEffects.AddEffect(EffectType.Carnage, 1, currentTurn);
                BattleLogManager.CreateEffectLog(User, instance.faceDefenition, RagedEnemyUnit, 1, 1, EffectType.Carnage, EffectType.Carnage);
                return;

            case TierTypes.Degraded:
                if (Random.value > 0.5f)
                {
                    RagedEnemyUnit = ListOfEnemies[Random.Range(0, ListOfEnemies.Count)];
                    User.activeEffects.AddEffect(EffectType.Rage, 1, currentTurn);
                    RagedEnemyUnit.activeEffects.AddEffect(EffectType.Rage, 1, currentTurn);
                    BattleLogManager.CreateEffectLog(User, instance.faceDefenition, RagedEnemyUnit, 1, 1, EffectType.Rage, EffectType.Rage);
                    return;
                }
                else
                {
                    BattleLogManager.FailLog(User, instance.faceDefenition);
                    return;
                }
            case TierTypes.Balanced:
                User.activeEffects.AddEffect(EffectType.Rage, 1, currentTurn);
                BattleLogManager.CreateEffectLog(User, instance.faceDefenition, null, 1, 0, EffectType.Rage);
                return;
            case TierTypes.Cleansed:
                if (Random.value > 0.5f)
                {
                    User.activeEffects.AddEffect(EffectType.Carnage, 1, currentTurn);
                    BattleLogManager.CreateEffectLog(User, instance.faceDefenition, null, 1, 0, EffectType.Carnage);
                    return;
                }
                else
                {
                    BattleLogManager.FailLog(User, instance.faceDefenition);
                    return;
                }
            case TierTypes.Pure:

                User.activeEffects.AddEffect(EffectType.Carnage, 1, currentTurn);
                BattleLogManager.CreateEffectLog(User, instance.faceDefenition, null, 1, 0, EffectType.Carnage);
                return;

        }
    }
    static void EtherealShackles(InstanceFace instance, UnitScript Attacker, UnitScript Defender)
    {
        int currentTurn = ((BattleSceneManager)BattleSceneManager.Instance).turn;
        bool isEnemyStun = false;
        bool isPlayerStun = false;
        switch (instance.faceDefenition.Stats.Tier)
        {
            case TierTypes.Corrupted:
                isEnemyStun = Defender.activeEffects.AddEffect(EffectType.Stun, 1, currentTurn);
                if (Random.value > 0.5f) isPlayerStun = Attacker.activeEffects.AddEffect(EffectType.Stun, 1, currentTurn);
                if (isEnemyStun)
                {
                    if (isPlayerStun) BattleLogManager.CreateEffectLog(Attacker, instance.faceDefenition, Defender, 1, 1, EffectType.Stun, EffectType.Stun);
                    else if (!isPlayerStun) BattleLogManager.CreateEffectLog(Attacker, instance.faceDefenition, Defender, 0, 1, EffectType.None, EffectType.Stun);
                }
                break;
            case TierTypes.Degraded:
                if (Random.value > 0.25f) isEnemyStun = Defender.activeEffects.AddEffect(EffectType.Stun, 1, currentTurn);
                if (Random.value > 0.75f) isPlayerStun = Attacker.activeEffects.AddEffect(EffectType.Stun, 1, currentTurn);
                if (isEnemyStun)
                {
                    if (isPlayerStun) BattleLogManager.CreateEffectLog(Attacker, instance.faceDefenition, Defender, 1, 1, EffectType.Stun, EffectType.Stun);
                    else if (!isPlayerStun) BattleLogManager.CreateEffectLog(Attacker, instance.faceDefenition, Defender, 0, 1, EffectType.None, EffectType.Stun);
                }
                break;
            case TierTypes.Balanced:
                if (Random.value > 0.75f) isEnemyStun = Defender.activeEffects.AddEffect(EffectType.Stun, 1, currentTurn);
                if (isEnemyStun) BattleLogManager.CreateEffectLog(Attacker, instance.faceDefenition, Defender, 0, 1, EffectType.None, EffectType.Stun);
                break;
            case TierTypes.Cleansed:
                if (Random.value > 0.5f) isEnemyStun = Defender.activeEffects.AddEffect(EffectType.Stun, 1, currentTurn);
                if (isEnemyStun) BattleLogManager.CreateEffectLog(Attacker, instance.faceDefenition, Defender, 0, 1, EffectType.None, EffectType.Stun);
                break;
            case TierTypes.Pure:
                isEnemyStun = Defender.activeEffects.AddEffect(EffectType.Stun, 1, currentTurn);
                if (isEnemyStun) BattleLogManager.CreateEffectLog(Attacker, instance.faceDefenition, Defender, 0, 1, EffectType.None, EffectType.Stun);
                break;
        }
        if (!isEnemyStun) BattleLogManager.FailLog(Attacker, instance.faceDefenition, Defender);


    }
    /*
50% chance enemy become Stun
100% chance enemy enemy become Stun*/

    static public bool operator ==(InstanceFace left, InstanceFace right)
    {
        return left.faceDefenition == right.faceDefenition && left.Uses == right.Uses;
    }
    static public bool operator !=(InstanceFace left, InstanceFace right)
    {
        return left == right;
    }
}