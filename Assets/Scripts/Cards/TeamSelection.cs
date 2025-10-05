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
    CardDisplay[] collectionCardDisplays;
    CardDuelist duelist;
    LookPlayerDirection lookDirection;

    bool isInTeamSelection;
    public bool isVisible => panel.activeSelf;
    List<ScriptableTGCCard> selectedCards;

    private void Awake()
    {
        collectionCardDisplays = cardCollectionGrid.GetComponentsInChildren<CardDisplay>(true);
        duelist = GetComponentInParent<CardDuelist>(true);
        lookDirection = GetComponentInParent<LookPlayerDirection>(true);
    }

    void Start()
    {
        toggleButton.onClick += OnToggleModeButton;
        showPanelButton.onClick += OnShowPanelButton;

        EnableShowObjects(false);

        UpdateDisplay();
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
        lookDirection.enabled = false;
        panel.SetActive(true);
    }

    public void Hide()
    {
        EndTeamSelection();
        lookDirection.enabled = true;
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
        else
            EnterTeamSelection();
    }

    void Update()
    {
        //Stop if we leave the collection
        if (isInTeamSelection && !CardCollectionDisplay.Instance.IsVisible)
        {
            EndTeamSelection();
            Hide();
        }
    }

    void EnterTeamSelection()
    {
        isInTeamSelection = true;
        duelist.ClearCards();

        CardDisplay.onAnyCardClick -= OnAnyCardClick;
        CardDisplay.onAnyCardClick += OnAnyCardClick;
        SetCollectionCardsInteractive(true);
        selectedCards = new();

        EnableShowObjects(true);
    }

    void EndTeamSelection()
    {
        EnableShowObjects(false);

        selectedCards = null;
        SetCollectionCardsInteractive(false);
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
        List<ScriptableTGCCard> duelistTeam = duelist.Cards;
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

    void SetCollectionCardsInteractive(bool value)
    {
        foreach (var display in collectionCardDisplays)
        {
            display.isInteractive = value;
        }
    }
}
