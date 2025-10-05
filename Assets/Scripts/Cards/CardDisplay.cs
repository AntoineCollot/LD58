using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardDisplay : MonoBehaviour,IWorldUISelectable
{
    ScriptableTGCCard currentCard;

    [Header("Info")]
    [SerializeField] TextMeshProUGUI title;
    [SerializeField] TextMeshProUGUI description;
    [SerializeField] Image artworkImage;

    [Header("Stats")]
    [SerializeField] TextMeshProUGUI strength;
    [SerializeField] TextMeshProUGUI hp;

    [Header("Click")]
    public bool isInteractive;
    [SerializeField] Graphic highlightOverlay;
    public event Action<ScriptableTGCCard> onClick;
    public static event Action<ScriptableTGCCard> onAnyCardClick;

    public void Display(ScriptableTGCCard card)
    {
        Display(card.CardData);
        currentCard = card;
    }

    public void Display(CardData data)
    {
        currentCard = null;
        gameObject.SetActive(true);
        title.text = data.cardName;
        description.text = data.power.GetDescrition();

        strength.text = data.strength.ToString();
        hp.text = Mathf.Max(0,data.hp).ToString();

        artworkImage.sprite = data.artwork;
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void OnClick()
    {
        if (!isInteractive)
            return;

        onClick?.Invoke(currentCard);
        onAnyCardClick?.Invoke(currentCard);

        SFXManager.PlaySound(GlobalSFX.CardSelect);
    }

    public void OnHoverEnter()
    {
        if (!isInteractive)
            return;
        highlightOverlay.gameObject.SetActive(true);
    }

    public void OnHoverExit()
    {
        highlightOverlay.gameObject.SetActive(false);
    }
}
