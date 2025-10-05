using System;
using UnityEngine;

public enum CardPower
{
    None,
    DamageAroundOnAttack, //Karot
    AvoidFirstAttack, //leek
    AttackEnemyBehind, //Banana
    ReturnAttackDamages, //Potato
    HealNearbyCardsThatAttacks, //Navet
    DamageRandomEnemyOnAttack, //Arty lery
    TakeNoDamagesFromPower, //Melon
    AttackTwoTime, //bros
    ReduceAttackDamages, //bros
}

public static class CardPowerDatabase
{
    public static string GetDescrition(this CardPower power)
    {
        string desc = "";
        switch (power)
        {
            case CardPower.None:
                break;
            case CardPower.DamageAroundOnAttack:
                desc = "[After Attack:] Damage adjacent cards";
                break;
            case CardPower.AvoidFirstAttack:
                desc = "Flee the first attack and avoid damages";
                break;
            case CardPower.AttackEnemyBehind:
                desc = "[After Attack:] Damage the next enemy";
                break;
            case CardPower.ReturnAttackDamages:
                desc = "[When Attacked:] Return damages";
                break;
            case CardPower.HealNearbyCardsThatAttacks:
                desc = "Heal nearby cards that attacks";
                break;
            case CardPower.DamageRandomEnemyOnAttack:
                desc = "[After Attack:] Damage a random enemy";
                break;
            case CardPower.TakeNoDamagesFromPower:
                desc = "Take no damages from powers";
                break;
            case CardPower.AttackTwoTime:
                desc = "Attack two times. Once only";
                break;
            case CardPower.ReduceAttackDamages:
                desc = "Reduce attack damages by 1";
                break;
        }

        return ApplyTextEffects(desc);
    }

    public static string ApplyTextEffects(string str)
    {
        str = str.Replace("[", "<i><b><smallcaps>");
        str = str.Replace("]", "</i></b></smallcaps>");
        return str;
    }

    public static CardPowerBase GetPowerObjectFor(this CardPower power, CardBattle target)
    {
        switch (power)
        {
            case CardPower.None:
                return null;
            case CardPower.DamageAroundOnAttack:
                return new PowerDamageAroundOnAttack(target);
            case CardPower.AvoidFirstAttack:
                return new PowerAvoidFirstAttack(target);
            case CardPower.AttackEnemyBehind:
                return new PowerAttackEnemyBehind(target);
            case CardPower.ReturnAttackDamages:
                return new PowerReturnAttack(target);
            case CardPower.HealNearbyCardsThatAttacks:
                return new PowerHealNearbyAttack(target);
            case CardPower.DamageRandomEnemyOnAttack:
                return new PowerAttackRandomEnemy(target);
            case CardPower.TakeNoDamagesFromPower:
                return new PowerTakeNoDamagesFromPower(target);
            case CardPower.AttackTwoTime:
                return new PowerAttackTwoTimes(target);
            case CardPower.ReduceAttackDamages:
                return new PowerReduceAttackDamage(target);
            default:
                throw new NotImplementedException();
        }
    }
}

public abstract class CardPowerBase
{
    protected CardBattle owner;

    public CardPowerBase(CardBattle owner)
    {
        this.owner = owner;
    }

    //Attack
    public virtual void PreAction(BattleAction action) { }
    public virtual void PostAction(BattleAction action) { }

    //Any
    public virtual void PreAnyAction(BattleAction action) { }
    public virtual void PostAnyAction(BattleAction action) { }
    public virtual void PostCardDie(CardBattle card) { }
}

public class PowerDamageAroundOnAttack : CardPowerBase
{
    public PowerDamageAroundOnAttack(CardBattle owner) : base(owner)
    {
    }

    public override void PostAction(BattleAction action)
    {
        if (action.type != BattleActionType.AttackDamage || action.source != owner)
            return;

        CardDuelManager.Instance.GetAdjacentCards(owner, out CardBattle left, out CardBattle right);
        if (left != null)
            CardDuelManager.Instance.RegisterAction(owner, left, BattleActionType.PowerDamage, 1, true);
        if (right != null)
            CardDuelManager.Instance.RegisterAction(owner, right, BattleActionType.PowerDamage, 1);
    }
}

