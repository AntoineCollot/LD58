using System;
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
    Queue<BattleAction> actionQueue = new Queue<BattleAction>();
    public event Action<BattleAction> onActionPlayed;
    public event Action<CardBattle> onCardDied;
    public event Action onDuelStart;
    public event Action onDuelEnd;

    [Header("Display")]
    [SerializeField] CardDisplay[] cardDisplays;
    [SerializeField] Transform panel;
    const float TURN_ACTION_DURATION = 0.8f;

    //player
    CompositeStateToken freezePlayerToken;

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        freezePlayerToken = new();
        PlayerState.Instance.freezeMoveState.Add(freezePlayerToken);
        PlayerState.Instance.freezeClickState.Add(freezePlayerToken);

        panel.gameObject.SetActive(false);

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
        //place in between
        Vector3 targetPos = (player.transform.position + opponent.transform.position) * 0.5f;
        targetPos.y = panel.position.y;
        panel.position = targetPos;

        //Look at player
        Vector3 lookAt = player.transform.position;
        lookAt.y = transform.position.y;
        panel.LookAt(lookAt);

        opponent.EnterDuel();

        StartDuel(new CardBattleTeam(player.Cards.ToArray(), TeamDir.Left), new CardBattleTeam(opponent.Cards.ToArray(), TeamDir.Right));
    }

    void StartDuel(CardBattleTeam player, CardBattleTeam opponent)
    {
        panel.gameObject.SetActive(true);
        StartCoroutine(Duel(player, opponent));
    }

    IEnumerator Duel(CardBattleTeam player, CardBattleTeam opponent)
    {
        onDuelStart?.Invoke();
        freezePlayerToken.SetOn(true);

        int turns = 0;
        const int MAX_TURNS = 30;
        bool victory = false;

        leftTeam = player;
        rightTeam = opponent;

        UpdateDisplay();

        yield return new WaitForSeconds(1);

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

        yield return new WaitForSeconds(1);

        onDuelEnd?.Invoke();
        freezePlayerToken.SetOn(false);
        HideCards();
        panel.gameObject.SetActive(false);
    }

    IEnumerator PlayTurn(CardBattleTeam team, CardBattleTeam other)
    {
        CardBattle playingCard = team.GetFirstCard();
        CardBattle targetCard = other.GetFirstCard();

        BattleAction attackAction = new BattleAction(playingCard, targetCard, BattleActionType.AttackDamage, playingCard.data.strength);
        RegisterAction(attackAction);

        const int MAX_LOOPS = 30;
        int loops = 0;
        while (TryProcessNextAction(out bool shouldWait) && loops < MAX_LOOPS)
        {
            loops++;
            UpdateDisplay();

            if (shouldWait)
                yield return new WaitForSeconds(TURN_ACTION_DURATION);
        }

        //First card of the playing team attacks the first of opposite team
        //playingCard.AttackCard(targetCard);

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
                for (int j = i+1; j < cards.Count; j++)
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
            onCardDied?.Invoke(deadCard);
        }

        dyingCards.Clear();
        return true;
    }

    public void RegisterAction(CardBattle source, CardBattle target, BattleActionType action, int amount, bool isMultiAction = false)
    {
        RegisterAction(new BattleAction(source, target, action, amount, isMultiAction));
    }

    public void RegisterAction(BattleAction action)
    {
        actionQueue.Enqueue(action);
    }

    bool TryProcessNextAction(out bool shouldWait)
    {
        shouldWait = true;
        if (!actionQueue.TryDequeue(out BattleAction action))
            return false;

        //Pre callbacks
        action.source.power?.PreAction(action);
        action.target.power?.PreAction(action);

        var cards = GetAllCardsInDisplayOrder();
        for (int i = 0; i < cards.Count; i++)
        {
            cards[i].power?.PreAnyAction(action);
        }

        //Apply damages
        switch (action.type)
        {
            case BattleActionType.AttackDamage:
            case BattleActionType.PowerDamage:
                action.target.Damage(action.amount);
                break;
            case BattleActionType.Heal:
                action.target.Heal(action.amount);
                break;
            default:
                throw new NotImplementedException();
        }

        shouldWait = !action.isMultiAction;
        onActionPlayed?.Invoke(action);

        //post callbacks
        action.source.power?.PostAction(action);
        action.target.power?.PostAction(action);

        for (int i = 0; i < cards.Count; i++)
        {
            cards[i].power?.PostAnyAction(action);
        }

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

public enum TeamDir { Left, Right }
public enum BattleActionType { AttackDamage, PowerDamage, Heal }
public class BattleAction
{
    public CardBattle source;
    public CardBattle target;
    public BattleActionType type;
    public int amount;
    public bool isMultiAction;

    public BattleAction()
    {
    }

    public BattleAction(CardBattle source, CardBattle target, BattleActionType type, int amount, bool isMultiAction = false)
    {
        this.source = source;
        this.target = target;
        this.type = type;
        this.amount = amount;
        this.isMultiAction = isMultiAction;
    }
}