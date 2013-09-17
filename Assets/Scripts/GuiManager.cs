/**
 * The Legend of Epikouros
 *   ~ a game made in 48h by Erhune for Ludum Dare 27
 *   
 * Copyright (c) 2013 Erhune <erhune@gmail.com>
 * 
 * Feel free to contact me any time!
 */
using System;
using UnityEngine;


public class GuiManager : GameComponent
{

    public UILabel Nickname;
    public UILabel Timer;
    public UILabel GameTime;
    public UILabel Bombs;
    public UILabel Keys;
    public UILabel Panel;
    public UILabel Status;
    public Transform Life;
    public GameObject FullHeartPrefab;
    public GameObject HalfHeartPrefab;
    public GameObject EmptyHeartPrefab;

    private int currentLife;
    private int currentMaxLife;

    void LateUpdate()
    {
        // Status
        switch (gameManager.state)
        {
            case PlayState.Playing:
            case PlayState.None:
                Status.text = "";
                break;

            case PlayState.Replaying:
                Status.text = "The story so far...";
                break;

            case PlayState.WaitingForInput:
                Status.text = "Press Space to play your 10 seconds.";
                break;

            case PlayState.RequestToStart:
                Status.text = "Starting in...";
                break;

            case PlayState.StartCountdown:
                Status.text = string.Format("Starting in... {0}", Mathf.FloorToInt(GameManager.CountdownDuration - (Time.time - gameManager.startedCountdownAt)));
                break;

            case PlayState.PostingRun:
                Status.text = "You did your part in this story.";
                break;

            case PlayState.Ended:
            case PlayState.PollingForMore:
                Status.text = "And thus, the story unfolds...";
                break;

            default:
                throw new ArgumentOutOfRangeException();
        }

        // Nickname
        Nickname.text = gameManager.hero.nickname;

        // Timer
        var remainingTime = TimeSpan.FromSeconds(gameManager.remainingTime);
        Timer.text = string.Format("{0}.{1:000}", remainingTime.Seconds, remainingTime.Milliseconds);
        if (gameManager.remainingTime == 0)
        {
            Timer.color = Color.red;
        }
        else if (gameManager.remainingTime < GameManager.RunDuration)
        {
            Timer.color = Color.white;
        }
        else
        {
            Timer.color = Color.blue;
        }

        // Game time
        var gameTime = TimeSpan.FromSeconds(gameManager.currentTime);
        GameTime.text = string.Format("{0}:{1:00}.{2:000}", Mathf.FloorToInt((float)gameTime.TotalMinutes), gameTime.Seconds, gameTime.Milliseconds);

        // Consumables
        Bombs.text = string.Format("x{0}", gameManager.hero.Bombs);
        Keys.text = string.Format("x{0}", gameManager.hero.Keys);

        // Panel
        Panel.text = (gameManager.hero.Panel ?? "").Replace("\\n", "\n");

        // Life
        if (currentLife != gameManager.hero.Life || currentMaxLife != gameManager.hero.MaxLife)
        {
            Life.DestroyAllChildren();
            var heartCount = Mathf.CeilToInt(gameManager.hero.MaxLife / 2f); // ex: life=3/8, heartCount = 4
            var delta = new Vector3(0, (heartCount <= 6) ? -16 : 0);
            for (int i = 0; i < heartCount; i++)
            {
                GameObject prefab;
                if (2 * i + 1 > gameManager.hero.Life) // ex: i=2,3
                {
                    prefab = EmptyHeartPrefab;
                }
                else if (2 * i + 1 < gameManager.hero.Life) // ex: i=0
                {
                    prefab = FullHeartPrefab;
                }
                else // ex: i=1
                {
                    prefab = HalfHeartPrefab;
                }
                var heart = InstantiateGameObjectParented(prefab, Life);
                heart.transform.localPosition = new Vector3(i % 6, (i >= 6) ? -1 : 0) * 32 + delta;
            }
            currentLife = gameManager.hero.Life;
            currentMaxLife = gameManager.hero.MaxLife;
        }
    }

}
