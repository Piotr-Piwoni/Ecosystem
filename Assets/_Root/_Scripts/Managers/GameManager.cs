using System;
using Ecosystem.Managers;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameManager : StaticInstance<GameManager>
{
    public static event Action<GameState> OnBeforeStateChanged;
    public static event Action<GameState> OnAfterStateChanged;

    public GameState State { get; private set; }
    [SerializeField] private GameState _State;

    public CreatureManager m_CreatureManager;

    public void ChangeState(GameState newState)
    {
        OnBeforeStateChanged?.Invoke(newState);

        State = newState;
        switch (newState)
        {
            case GameState.Starting:
                HandleStarting();
                break;
            case GameState.Simulation:
                HandleSimulation();
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

    private void HandleSimulation()
    {
        Debug.Log("In Simulation State");
        
        m_CreatureManager.FindCreatures();
    }
}

[Serializable]
public enum GameState
{
    Starting = 0,
    Simulation = 1,
}