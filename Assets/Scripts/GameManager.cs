/**
 * The Legend of Epikouros
 *   ~ a game made in 48h by Erhune for Ludum Dare 27
 *   
 * Copyright © 2013 Erhune <erhune@gmail.com>
 * 
 * Feel free to contact me any time!
 */
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;



public class GameManager : SingletonComponent<GameManager>
{

    public const float RunDuration = 10;
    public const float CountdownDuration = 4;
    //public const string ServerUrl = "http://localhost:8080";
    public const string ServerUrl = "http://tenseconds.npng.org";
    public const int InitialLives = 2;

    public static readonly bool AllowOffline = false;
    public static readonly bool DebugReplay = false;

    public static int nextUid = 1;

    public PlayState state { get; private set; }
    public float startedCountdownAt { get; private set; }
    public float startedAt { get; private set; }
    public static readonly List<Run> serverRuns = new List<Run>();
    public static string alreadyPlayedAs { get; private set; }
    public static readonly Run currentRun = new Run();

    public Hero hero { get; private set; }

    public bool ambiantEnabled
    {
        get { return (GetComponent<AudioSource>().volume > 0); }
        set { GetComponent<AudioSource>().volume = value ? 1f : 0f; }
    }

    public float remainingTime
    {
        get
        {
            switch (state)
            {
                case PlayState.None:
                case PlayState.RequestToStart:
                case PlayState.WaitingForInput:
                case PlayState.Replaying:
                case PlayState.StartCountdown:
                    return (alreadyPlayedAs != null) ? 0 : RunDuration;

                case PlayState.Playing:
                    return RunDuration - (Time.time - startedAt);

                case PlayState.PostingRun:
                case PlayState.Ended:
                case PlayState.PollingForMore:
                    return 0;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }

    public bool gameIsRunning
    {
        get { return (state == PlayState.Playing || state == PlayState.Replaying); }
    }

    public double currentTime { get; private set; }

    public static void SetInitialData(RunsData data)
    {
        serverRuns.AddRange(data.runs);
        alreadyPlayedAs = data.alreadyPlayedAs;
    }

    public static void StartAs(string nickname)
    {
        currentRun.nickname = nickname;
        Application.LoadLevel("World");
    }

    IEnumerator Start()
    {
        hero = FindObjectOfType<Hero>();
        currentRun.cycle = 1;

        state = PlayState.Replaying;

        foreach (var run in serverRuns)
        {
            yield return StartCoroutine(Replay(run));
        }

        GC.Collect();

        if (alreadyPlayedAs != null)
        {
            state = PlayState.Ended;
        }
        else
        {
            state = PlayState.WaitingForInput;
        }
        hero.nickname = "";

        if (AllowOffline)
        {
            hero.nickname = "Erhune";
            state = PlayState.Playing;
            startedAt = Time.time;
        }

        Log("You can now start to play as {0}!", hero.nickname);
    }

    private IEnumerator RequestStart()
    {
        if (state == PlayState.WaitingForInput)
        {
            state = PlayState.RequestToStart;

            Log("Request to start with nickname {0}...", currentRun.nickname);
            var data = new WWWForm();
            data.AddField("nickname", currentRun.nickname);
            var request = new WWW(ServerUrl + "/api/start", data);
            yield return request;
            LogDebug("Response: {0}", request.text);

            if (request.text == "OK")
            {
                hero.nickname = currentRun.nickname;
                state = PlayState.StartCountdown;
                startedCountdownAt = Time.time;
            }
            else
            {
                state = PlayState.WaitingForInput;
            }
        }
        else
        {
            LogError("Cannot request to start in state {0}", state);
        }
    }

    private IEnumerator EndRun(Run run)
    {
        state = PlayState.PostingRun;

        Log("Posting new run data...");
        var encodedRun = JSON.Serialize(run, false);
        var data = Encoding.UTF8.GetBytes(encodedRun);
        var request = new WWW(ServerUrl + "/api/run", data);
        yield return request;

        Log("New run posted: " + request.text);
        int cycle;
        if (int.TryParse(request.text, out cycle))
        {
            Log("And stored as cycle #{0}", cycle);
            currentRun.cycle = cycle;
            serverRuns.Add(currentRun);
        }

        state = PlayState.Ended;
        hero.nickname = "";
    }

    private IEnumerator PollForMore()
    {
        state = PlayState.PollingForMore;

        var lastCycle = 0;
        if (serverRuns.Count > 0)
        {
            lastCycle = serverRuns[serverRuns.Count - 1].cycle;
        }
        Log("Polling for more runs from cycle #{0}...", lastCycle);

        var request = new WWW(ServerUrl + "/api/poll?from=" + lastCycle);
        yield return request;
        if (request.error != null)
        {
            LogError("Polling returned an error: {0}", request.error);
            yield return new WaitForSeconds(30);
        }
        else
        {
            var runsData = JSON.Deserialize<RunsData>(request.text);
            Log("Server returned {0} new runs", runsData.runs.Length);

            if (runsData.runs.Length > 0)
            {
                foreach (var run in runsData.runs)
                {
                    serverRuns.Add(run);
                    yield return StartCoroutine(Replay(run));
                }
                yield return new WaitForSeconds(3);
            }
            else
            {
                yield return new WaitForSeconds(10);
            }
        }

        Log("Going back to state Ended");
        state = PlayState.Ended;
    }

    void Update()
    {
        if (hero == null)
        {
            return;
        }

        if (state == PlayState.WaitingForInput)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                StartCoroutine(RequestStart());
            }
        }

        if (state == PlayState.StartCountdown && (Time.time - startedCountdownAt) >= CountdownDuration)
        {
            state = PlayState.Playing;
            startedAt = Time.time;
        }

        if (state == PlayState.Ended)
        {
            StartCoroutine(PollForMore());
        }

        if (state != PlayState.Playing)
        {
            return;
        }

        // FROM NOW ON, THIS IS FOR WHEN WE ARE PLAYING!

        if (Time.time - startedAt >= RunDuration)
        {
            StartCoroutine(EndRun(currentRun));

            // Cannot play any longer
            return;
        }

        currentTime = currentRun.startTime + Time.time - startedAt;

        // Movement
        var dx = Input.GetAxisRaw("Horizontal");
        var dy = Input.GetAxisRaw("Vertical");
        var newMovement = new Action
                              {
                                  type = ActionType.Movement,
                                  x = Mathf.RoundToInt(dx),
                                  y = Mathf.RoundToInt(dy)
                              };
        if (hero.CanAct() && (newMovement.x != hero.currentMovementX || newMovement.y != hero.currentMovementY))
        {
            //LogDebug("Change movement!");
            AddAndExecute(newMovement);
        }

        // Update all entities
        AdvanceGameplay(Time.deltaTime, AddAndExecute);

        // Attack
        if (Input.GetButtonDown("Fire1") && hero.CanAttack())
        {
            AddAndExecute(new Action
                              {
                                  type = ActionType.Attack
                              });
        }

        // Drop bomb?
        if (Input.GetButtonDown("Fire2") && hero.CanAct())
        {
            if (hero.Bombs > 0)
            {
                AddAndExecute(new Action
                                  {
                                      type = ActionType.DropBomb
                                  });
            }
            else
            {
                Log("Not enough bombs!");
            }
        }
    }

