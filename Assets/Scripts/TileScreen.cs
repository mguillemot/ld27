/**
 * The Legend of Epikouros
 *   ~ a game made in 48h by Erhune for Ludum Dare 27
 *   
 * Copyright (c) 2013 Erhune <erhune@gmail.com>
 * 
 * Feel free to contact me any time!
 */
using System.Collections;
using UnityEngine;


public class TileScreen : GameComponent
{

    public UILabel Status;
    public UILabel Called;
    public UIInput Nickname;

    private bool dataLoaded;
    private bool serverError;

    IEnumerator Start()
    {
        Log("Starting game...");

        StartCoroutine(LoadInitialData());

        var lines = Status.text.Split('\n');
        Status.text = "";
        Called.text = "";
        Nickname.gameObject.SetActive(false);
        
        for (int i = 0, n = lines.Length; i < n; i++)
        {
            var line = lines[i];
            Status.text += line + "\n";
            yield return new WaitForSeconds(Application.isEditor ? .1f : 2f);
        }

        while (!dataLoaded && !serverError)
        {
            yield return null;
        }

        if (serverError)
        {
            Called.text = "And I cannot start my journey because the server is down :(";
        }
        else if (GameManager.alreadyPlayedAs != null)
        {
            Called.text = string.Format("And I was known during my adventures as {0}.", GameManager.alreadyPlayedAs);
            yield return new WaitForSeconds(3f);
            GameManager.StartAs(GameManager.alreadyPlayedAs);
        }
        else
        {
            Called.text = "But people call me:";
            Nickname.gameObject.SetActive(true);
            Nickname.selected = true;
        }
    }
    
    IEnumerator LoadInitialData()
    {
        Log("Loading server data so far...");
        var request = new WWW(GameManager.ServerUrl + "/api/load");
        yield return request;
        if (request.error != null)
        {
            LogError("Error while loading server data: {0}", request.error);
            serverError = true;
        }
        else
        {
            var runsData = JSON.Deserialize<RunsData>(request.text);
            Log("Received: {0} runs, alreadyPlayed={1}", runsData.runs.Length, runsData.alreadyPlayedAs);
            dataLoaded = true;

            GameManager.SetInitialData(runsData);
        }
    }

    void OnSubmit()
    {
        var nickname = Nickname.text.Trim();
        if (nickname.Length > 32)
        {
            nickname = nickname.Substring(0, 32);
        }
        if (!string.IsNullOrEmpty(nickname))
        {
            Log("Nickname set to: " + nickname);
            GameManager.StartAs(Nickname.text);
        }
        else
        {
            LogError("Nickname is empty");
        }
    }

}
