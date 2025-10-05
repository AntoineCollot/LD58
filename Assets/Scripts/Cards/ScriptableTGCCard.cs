using UnityEngine;

[CreateAssetMenu(fileName = "Card", menuName = "ScriptableObjects/Card", order = 1)]
public class ScriptableTGCCard : ScriptableObject
{
    [Header("Info")]
    public string cardName;
    public Sprite artwork;
    public bool isRare;

    [Header("Stats")]
    public CardPower power;
    public int strength;
    public int hp;

    public CardData CardData
    {
        get
        {
            CardData data = new CardData();
            data.artwork = artwork;
            data.cardName = cardName;
            data.power = power;
            data.strength = strength;
            data.hp = hp;
            return data;
        }
    }
}

[System.Serializable]
public struct CardData
{
    public string cardName;
    public CardPower power;
    public Sprite artwork;

    public int strength;
    public int hp;
}