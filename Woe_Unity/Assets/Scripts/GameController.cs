using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public enum GameDifficulty { Easy, Normal, Hard, Nightmare, Unrelenting};
    public GameDifficulty gameDifficulty;

    public GameObject[] players;
    public GameObject monster;
    public float menace = 0.0f;

    public GameObject[] bots;

    public bool globalDebug = false;

    // Start is called before the first frame update
    void Start()
    {
        players = GameObject.FindGameObjectsWithTag("Player");
        monster = GameObject.FindGameObjectWithTag("Monster");
        bots = GameObject.FindGameObjectsWithTag("Bot");
    }

    // Update is called once per frame
    void Update()
    {
        if (globalDebug)
        {
            monster.GetComponent<MonsterController>().debug = true;

            foreach (GameObject player in players)
            {
                player.GetComponent<PlayerController>().debug = true;
                player.GetComponent<MovementController>().debug = true;
            }

            foreach (GameObject bot in bots)
            {
                bot.GetComponent<BotController>().debug = true;
            }

        }
        else
        {
            monster.GetComponent<MonsterController>().debug = false;

            foreach (GameObject player in players)
            {
                player.GetComponent<PlayerController>().debug = false;
                player.GetComponent<MovementController>().debug = false;
            }

            foreach (GameObject bot in bots)
            {
                bot.GetComponent<BotController>().debug = false;
            }
        }

        switch (gameDifficulty)
        {
            case GameDifficulty.Easy:
                DiffEasy();
                break;

            case GameDifficulty.Normal:
                DiffNormal();
                break;

            case GameDifficulty.Hard:
                DiffHard();
                break;

            case GameDifficulty.Nightmare:
                DiffNightmare();
                break;

            case GameDifficulty.Unrelenting:
                DiffUnrelenting();
                break;
        }
    }

    void DiffEasy()
    {
        monster.GetComponent<MonsterController>().intelligence = MonsterController.MonsterIntelligence.Dumb;
    }

    void DiffNormal()
    {
        monster.GetComponent<MonsterController>().intelligence = MonsterController.MonsterIntelligence.Incompetent;
    }

    void DiffHard()
    {
        monster.GetComponent<MonsterController>().intelligence = MonsterController.MonsterIntelligence.Competent;
    }

    void DiffNightmare()
    {
        monster.GetComponent<MonsterController>().intelligence = MonsterController.MonsterIntelligence.Smart;
    }
    void DiffUnrelenting()
    {
        monster.GetComponent<MonsterController>().intelligence = MonsterController.MonsterIntelligence.ApexPredator;
    }
}
