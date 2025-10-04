using System;
using System.Collections.Generic;
using UnityEngine;

public static class PlayerCardCollection
{
    public static bool HasBet => betSacrificeCard != null && betRewardCard != null;
    public static ScriptableTGCCard betSacrificeCard;
    public static ScriptableTGCCard betRewardCard;

    public static Dictionary<ScriptableTGCCard, int> collection;

    public static event Action onCollectionUpdated;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void Init()
    {
        collection = new Dictionary<ScriptableTGCCard, int>();
    }

    public static void AddCard(ScriptableTGCCard card)
    {
        if (collection.ContainsKey(card))
        {
            collection[card]++;
        }
        else
        {
            collection.Add(card, 1);
        }

        onCollectionUpdated?.Invoke();
    }

    public static bool TryRemoveCard(ScriptableTGCCard card)
    {
        if (!collection.TryGetValue(card, out int count))
            return false;

        if (count <= 0)
            return false;

        collection[card]--;
        onCollectionUpdated?.Invoke();

        return true;
    }

    public static int GetCardCount(ScriptableTGCCard card)
    {
        if (!collection.TryGetValue(card, out int count))
            return 0;

        return count;
    }

    public static bool HasCard(ScriptableTGCCard card)
    {
        return GetCardCount(card) > 0;
    }

    public static bool Trade(ScriptableTGCCard receiveCard, ScriptableTGCCard giveCard)
    {
        if (!TryRemoveCard(giveCard))
            return false;

        AddCard(receiveCard);
        return true;
    }

    public static void PlaceBet(ScriptableTGCCard sacrificeCard, ScriptableTGCCard rewardCard)
    {
        betSacrificeCard = sacrificeCard;
        betRewardCard = rewardCard;
    }

    public static bool PerformBet(bool playerVictory)
    {
        if (!HasBet)
            return false;

        bool success = true;
        if (playerVictory)
            AddCard(betRewardCard);
        else
            success = TryRemoveCard(betSacrificeCard);

        ClearBet();
        return success;
    }

    public static void ClearBet()
    {
        betSacrificeCard = null;
        betRewardCard = null;
    }
}