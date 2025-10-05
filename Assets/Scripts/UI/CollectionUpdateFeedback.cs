using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CollectionUpdateFeedback : MonoBehaviour
{
    [SerializeField] GameObject panel;
    [SerializeField] Graphic cardGraphic;
    [SerializeField] CanvasGroup canvasGroup;
    [SerializeField] CardDisplay renderCardDisplay;
    Material instancedMat;
    Queue<CollectionUpdate> collectionUpdates = new();

    float cardOriginalScale;
    const string DISSOLVE_PROPERTY = "_Dissolve";
    const string SHINE_PROPERTY = "_Shine";

    bool isInAnim;
    bool IsVisible => panel.activeSelf;

    struct CollectionUpdate
    {
        public ScriptableTGCCard card;
        public PlayerCardCollection.UpdateType type;

        public CollectionUpdate(ScriptableTGCCard card, PlayerCardCollection.UpdateType update)
        {
            this.card = card;
            this.type = update;
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        instancedMat = new Material(cardGraphic.material);
        cardGraphic.material = instancedMat;
        cardOriginalScale = cardGraphic.transform.localScale.x;

        PlayerCardCollection.onCollectionUpdated += OnCollectionUpdated;
        panel.SetActive(false);
    }

    private void OnDestroy()
    {
        PlayerCardCollection.onCollectionUpdated -= OnCollectionUpdated;
    }

    private void Update()
    {
        if (isInAnim || !IsVisible)
            return;

        if (collectionUpdates.TryDequeue(out var collectionUpdate))
        {
            StartCoroutine(Anim(collectionUpdate));
            return;
        }

        panel.SetActive(false);
    }

    private void OnCollectionUpdated(ScriptableTGCCard card, PlayerCardCollection.UpdateType update)
    {
        panel.SetActive(true);
        CollectionUpdate collectionUpdate = new CollectionUpdate(card, update);
        if (!isInAnim)
            StartCoroutine(Anim(collectionUpdate));
        else
            collectionUpdates.Enqueue(collectionUpdate);
    }

    IEnumerator Anim(CollectionUpdate updateData)
    {
        renderCardDisplay.Display(updateData.card);
        isInAnim = true;
        cardGraphic.transform.localScale = Vector3.zero;

        instancedMat.SetFloat(DISSOLVE_PROPERTY, 0);
        instancedMat.SetFloat(SHINE_PROPERTY, 0);

        //Show background
        yield return StartCoroutine(Fade(0, 1, 0.2f));

        //Show card
        yield return StartCoroutine(ScaleCard(0.3f));

        yield return new WaitForSeconds(0.5f);
        //Effect
        yield return StartCoroutine(Effect(1f, updateData.type));

        yield return new WaitForSeconds(0.5f);
        //Hide
        yield return StartCoroutine(Fade(1, 0, 0.2f));
        isInAnim = false;
    }

    IEnumerator Fade(float from, float to, float time)
    {
        float t = 0;
        while (t < 1)
        {
            t += Time.deltaTime / time;

            canvasGroup.alpha = Mathf.Lerp(from, to, t);

            yield return null;
        }
    }

    IEnumerator ScaleCard(float time)
    {
        float t = 0;
        while (t < 1)
        {
            t += Time.deltaTime / time;

            cardGraphic.transform.localScale = Vector3.one * Curves.BackEaseOut(0, cardOriginalScale, Mathf.Clamp01(t));

            yield return null;
        }
    }

    IEnumerator Effect(float time, PlayerCardCollection.UpdateType update)
    {
        float t = 0;

        int propertyID;
        switch (update)
        {
            case PlayerCardCollection.UpdateType.Added:
            case PlayerCardCollection.UpdateType.Mixed:
            default:
                propertyID = Shader.PropertyToID(SHINE_PROPERTY);
                break;
            case PlayerCardCollection.UpdateType.Removed:
                propertyID = Shader.PropertyToID(DISSOLVE_PROPERTY);
                time *= 2;
                break;
        }

        while (t < 1)
        {
            t += Time.deltaTime / time;

            instancedMat.SetFloat(propertyID, Curves.QuadEaseInOut(0, 1, Mathf.Clamp01(t)));

            yield return null;
        }
    }
}
