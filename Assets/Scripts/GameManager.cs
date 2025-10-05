using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    public bool gameIsOver { get; private set; }
    public bool gameHasStarted { get; private set; }
    public bool GameIsPlaying => !gameIsOver && gameHasStarted;
    public bool autoStart = true;

    public UnityEvent onGameStart = new UnityEvent();
    public UnityEvent onGameOver = new UnityEvent();
    public UnityEvent onGameWin = new UnityEvent();

    public static GameManager Instance;

    void Awake()
    {
        Instance = this;
        if (autoStart)
            StartGame();
    }

    private void Start()
    {
        PlayerCardCollection.onCollectionUpdated += OnCollectionUpdate;
    }

    private void OnDestroy()
    {
        PlayerCardCollection.onCollectionUpdated -= OnCollectionUpdate;
    }

    private void OnCollectionUpdate(ScriptableTGCCard card, PlayerCardCollection.UpdateType type)
    {
        if (!gameIsOver && PlayerCardCollection.HasAllCards())
            ClearLevel();
    }

    public void StartGame()
    {
        if (gameHasStarted)
            return;
        gameHasStarted = true;
        onGameStart.Invoke();
    }

    public void GameOver()
    {
        if (gameIsOver)
            return;
        gameIsOver = true;
        onGameOver.Invoke();
    }

    public void ClearLevel()
    {
        if (gameIsOver)
            return;

        gameIsOver = true;
        onGameWin.Invoke();
        SFXManager.PlaySound(GlobalSFX.GameWin);
    }
}
