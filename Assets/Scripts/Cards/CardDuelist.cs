using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CardDuelist : MonoBehaviour
{
    //Unity bug with list of scriptables.........
    [SerializeField] ScriptableTGCCard card0;
    [SerializeField] ScriptableTGCCard card1;
    [SerializeField] ScriptableTGCCard card2;
    [SerializeField] ScriptableTGCCard card3;

    Animator anim;
    const string DUEL_ANIM = "IsDuel";

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
    private void Start()
    {
        anim = GetComponentInChildren<Animator>(true);
    }

    private void OnDestroy()
    {
        if (CardDuelManager.Instance != null)
            CardDuelManager.Instance.onDuelEnd -= EndDuel;
    }

    public void SetCards(List<ScriptableTGCCard> cards)
    {
        if (cards.Count > 0)
        {
            card0 = cards[0];
        }
        if (cards.Count > 1)
        {
            card1 = cards[1];
        }
        if (cards.Count > 2)
        {
            card2 = cards[2];
        }
        if (cards.Count > 3)
        {
            card3 = cards[3];
        }
    }

    public void ClearCards()
    {
        card0 = null;
        card1 = null;
        card2 = null;
        card3 = null;
    }

    public void EnterDuel()
    {
        if (anim != null)
            anim.SetBool(DUEL_ANIM, true);

        CardDuelManager.Instance.onDuelEnd += EndDuel;
    }

    public void EndDuel()
    {
        CardDuelManager.Instance.onDuelEnd -= EndDuel;

        if (anim != null)
            anim.SetBool(DUEL_ANIM, false);
    }

    public bool HasValidTeam => card0 != null;

    public const int MAX_TEAM_SIZE = 4;
}

[System.Serializable]
public class CardBattleTeam
{
    public CardBattleTeam(ScriptableTGCCard[] cards, TeamDir teamDir)
    {
        this.teamDir = teamDir;

        teamCards = new();
        for (int i = 0; i < cards.Length; i++)
        {
            AddCardToTeam(cards[i].CardData,i);
        }
    }

    void AddCardToTeam(CardData data, int id)
    {
        teamCards.Add(new CardBattle(this, data, id));
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

    public int GetBattlegroundIDForCard(int IdInTeam)
    {
        switch (teamDir)
        {
            case TeamDir.Left:
            default:
                return CardDuelist.MAX_TEAM_SIZE - IdInTeam -1; 
            case TeamDir.Right:
                return CardDuelist.MAX_TEAM_SIZE + IdInTeam; 
        }
    }

    public bool HasCardAlive => AliveCardCount > 0;

    TeamDir teamDir;
    public List<CardBattle> teamCards;
}

public class CardBattle
{
    public CardBattle(CardBattleTeam team, CardData data, int idInTeam)
    {
        this.data = data;
        this.team = team;
        power = data.power.GetPowerObjectFor(this);
        isDead = false;
        id = idInTeam;
    }

    public CardBattleTeam team;
    public CardPowerBase power;

    public CardData data;
    public int hitReceived;
    public bool isDead;
    public int id;

    public bool IsAlive => !isDead;

    public void Damage(int amount)
    {
        if (isDead)
            return;

        data.hp -= Mathf.Max(0,amount);
        hitReceived++;

        if (data.hp <= 0)
            Die();
    }

    public void Heal(int amount)
    {
        if (isDead)
            return;

        data.hp += Mathf.Max(0, amount);
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

    public int GetBattlegroundID()
    {
        return team.GetBattlegroundIDForCard(id);
    }

    public bool IsSameTeam(CardBattleTeam other) => other == team;
}