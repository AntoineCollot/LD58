using UnityEngine;

public class LostCard : MonoBehaviour
{
    [SerializeField] ScriptableTGCCard card;
    [SerializeField] WorldUIButton pickUpButton;

    void Start()
    {
        pickUpButton.onClick += OnPickUpButton;
        GetComponentInChildren<CardDisplay>(true).Display(card);
    }

    void OnDestroy()
    {
        if (pickUpButton != null)
            pickUpButton.onClick -= OnPickUpButton;
    }


    private void OnPickUpButton()
    {
        if (!gameObject.activeSelf)
            return;
        PickUp();
    }

    public void PickUp()
    {
        PlayerCardCollection.AddCard(card);
        gameObject.SetActive(false);
    }
}
