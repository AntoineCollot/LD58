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
    public virtual void PreAttack(CardBattle target) { }
    public virtual void PostAttack(CardBattle target) { }

    //Receive
    public virtual void PreReceiveAttack(CardBattle from) { }
    public virtual void PostReceiveAttack(CardBattle from) { }

    //Any
    public virtual void PreAnyAttack(CardBattle from, CardBattle target) { }
    public virtual void PostAnyAttack(CardBattle from, CardBattle target) { }
    public virtual void PostCardDie(CardBattle card) { }
}

public class CardPowerDamageAroundOnAttack : CardPowerBase
{
    public CardPowerDamageAroundOnAttack(CardBattle owner) : base(owner)
    {
    }

    public override void PostAttack(CardBattle target)
    {
        CardDuelManager.Instance.GetAdjacentCards(owner, out CardBattle left, out CardBattle right);
        if (left != null)
            left.Damage(1);
        if (right != null)
            right.Damage(1);

        CardDuelManager.Instance.UpdateDisplay();
    }
}