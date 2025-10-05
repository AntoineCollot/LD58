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

    bool CanBet => playerBetCard != null;

    void Start()
    {
        betButton.onClick += OnBetButton;
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
        if (hasPerformedBet)
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

        hasPerformedBet = true;
        PlayerCardCollection.PlaceBet(playerBetCard, betCard);

        HideBet();

        //Start duel after the bet is done
        CardDuelManager.Instance.StartDuelAgainstPlayer(GetComponent<CardDuelist>());
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