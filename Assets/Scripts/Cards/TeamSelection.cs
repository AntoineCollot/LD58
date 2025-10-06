using System;
using System.Collections.Generic;
using UnityEngine;

public class TeamSelection : MonoBehaviour
{
    [SerializeField] GameObject panel;
    [SerializeField] CardDisplay[] teamDisplays;
    [SerializeField] Transform cardCollectionGrid;
    [SerializeField] WorldUIButton toggleButton;
    [SerializeField] WorldUIButton showPanelButton;
    [SerializeField] GameObject[] toEnableDuringTeamSelection;
    CardDuelist duelist;

    public bool isInTeamSelection { get; private set; }
    public bool isVisible => panel.activeSelf;
    List<ScriptableTGCCard> selectedCards;

    public static int playerTeamSize { get; private set; }

    private void Awake()
    {
        duelist = GetComponentInParent<CardDuelist>(true);
        playerTeamSize = 0;
    }

    private void OnEnable()
    {
        if (NPCBet.isInAnyBetSelect && isVisible)
            Hide();
    }

    void Start()
    {
        toggleButton.onClick += OnToggleModeButton;
        showPanelButton.onClick += OnShowPanelButton;

        EnableShowObjects(false);

        UpdateDisplay();
        Hide();
    }

    private void OnDisable()
    {
        Hide();
    }

    private void OnDestroy()
    {
        if (toggleButton != null)
            toggleButton.onClick -= OnToggleModeButton;
        if (showPanelButton != null)
            showPanelButton.onClick -= OnShowPanelButton;
    }

    void EnableShowObjects(bool value)
    {
        foreach (GameObject obj in toEnableDuringTeamSelection)
        {
            obj.SetActive(value);
        }
    }

    public void Show()
    {
        panel.SetActive(true);
    }

    public void Hide(bool doNotResetCardInteractivity = false)
    {
        EndTeamSelection(doNotResetCardInteractivity);
        if (panel != null)
            panel.SetActive(false);
    }

    private void OnShowPanelButton()
    {
        if (isVisible)
            Hide();
        else
            Show();
    }

    private void OnToggleModeButton()
    {
        if (isInTeamSelection)
            EndTeamSelection();
        else if (!NPCBet.isInAnyBetSelect)
            EnterTeamSelection();
    }

    void Update()
    {
        if (isVisible && NPCBet.isInAnyBetSelect)
        {
            Hide(true);
        }
        //Stop if we leave the collection
        else if (isInTeamSelection && !CardCollectionDisplay.Instance.IsVisible)
        {
            EndTeamSelection();
            Hide();
        }

        showPanelButton.gameObject.SetActive(!NPCBet.isInAnyBetSelect);
    }

    void EnterTeamSelection()
    {
        isInTeamSelection = true;
        if (duelist != null)
            duelist.ClearCards();

        UpdateDisplay();

        CardDisplay.onAnyCardClick -= OnAnyCardClick;
        CardDisplay.onAnyCardClick += OnAnyCardClick;
        selectedCards = new();
        CardCollectionDisplay.Instance.SetCardsInteractive(true);

        EnableShowObjects(true);
    }

    void EndTeamSelection(bool doNotResetCardInteractivity = false)
    {
        EnableShowObjects(false);

        selectedCards = null;
        if (!doNotResetCardInteractivity && Time.time>1)
            CardCollectionDisplay.Instance.SetCardsInteractive(false);
        isInTeamSelection = false;
        CardDisplay.onAnyCardClick -= OnAnyCardClick;
    }

    void SelectCard(ScriptableTGCCard card)
    {
        if (!isInTeamSelection)
            return;

        if (!PlayerCardCollection.HasCard(card))
            return;

        if (selectedCards.Contains(card))
            return;

        selectedCards.Add(card);
        duelist.SetCards(selectedCards);
        playerTeamSize = selectedCards.Count;
        UpdateDisplay();

        if (selectedCards.Count >= CardDuelist.MAX_TEAM_SIZE)
            EndTeamSelection();
    }

    private void OnAnyCardClick(ScriptableTGCCard card)
    {
        SelectCard(card);
    }

    void UpdateDisplay()
    {
        HideAllDisplays();
        List<ScriptableTGCCard> duelistTeam = duelist.CardsAsPlayer;
        for (int i = 0; i < Mathf.Min(duelistTeam.Count, teamDisplays.Length); i++)
        {
            teamDisplays[i].Display(duelistTeam[i]);
        }
    }

    void HideAllDisplays()
    {
        for (int i = 0; i < teamDisplays.Length; i++)
        {
            teamDisplays[i].Hide();
        }
    }
}
