using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardDisplay : MonoBehaviour
{
    [Header("Info")]
    [SerializeField] TextMeshProUGUI title;
    [SerializeField] TextMeshProUGUI description;
    [SerializeField] Image artworkImage;

    [Header("Stats")]
    [SerializeField] TextMeshProUGUI strength;
    [SerializeField] TextMeshProUGUI hp;

    public void Display(in CardData data)
    {
        gameObject.SetActive(true);
        title.text = data.cardName;
        description.text = data.power.GetDescrition();

        strength.text = data.strength.ToString();
        hp.text = data.hp.ToString();

        artworkImage.sprite = data.artwork;
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
