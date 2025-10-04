using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardDisplay : MonoBehaviour
{
    public ScriptableCard card;

    [Header("Info")]
    [SerializeField] TextMeshProUGUI title;
    [SerializeField] TextMeshProUGUI description;
    [SerializeField] Image artworkImage;

    [Header("Stats")]
    [SerializeField] TextMeshProUGUI strength;
    [SerializeField] TextMeshProUGUI hp;

    private void Start()
    {
        Display(in card.data);
    }

    public void Display(in CardData data)
    {
        title.text = data.title;
        description.text = data.power.GetDescrition();

        strength.text = data.strength.ToString();
        hp.text = data.hp.ToString();

        artworkImage.sprite = data.artwork;
    }
}
