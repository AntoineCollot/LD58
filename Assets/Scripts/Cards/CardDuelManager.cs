using System.Collections;
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

    [Header("Display")]
    [SerializeField] CardDisplay[] cardDisplays;
    const float TURN_ACTION_DURATION = 0.2f;

    //player
    CompositeStateToken freezePlayerToken;

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        freezePlayerToken = new();
        PlayerState.Instance.freezeInputState.Add(freezePlayerToken);

        HideCards();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void HideCards()
    {
        foreach (CardDisplay display in cardDisplays)
        {
            display.Hide();
        }
    }

    #region Duel
    public void StartDuel(CardDuelist player, CardDuelist opponent)
    {
        StartDuel(new CardBattleTeam(player.Cards.ToArray()), new CardBattleTeam(opponent.Cards.ToArray()));
    }

    public void StartDuel(CardBattleTeam player, CardBattleTeam opponent)
    {
        StartCoroutine(Duel(player, opponent));
    }

    IEnumerator Duel(CardBattleTeam player, CardBattleTeam opponent)
    {
        freezePlayerToken.SetOn(true);

        int turns = 0;
        const int MAX_TURNS = 30;
        bool victory = false;

        leftTeam = player;
        rightTeam = opponent;

        UpdateDisplay();

        while (turns < MAX_TURNS)
        {
            if (!opponent.HasCardAlive)
            {
                victory = true;
                break;
            }

            //Oppnent turn
            yield return StartCoroutine(PlayTurn(opponent, player));

            if (!player.HasCardAlive)
                break;

            //player turn
            yield return StartCoroutine(PlayTurn(player, opponent));
        }

        Debug.Log("Duel finished! Victory: " + victory);
        freezePlayerToken.SetOn(false);

        yield return new WaitForSeconds(1);

        HideCards();
    }

    IEnumerator PlayTurn(CardBattleTeam team, CardBattleTeam other)
    {
        CardBattle playingCard = team.GetFirstCard();
        CardBattle targetCard = other.GetFirstCard();

        //Pre callbacks
        playingCard.power?.PreAttack(targetCard);
        targetCard.power?.PreReceiveAttack(playingCard);

        var cards = GetAllCardsInDisplayOrder();
        for (int i = 0; i < cards.Count; i++)
        {
            cards[i].power?.PreAnyAttack(playingCard, targetCard);
        }

        //First card of the playing team attacks the first of opposite team
        playingCard.AttackCard(targetCard);
        UpdateDisplay();

        yield return new WaitForSeconds(TURN_ACTION_DURATION);

        //Pre callbacks
        playingCard.power?.PostAttack(targetCard);
        targetCard.power?.PostReceiveAttack(playingCard);

        for (int i = 0; i < cards.Count; i++)
        {
            cards[i].power?.PostAnyAttack(playingCard, targetCard);
        }

        while (ProcessDeadCards())
        {
            //Process all dead cards until there is none left
            yield return new WaitForSeconds(TURN_ACTION_DURATION);
        }
    }

    public List<CardBattle> GetAllCardsInDisplayOrder()
    {
        List<CardBattle> cards = new();
        cards.AddRange(leftTeam.teamCards);
        //Reverse the left team to have them in disply order
        cards.Reverse();
        cards.AddRange(rightTeam.teamCards);
        return cards;
    }

    public void GetAdjacentCards(CardBattle source, out CardBattle left, out CardBattle right)
    {
        List<CardBattle> cards = GetAllCardsInDisplayOrder();

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
            var cards = GetAllCardsInDisplayOrder();
            for (int i = 0; i < cards.Count; i++)
            {
                cards[i].power?.PostCardDie(deadCard);
            }
        }

        dyingCards.Clear();
        return true;
    }
    #endregion

    #region Display
    public void UpdateDisplay()
    {
        HideCards();

        //Left team starts at 3 id and goes down
        int displayID = 3;
        //Foreach left team
        foreach (CardBattle card in leftTeam.teamCards)
        {
            cardDisplays[displayID].Display(card.data);
            displayID--;
        }

        //Right Team
        displayID = 4;
        foreach (CardBattle card in rightTeam.teamCards)
        {
            cardDisplays[displayID].Display(card.data);
            displayID++;
        }
    }
    #endregion
}
