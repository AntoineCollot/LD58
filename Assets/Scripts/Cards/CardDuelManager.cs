using System.Collections.Generic;
using UnityEngine;

public class CardDuelManager : MonoBehaviour
{
    public static CardDuelManager Instance;
    //Player
    CardBattleTeam leftTeam;
    //opponent
    CardBattleTeam rightTeam;

    List<CardBattle> dyingCards = new();

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void StartDuel(CardDuelist player, CardDuelist opponent)
    {
        StartDuel(new CardBattleTeam(player.cards), new CardBattleTeam(opponent.cards));
    }

    public void StartDuel(CardBattleTeam player, CardBattleTeam opponent)
    {
        int turns = 0;
        const int MAX_TURNS = 30;
        bool victory = false;

        leftTeam = player;
        rightTeam = opponent;

        while (turns < MAX_TURNS)
        {
            if (!opponent.HasCardAlive)
            {
                victory = true;
                break;
            }

            //Oppnent turn
            PlayTurn(opponent, player);

            if (!player.HasCardAlive)
                break;

            //player turn
            PlayTurn(player, opponent);
        }
    }

    void PlayTurn(CardBattleTeam team, CardBattleTeam other)
    {
        CardBattle playingCard = team.GetFirstCard();
        CardBattle targetCard = other.GetFirstCard();

        //Pre callbacks
        playingCard.power?.PreAttack(targetCard);
        targetCard.power?.PreReceiveAttack(playingCard);

        var cards = GetAllCards();
        for (int i = 0; i < cards.Count; i++)
        {
            cards[i].power?.PreAnyAttack(playingCard, targetCard);
        }

        //First card of the playing team attacks the first of opposite team
        playingCard.AttackCard(targetCard);

        //Pre callbacks
        playingCard.power?.PostAttack(targetCard);
        targetCard.power?.PostReceiveAttack(playingCard);

        for (int i = 0; i < cards.Count; i++)
        {
            cards[i].power?.PostAnyAttack(playingCard, targetCard);
        }

        while(ProcessDeadCards())
        {
            //Process all dead cards until there is none left
        }
    }

    public List<CardBattle> GetAllCards()
    {
        List<CardBattle> cards = new();
        cards.AddRange(leftTeam.teamCards);
        cards.AddRange(rightTeam.teamCards);
        return cards;
    }

    public void GetAdjacentCards(CardBattle source, out CardBattle left, out CardBattle right)
    {
        List<CardBattle> cards = GetAllCards();

        left = null;
        right = null;

        for (int i = 0; i < cards.Count; i++)
        {
            //If we found ourselves
            if (cards[i] == source)
            {
                //Try get right card
                for (int j = i; j < cards.Count; j++)
                {
                    if (cards[j].IsAlive)
                    {
                        right = cards[j];
                        break;
                    }
                }
                //Stop as we already have left card if any
                return;
            }

            if (cards[i].IsAlive)
                left = cards[i];
        }
    }

    public void RegisterCardDied(CardBattle card)
    {
        if (!dyingCards.Contains(card))
            dyingCards.Add(card);
    }

    bool ProcessDeadCards()
    {
        if (dyingCards.Count == 0)
            return false;

        foreach (CardBattle deadCard in dyingCards)
        {
            var cards = GetAllCards();
            for (int i = 0; i < cards.Count; i++)
            {
                cards[i].power?.PostCardDie(deadCard);
            }
        }

        dyingCards.Clear();
        return true;
    }
}
