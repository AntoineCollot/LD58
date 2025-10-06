using UnityEngine;

//After slots
[DefaultExecutionOrder(10)]
public class CardCollectionDisplay : MonoBehaviour
{
    public static CardCollectionDisplay Instance;
    [SerializeField] GameObject panel;
    CardCollectionSlot[] slots;
    public bool IsVisible => panel.activeSelf;
    LookPlayerDirection lookDirection;

    bool areCardInteractive;

    InputMap inputMap;

#if UNITY_EDITOR
    [SerializeField] bool spawnWithEverything;
#endif

    void Awake()
    {
        Instance = this;
        lookDirection = GetComponentInParent<LookPlayerDirection>(true);
        slots = GetComponentsInChildren<CardCollectionSlot>(true);

        inputMap = new InputMap();
        inputMap.Enable();
        inputMap.Main.Collection.performed += OnCollectionKeyPerformed;

#if UNITY_EDITOR
        if(spawnWithEverything)
        {
            ScriptableTGCCard[] cards = Resources.LoadAll<ScriptableTGCCard>("Cards");
            foreach (ScriptableTGCCard card in cards)
            {
                PlayerCardCollection.AddCard(card);
            }
        }
#endif
        PlayerCardCollection.onCollectionUpdated += OnCollectionUpdated;
    }

    private void OnDestroy()
    {
        PlayerCardCollection.onCollectionUpdated -= OnCollectionUpdated;
        if (inputMap != null)
        {
            inputMap.Main.Collection.performed -= OnCollectionKeyPerformed;
            inputMap.Disable();
            inputMap.Dispose();
        }
    }

    private void Start()
    {
        Hide();
        UpdateDisplay();
    }

    private void Update()
    {
        lookDirection.enabled = !areCardInteractive || !IsVisible;
    }

    private void OnCollectionUpdated(ScriptableTGCCard card, PlayerCardCollection.UpdateType update)
    {
        UpdateDisplay();
    }

    private void OnCollectionKeyPerformed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        Toggle();
    }

    public void Toggle()
    {
        if (IsVisible)
            Hide();
        else
            Show();
    }

    public void Show()
    {
        if (NPCBet.isInAnyBetSelect)
        {
            SetCardsInteractive(true);
        }
        panel.SetActive(true);
        SFXManager.PlaySound(GlobalSFX.OpenBook);
    }

    public void Hide()
    {
        SFXManager.PlaySound(GlobalSFX.CloseBook);
        panel.SetActive(false);
    }

    public void UpdateDisplay()
    {
        foreach (var slot in slots)
        {
            slot.Display(PlayerCardCollection.GetCardCount(slot.cardToDisplay));
        }
    }

    public void SetCardsInteractive(bool value)
    {
        foreach (var slot in slots)
        {
            slot.SetCardInteractive(value);
        }
        areCardInteractive = value;
    }
}
