using UnityEngine;

public class CardTrade : MonoBehaviour, INPCSelectable
{
    [Header("Trade")]
    [SerializeField] ScriptableTGCCard lookingForCard;
    [SerializeField] ScriptableTGCCard giveCard;

    [Header("UI")]
    [SerializeField] GameObject panel;
    [SerializeField] CardDisplay lookingForCardDisplay;
    [SerializeField] CardDisplay giveDisplay;
    [SerializeField] WorldUIButton performTradeButton;

    bool IsVisible => panel.activeSelf;
    bool hasPerformedTrade;

    Animator anim;

    void Start()
    {
        anim = GetComponentInChildren<Animator>(true);
        performTradeButton.onClick += OnPerformTradeClicked;

        HideTrade();
    }

    void OnDestroy()
    {
        if (performTradeButton != null)
            performTradeButton.onClick -= OnPerformTradeClicked;
    }

    void Update()
    {
        if (!IsVisible)
            return;

        performTradeButton.gameObject.SetActive(PlayerCardCollection.HasCard(lookingForCard));
    }

    private void OnPerformTradeClicked()
    {
        if (IsVisible)
            PerformTrade();
    }

    public void ShowTrade()
    {
        if (hasPerformedTrade)
            return;
        panel.SetActive(true);
        lookingForCardDisplay.Display(lookingForCard);
        giveDisplay.Display(giveCard);
    }

    public void HideTrade()
    {
        panel.SetActive(false);
    }

    public void PerformTrade()
    {
        if (hasPerformedTrade)
            return;

        if (PlayerCardCollection.Trade(giveCard, lookingForCard))
            hasPerformedTrade = true;
        HideTrade();

        if (anim != null)
            anim.SetTrigger("Happy");
    }

    public void HoverEnterMessage()
    {
        ShowTrade();
    }

    public void HoverExitMessage()
    {
        HideTrade();
    }

    public void NPCSelectMessage()
    {
    }
}
