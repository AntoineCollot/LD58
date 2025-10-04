using UnityEngine;

[CreateAssetMenu(fileName = "Card", menuName = "ScriptableObjects/Card", order = 1)]
public class ScriptableCard : ScriptableObject
{
    public CardData data;
}

[System.Serializable]
public struct CardData
{
    [Header("Visuals")]
    public string title;
    public CardPower power;
    public Sprite artwork;

    [Header("Gameplay")]
    public int strength;
    public int hp;
}