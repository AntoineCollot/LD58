using UnityEngine;

public class LostCard : MonoBehaviour
{
    [SerializeField] ScriptableTGCCard card;
    [SerializeField] WorldUIButton pickUpButton;
    public bool hasBeenPickedUp { get; private set; }

    void Start()
    {
        pickUpButton.onClick += OnPickUpButton;
        CardDisplay display = GetComponentInChildren<CardDisplay>(true);
        if(display!=null)
            display.Display(card);
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
        hasBeenPickedUp = true;

        SFXManager.PlaySound(GlobalSFX.PickUpCard);
    }
}