public class PowerAvoidFirstAttack : CardPowerBase
{
    bool hasBeenUsed;

    public PowerAvoidFirstAttack(CardBattle owner) : base(owner)
    {
    }

    public override void PreAction(BattleAction action)
    {
        if (hasBeenUsed)
            return;
        //On attack, on us
        if (action.type != BattleActionType.AttackDamage || action.target != owner)
            return;

        hasBeenUsed = true;
        action.amount = 0;
    }
}

public class PowerAttackEnemyBehind : CardPowerBase
{
    public PowerAttackEnemyBehind(CardBattle owner) : base(owner)
    {
    }

    public override void PostAction(BattleAction action)
    {
        //On attack, from us
        if (action.type != BattleActionType.AttackDamage || action.source != owner)
            return;

        bool hasTarget = CardDuelManager.Instance.TryGetNextCard(action.target, action.target.team.teamDir, out CardBattle target);
        if (hasTarget)
            CardDuelManager.Instance.RegisterAction(owner, target, BattleActionType.PowerDamage, 1);
    }
}

public class PowerReturnAttack : CardPowerBase
{
    public PowerReturnAttack(CardBattle owner) : base(owner)
    {
    }

    public override void PostAction(BattleAction action)
    {
        //On attack, on us
        if (action.type != BattleActionType.AttackDamage || action.target != owner)
            return;

        CardDuelManager.Instance.RegisterAction(owner, action.source, BattleActionType.PowerDamage, action.amount);
    }
}

public class PowerHealNearbyAttack : CardPowerBase
{
    public PowerHealNearbyAttack(CardBattle owner) : base(owner)
    {
    }

    public override void PostAnyAction(BattleAction action)
    {
        //On attack
        if (action.type != BattleActionType.AttackDamage)
            return;

        if (owner.isDead)
            return;

        CardDuelManager.Instance.GetAdjacentCards(owner, out CardBattle left, out CardBattle right);

        if(left == action.source && left!=null)
            CardDuelManager.Instance.RegisterAction(owner, action.source, BattleActionType.Heal, 1);
        if (right == action.source && right != null)
            CardDuelManager.Instance.RegisterAction(owner, action.source, BattleActionType.Heal, 1);
    }
}

public class PowerAttackRandomEnemy : CardPowerBase
{
    public PowerAttackRandomEnemy(CardBattle owner) : base(owner)
    {
    }

    public override void PostAnyAction(BattleAction action)
    {
        //On attack from us
        if (action.type != BattleActionType.AttackDamage || action.source!=owner)
            return;

        if (owner.isDead)
            return;

        if (CardDuelManager.Instance.TryGetRandomCard(action.target.team.teamDir, out CardBattle card))
            CardDuelManager.Instance.RegisterAction(owner, card, BattleActionType.PowerDamage, 1);
    }
}

public class PowerTakeNoDamagesFromPower : CardPowerBase
{
    public PowerTakeNoDamagesFromPower(CardBattle owner) : base(owner)
    {
    }

    public override void PreAction(BattleAction action)
    {
        //On power on us
        if (action.type != BattleActionType.PowerDamage || action.target != owner)
            return;

        action.amount = 0;
    }
}

public class PowerAttackTwoTimes : CardPowerBase
{
    bool used;

    public PowerAttackTwoTimes(CardBattle owner) : base(owner)
    {
    }

    public override void PostAction(BattleAction action)
    {
        if (used)
            return;
        //On attack from us
        if (action.type != BattleActionType.AttackDamage || action.source != owner)
            return;

        used = true;
        BattleAction doubleAction = new BattleAction(action.source, action.target, BattleActionType.AttackDamage, action.amount);
        CardDuelManager.Instance.RegisterAction(doubleAction);
    }
}

public class PowerReduceAttackDamage : CardPowerBase
{
    public PowerReduceAttackDamage(CardBattle owner) : base(owner)
    {
    }

    public override void PreAction(BattleAction action)
    {
        //On attack on us
        if (action.type != BattleActionType.AttackDamage || action.target != owner)
            return;

        action.amount = Mathf.Max(0, action.amount - 1);
    }
}
