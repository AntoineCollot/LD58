using System;
using System.Collections;
using UnityEngine;

public class NPCBet : MonoBehaviour, INPCSelectable
{
    [SerializeField] ScriptableTGCCard betCard;
    ScriptableTGCCard playerBetCard;

    bool hasPerformedBet;
    bool isSelected;
    public static bool isInAnyBetSelect { get; private set; } 

    [Header("Display")]
    [SerializeField] GameObject panel;
    [SerializeField] CardDisplay betCardDisplay;
    [SerializeField] CardDisplay playerBetCardDisplay;
    [SerializeField] WorldUIButton betButton;

    float nextAvailableTime;
    const float CAN_BET_AGAIN_TIME = 60;
    bool IsInteractive => Time.time > nextAvailableTime;

    bool CanBet => playerBetCard != null && TeamSelection.playerTeamSize >0;

    void Start()
    {
        betButton.onClick += OnBetButton;
        panel.SetActive(false);
        isInAnyBetSelect = false;
    }

    private void OnDestroy()
    {
        if(betButton!=null)
            betButton.onClick -= OnBetButton;

        CardDisplay.onAnyCardClick -= OnCardSelected;
    }

    void Update()
    {
        betButton.gameObject.SetActive(CanBet);
    }

    public void ShowBet()
    {
        if (!IsInteractive)
            return;

        panel.SetActive(true);
        betCardDisplay.Display(betCard);
        playerBetCardDisplay.Hide();

        CardCollectionDisplay.Instance.SetCardsInteractive(true);
        CardDisplay.onAnyCardClick += OnCardSelected;

        isInAnyBetSelect = true;
    }

    public void HideBet()
    {
        CardDisplay.onAnyCardClick -= OnCardSelected;
        CardCollectionDisplay.Instance.SetCardsInteractive(false);

        playerBetCard = null;
        panel.SetActive(false);
        isInAnyBetSelect = false;
    }

    void PlaceBet()
    {
        if (!CanBet)
            return;

        nextAvailableTime = Time.time + CAN_BET_AGAIN_TIME;
        hasPerformedBet = true;
        PlayerCardCollection.PlaceBet(playerBetCard, betCard);

        HideBet();

        //Start duel after the bet is done
        CardDuelManager.Instance.StartDuelAgainstPlayer(GetComponent<CardDuelist>());

        SFXManager.PlaySound(GlobalSFX.Bet);
    }

    private void OnCardSelected(ScriptableTGCCard card)
    {
        playerBetCard = card;
        playerBetCardDisplay.Display(card);
    }

    void OnBetButton()
    {
        PlaceBet();
    }

    public void HoverEnterMessage()
    {
        isSelected = true;
        ShowBet();
    }

    public void HoverExitMessage()
    {
       HideBet();
    }

    public void NPCSelectMessage()
    {
    }
}