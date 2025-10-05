using UnityEngine;

public class CardCollectionDisplay : MonoBehaviour
{
    public static CardCollectionDisplay Instance;
    [SerializeField] GameObject panel;
    CardCollectionSlot[] slots;
    public bool IsVisible => panel.activeSelf;

    InputMap inputMap;

    void Awake()
    {
        Instance = this;
        slots = GetComponentsInChildren<CardCollectionSlot>(true);

        inputMap = new InputMap();
        inputMap.Enable();
        inputMap.Main.Collection.performed += OnCollectionKeyPerformed;

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
        UpdateDisplay();
    }

    private void OnCollectionUpdated()
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
        panel.SetActive(true);
    }

    public void Hide()
    {
        panel.SetActive(false);
    }

    public void UpdateDisplay()
    {
        foreach (var slot in slots)
        {
            slot.Display(PlayerCardCollection.GetCardCount(slot.cardToDisplay));
        }
    }
}
