using TMPro;
using UnityEngine;

public class CardCollectionSlot : MonoBehaviour
{
    public ScriptableTGCCard cardToDisplay;
    [Header("UI")]
    [SerializeField] TextMeshProUGUI countText;
    [SerializeField] GameObject shadow;
    CardDisplay display;

    private void Awake()
    {
        display = GetComponentInChildren<CardDisplay>(true);
    }

    public void Display(int count)
    {
        if (count <= 0)
        {
            countText.gameObject.SetActive(false);
            display.Hide();
            shadow.SetActive(false);
            return;
        }

        shadow.SetActive(true);
        countText.gameObject.SetActive(count > 1);
        countText.text = "x" + count.ToString();
        display.Display(cardToDisplay);
    }

    public void SetCardInteractive(bool value)
    {
        display.isInteractive = value;
    }
}
