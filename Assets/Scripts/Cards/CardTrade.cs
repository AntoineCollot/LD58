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
    float nextAvailableTime;
    const float CAN_BE_TRADED_AGAIN_TIME = 60;
    bool CanTrade => Time.time>nextAvailableTime;

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
        if (!CanTrade)
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
        if (!CanTrade)
            return;

        if (PlayerCardCollection.Trade(giveCard, lookingForCard))
        {
            hasPerformedTrade = true;
            nextAvailableTime = Time.time + CAN_BE_TRADED_AGAIN_TIME;
        }
        HideTrade();

        if (anim != null)
            anim.SetTrigger("Happy");

        SFXManager.PlaySound(GlobalSFX.Trade);
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
