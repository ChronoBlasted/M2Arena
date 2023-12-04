using BaseTemplate.Behaviours;
using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameState { MENU, GAME }

public class GameManager : MonoSingleton<GameManager>
{
    public event Action<GameState> OnGameStateChanged;
    GameState _gameState;

    public GameState GameState { get => _gameState; }

    void Awake()
    {
        UIManager.Instance.Init();

        PlayerController.Instance.Init();

        NakamaManager.Instance.Init();
    }

    void Update()
    {
        if (Input.GetKeyUp(KeyCode.R))
        {
            ReloadScene();
        }
    }

    public void UpdateGameState(GameState newState)
    {
        _gameState = newState;

        Debug.Log("New GameState : " + _gameState);

        switch (_gameState)
        {
            case GameState.MENU:
                HandleMenu();
                break;
            case GameState.GAME:
                HandleGame();
                break;
            default:
                break;
        }

        OnGameStateChanged?.Invoke(_gameState);
    }

    void HandleMenu()
    {
    }

    void HandleGame()
    {
    }

    public void UpdateStateToMenu() => UpdateGameState(GameState.MENU);
    public void UpdateStateToGame() => UpdateGameState(GameState.GAME);

    public void ReloadScene()
    {
        DOTween.KillAll();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void QuitApp() => Application.Quit();
}