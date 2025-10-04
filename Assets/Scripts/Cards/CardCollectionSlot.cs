using TMPro;
using UnityEngine;

public class CardCollectionSlot : MonoBehaviour
{
    public ScriptableTGCCard cardToDisplay;
    [SerializeField] TextMeshProUGUI countText;
    CardDisplay display;

    private void Start()
    {
        display = GetComponentInChildren<CardDisplay>(true);
    }

    public void Display(int count)
    {
        if (count <= 0)
        {
            countText.gameObject.SetActive(false);
            display.Hide();
            return;
        }

        countText.gameObject.SetActive(count > 1);
        countText.text = count.ToString();
        display.Display(cardToDisplay.CardData);
    }
}
