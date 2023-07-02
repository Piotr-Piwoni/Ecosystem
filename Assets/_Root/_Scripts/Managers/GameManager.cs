using System;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameManager : StaticInstance<GameManager>
{
    public static event Action<GameState> OnBeforeStateChanged;
    public static event Action<GameState> OnAfterStateChanged;

    public GameState State { get; private set; }
    [SerializeField] private GameState _State;

    public void ChangeState(GameState newState)
    {
        OnBeforeStateChanged?.Invoke(newState);

        State = newState;
        switch (newState)
        {
            case GameState.Starting:
                HandleStarting();
                break;
            case GameState.Game:
                HandleGame();
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(newState), newState, null);
        }

        OnAfterStateChanged?.Invoke(newState);
    }

    private void HandleStarting()
    {
        Debug.Log("In Starting State");
    }

    private void HandleGame()
    {
        Debug.Log("In Game State");
    }
}

[Serializable]
public enum GameState
{
    Starting = 0,
    Game = 1,
}