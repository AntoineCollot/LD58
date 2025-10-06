using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RandomLostCardSpawner : MonoBehaviour
{
    ScriptableTGCCard[] cards;
    [SerializeField] LostCard lostCardPrefab;
    [SerializeField] Transform[] spawns;
    [SerializeField] float intervalTime = 30;
    List<Transform> availableSpawns;

    void Start()
    {
        cards = Resources.LoadAll<ScriptableTGCCard>("Cards");
        cards = cards.Where(c => !c.isRare).ToArray();

        RefreshSpawns();

        StartCoroutine(SpawnLostCards());
    }

    IEnumerator SpawnLostCards()
    {
        while(!GameManager.Instance.gameHasStarted)
            yield return null;

        while (true)
        {
            yield return new WaitForSeconds(intervalTime);
            SpawnCard();
        }
    }

    void RefreshSpawns()
    {
        availableSpawns = new List<Transform>();
        availableSpawns.AddRange(spawns);
    }

    public void SpawnCard()
    {
        Transform pos = GetSpawnPosOutOfPlayerView();
        LostCard card = Instantiate(lostCardPrefab, pos.position, pos.rotation, transform);
        card.SetCard(cards[Random.Range(0, cards.Length)]);
    }

    Transform GetSpawnPosOutOfPlayerView()
    {
        if (availableSpawns.Count < 3)
            RefreshSpawns();

        Transform candidate;
        int tries = 0;
        const int MAX_TRIES = 30;
        do
        {
            tries++;
            candidate = availableSpawns[Random.Range(0, availableSpawns.Count)];
        } while (IsPositionInPlayerView(candidate.position) && tries < MAX_TRIES);

        availableSpawns.Remove(candidate);

        if (tries > 10)
            RefreshSpawns();

        return candidate;
    }

    bool IsPositionInPlayerView(in Vector3 pos)
    {
        Vector3 fromPlayer = pos - PlayerState.Instance.transform.position;
        return Vector3.Dot(Camera.main.transform.forward, fromPlayer) > 0 && fromPlayer.magnitude < 8;
    }
}
