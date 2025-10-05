using System;
using UnityEngine;

public enum CardPower
{
    None,
    DamageAroundOnAttack,
}

public static class CardPowerDatabase
{
    public static string GetDescrition(this CardPower power)
    {
        string desc = "";
        switch (power)
        {
            case CardPower.DamageAroundOnAttack:
                desc = "[After Attack:] Damage adjacent cards";
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
                return new CardPowerDamageAroundOnAttack(target);
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

public class CardPowerDamageAroundOnAttack : CardPowerBase
{
    public CardPowerDamageAroundOnAttack(CardBattle owner) : base(owner)
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