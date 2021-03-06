﻿using UnityEngine;
using System.Collections;

public enum GameState
{
    StartScreen,
    RandomizationScreen,
    Gameplay,
    GameOver
}

public class GameManager : DestructiveSingleton<GameManager>
{
    IEnumerator Start ()
    {
        base.Start();
        yield return null; //wait for all screen scripts to subscribe to OnStateChange and then call it
        SwitchState(GameState.StartScreen);
        OnStateChange += CheckForTimerReset;
    }
    private GameState state;

    public void SwitchState(GameState newState)
    {
        NotifyStateChange(state, newState);
    }

    public delegate void StateHandler(GameState lastState, GameState newState);
    public event StateHandler OnStateChange;

    void NotifyStateChange(GameState lastState, GameState newState)
    {
        state = newState;
        if (OnStateChange != null)
        {
            OnStateChange(lastState, newState);
        }
    }

    void CheckForTimerReset(GameState lastState, GameState newState)
    {
        if (newState == GameState.Gameplay)
        {
            timer = 0;
        }
    }

    public float timeLimit;
    private float _timer;
    public float timer
    {
        get { return _timer; }
        set
        {
            _timer = value;
            if (_timer >= timeLimit)
            {
                EndLevelTimeOut();
            }
        }
    }  

    void Update ()
    {
        if (state == GameState.Gameplay)
        {
            timer += Time.deltaTime;
            //Debug.Log(timer);
        }
    }

    [HideInInspector] public int goalScore;

    public void EndLevelGoalReached (ScoreManager winner)
    {
        EndLevel();
    }

    void EndLevelTimeOut ()
    {
        EndLevel();
    }

    public void EndLevel ()
    {
        goalScore = 0;
        Application.LoadLevel("Menu");
        Destroy(PlayerManager.instance.currentCharacter);
        SwitchState(GameState.GameOver);
    }

    public GameObject targetEffect;
    public GameObject successfullDropEffect;
    public GameObject splashEffect;
    [HideInInspector] public float score;
    [HideInInspector] public float health;
    public float timeLeft
    {
        get { return timeLimit - timer; }
    }
}
