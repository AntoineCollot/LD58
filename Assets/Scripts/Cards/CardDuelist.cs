using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CardDuelist : MonoBehaviour
{
    public ScriptableCard[] cards;
    public const int MAX_TEAM_SIZE = 4;
}

[System.Serializable]
public class CardBattleTeam
{
    public CardBattleTeam(ScriptableCard[] cards)
    {
        for (int i = 0; i < cards.Length; i++)
        {
            AddCardToTeam(cards[i].data);
        }
    }

    void AddCardToTeam(CardData data)
    {
        teamCards.Add(new CardBattle(this,data));
    }

    public CardBattle GetFirstCard()
    {
       return teamCards.First(c => c.IsAlive);
    }

    public int AliveCardCount
    {
        get
        {
            int count = 0;
            for (int i = 0; i < teamCards.Count; i++)
            {
                if (teamCards[i].IsAlive)
                    count++;
            }
            return count;
        }
    }

    public bool HasCardAlive=>AliveCardCount> 0;

    public List<CardBattle> teamCards;
}

public class CardBattle
{
    public CardBattle(CardBattleTeam team,CardData data)
    {
        this.data = data;
        currentStrength = data.strength;
        currentHP = data.hp;
        this.team = team;
        power = data.power.GetPowerObjectFor(this);
        isDead = false;
    }

    public CardBattleTeam team;
    public CardPowerBase power;

    public CardData data;
    public int currentHP;
    public int currentStrength;
    public int hitReceived;
    public bool isDead;

    public bool IsAlive =>!isDead;

    public void Damage(int amount)
    {
        if (isDead)
            return;

        currentHP -= amount;
        hitReceived++;

        if (currentHP < 0)
            Die(); 
    }

    public void Die()
    {
        isDead = true;
        CardDuelManager.Instance.RegisterCardDied(this);
    }

    public void AttackCard(CardBattle other)
    {
        other.Damage(currentStrength);
    }

    public bool IsSameTeam(CardBattleTeam other) => other == team;
}