    public Enemy FindEnemy(int uid)
    {
        foreach (var enemy in FindObjectsOfType<Enemy>())
        {
            if (enemy.uid == uid)
            {
                return enemy;
            }
        }
        return null;
    }

    public Pickup FindPickup(int uid)
    {
        foreach (var pickup in FindObjectsOfType<Pickup>())
        {
            if (pickup.uid == uid)
            {
                return pickup;
            }
        }
        return null;
    }

    public Bombable FindBombable(int uid)
    {
        foreach (var bombable in FindObjectsOfType<Bombable>())
        {
            if (bombable.uid == uid)
            {
                return bombable;
            }
        }
        return null;
    }

    public Unlockable FindUnlockable(int uid)
    {
        foreach (var unlockable in FindObjectsOfType<Unlockable>())
        {
            if (unlockable.uid == uid)
            {
                return unlockable;
            }
        }
        return null;
    }

    private void AddAndExecute(Action action)
    {
        action.gameTime = currentTime;
        action.atX = hero.transform.position.x;
        action.atY = hero.transform.position.y;
        if (state == PlayState.Playing)
        {
            currentRun.actions.Add(action);
        }
        hero.Apply(action, AddAndExecute);
    }

    private void ExecuteOnly(Action action)
    {
        hero.Apply(action, ExecuteOnly);
    }

    private IEnumerator Replay(Run run)
    {
        Log("Replaying {0}", run);
        hero.nickname = run.nickname;
        var start = Time.time;
        currentTime = run.startTime;
        foreach (var action in run.actions)
        {
            currentTime = run.startTime + Time.time - start;
            if (DebugReplay) LogDebug("Trying to replay {0}", action);
            while (action.gameTime > currentTime)
            {
                if (DebugReplay) LogDebug("  => wait...");
                AdvanceGameplay(Time.deltaTime, ExecuteOnly);
                yield return null;
                currentTime = run.startTime + Time.time - start;
            }
            if (DebugReplay) LogDebug("  => OK @ {0:0.000}", currentTime);
            hero.transform.position = new Vector3(action.atX, action.atY);
            ExecuteOnly(action);
        }

        // Wait for the end of the run time slot
        while (currentTime < run.startTime + RunDuration)
        {
            AdvanceGameplay(Time.deltaTime, ExecuteOnly);
            yield return null;
            currentTime = run.startTime + Time.time - start;
        }

        // Be sure to remove any pending movement from the run, or else the real player might inadvertently start their turn
        hero.StopMovement();
        currentRun.cycle = run.cycle + 1;
    }

    private void AdvanceGameplay(float deltaTime, ActionProcessor processor)
    {
        hero.Advance(deltaTime, processor);
        foreach (var enemy in FindObjectsOfType<Enemy>())
        {
            enemy.Advance();
        }
        foreach (var bomb in FindObjectsOfType<Bomb>())
        {
            bomb.Advance(processor);
        }
    }

}
