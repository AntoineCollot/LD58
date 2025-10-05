using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class CardBattleFeedback : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] int id;
    [SerializeField] AnimDir animDir;
    public enum AnimDir { Up, Down };

    [Header("FX")]
    [SerializeField] ParticleSystem attackParticles;
    [SerializeField] ParticleSystem hitParticles;
    [SerializeField] ParticleSystem powerParticles;
    [SerializeField] GameObject cardBack;

    [Header("Const")]
    //Attack
    [SerializeField] float attackDuration = 0.5f;
    [SerializeField] float attackMovement = 20;
    [SerializeField] AnimationCurve attackCurve;
    //Hit
    [SerializeField] AnimationCurve rotateCurve;
    [SerializeField] float hitRotateRange = 10;
    [SerializeField] float hitDuration = 0.5f;
    [SerializeField] float hitFreq = 4;

    float hitAmount01;
    float attackAmount01;
    RectTransform rectT;
    Vector2 originalPos;
    bool hasDied;

    void Start()
    {
        rectT = transform as RectTransform;
        originalPos = rectT.anchoredPosition;

        CardDuelManager.Instance.onActionPlayed += OnActionPlayed;
            CardDuelManager.Instance.onCardDied += OnCardDied;
            CardDuelManager.Instance.onDuelStart += OnDuelStart;
    }

    private void OnDestroy()
    {
        if (CardDuelManager.Instance != null)
        {
            CardDuelManager.Instance.onActionPlayed -= OnActionPlayed;
            CardDuelManager.Instance.onCardDied -= OnCardDied;
            CardDuelManager.Instance.onDuelStart -= OnDuelStart;
        }
    }

    private void OnDuelStart()
    {
        ResetForNewFight();
    }

    public void ResetForNewFight()
    {
        hasDied = false;
        cardBack.SetActive(false);
    }

    private void OnCardDied(CardBattle card)
    {
        if (card.GetBattlegroundID() == id)
        {
            hasDied = true;
            StartCoroutine(TurnCard());
        }
    }

    IEnumerator TurnCard()
    {
        const float TURN_AROUND_TIME = 0.4f;
        float t = 0;
        cardBack.SetActive(false);
        while (t<1)
        {
            t += Time.deltaTime/ TURN_AROUND_TIME;

            transform.localRotation = Quaternion.Euler(0, Curves.QuadEaseIn(0, 180, Mathf.Clamp01(t)), 0);

            if (t >= 0.5f && !cardBack.activeSelf)
                cardBack.SetActive(true);

            yield return null;
        }
    }

    private void OnActionPlayed(BattleAction action)
    {
        if (action.source.GetBattlegroundID() == id)
        {
            Attack(action.type);
        }
        if (action.target.GetBattlegroundID() == id)
        {
            if (action.type != BattleActionType.Heal)
                Hit(action.amount);
        }
    }

    public void Attack(BattleActionType type)
    {
        switch (type)
        {
            case BattleActionType.AttackDamage:
                //attackParticles.Play();
                attackParticles.gameObject.SetActive(true);
                break;
            case BattleActionType.PowerDamage:
                //powerParticles.Play();
               powerParticles.gameObject.SetActive(true);
                break;
        }
        attackAmount01 = 1;
    }

    public void Hit(int amount)
    {
        if (amount > 0)
        {
            //hitParticles.Play();
            hitParticles.gameObject.SetActive(true);
        }
        hitAmount01 = 1;
    }

    void Update()
    {
        if (hasDied)
            return;

        //Hit
        hitAmount01 -= Time.deltaTime / hitDuration;
        hitAmount01 = Mathf.Clamp01(hitAmount01);
        float rotate = Mathf.Sin(hitAmount01 * hitFreq) * rotateCurve.Evaluate(hitAmount01) * hitRotateRange;
        transform.localRotation = Quaternion.Euler(0, 0, rotate);

        //Attack
        attackAmount01 -= Time.deltaTime / attackDuration;
        attackAmount01 = Mathf.Clamp01(attackAmount01);
        float offset = attackMovement * attackCurve.Evaluate(attackAmount01);
        if (animDir == AnimDir.Down)
            offset *= -1;
        rectT.anchoredPosition = originalPos + Vector2.up * offset;
    }
}
