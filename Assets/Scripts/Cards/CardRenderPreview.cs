using UnityEngine;

public class CardRenderPreview : MonoBehaviour
{
    [SerializeField] ScriptableTGCCard card;
    CardDisplay cardDisplay;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        cardDisplay = GetComponentInChildren<CardDisplay>(true);
    }

    // Update is called once per frame
    void Update()
    {
        cardDisplay.Display(card);
    }
}
