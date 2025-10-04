using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CardDuelist : MonoBehaviour
{
    [SerializeField] ScriptableTGCCard card0;
    [SerializeField] ScriptableTGCCard card1;
    [SerializeField] ScriptableTGCCard card2;
    [SerializeField] ScriptableTGCCard card3;

    public List<ScriptableTGCCard> Cards
    {
        get
        {
            List<ScriptableTGCCard> cards = new();
            if (card0 != null)
                cards.Add(card0);
            if (card1 != null)
                cards.Add(card1);
            if (card2 != null)
                cards.Add(card2);
            if (card3 != null)
                cards.Add(card3);
            return cards;
        }
    }
    public const int MAX_TEAM_SIZE = 4;
}

[System.Serializable]
public class CardBattleTeam
{
    public CardBattleTeam(ScriptableTGCCard[] cards)
    {
        teamCards = new();
        for (int i = 0; i < cards.Length; i++)
        {
            AddCardToTeam(cards[i].CardData);
        }
    }

    void AddCardToTeam(CardData data)
    {
        teamCards.Add(new CardBattle(this, data));
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

    public bool HasCardAlive => AliveCardCount > 0;

    public List<CardBattle> teamCards;
}

public class CardBattle
{
    public CardBattle(CardBattleTeam team, CardData data)
    {
        this.data = data;
        this.team = team;
        power = data.power.GetPowerObjectFor(this);
        isDead = false;
    }

    public CardBattleTeam team;
    public CardPowerBase power;

    public CardData data;
    public int hitReceived;
    public bool isDead;

    public bool IsAlive => !isDead;

    public void Damage(int amount)
    {
        if (isDead)
            return;

        data.hp -= amount;
        hitReceived++;

        if (data.hp <= 0)
            Die();
    }

    public void Die()
    {
        isDead = true;
        CardDuelManager.Instance.RegisterCardDied(this);
    }

    public void AttackCard(CardBattle other)
    {
        other.Damage(data.strength);
    }

    public bool IsSameTeam(CardBattleTeam other) => other == team;
}