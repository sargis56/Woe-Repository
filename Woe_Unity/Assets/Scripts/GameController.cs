using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    public TextMeshProUGUI deadPlayersText;

    public enum GameDifficulty { Easy, Normal, Hard, Nightmare, Unrelenting};
    public GameDifficulty gameDifficulty;

    public GameObject[] players;
    public int deadPlayersNum;
    public GameObject monster;
    public float menace = 0.0f;

    public GameObject[] bots;
    public GameObject[] enemies;

    public GameObject[] vitaChambers;

    public int playerLives;

    public bool globalDebug = false;

    // Start is called before the first frame update
    void Start()
    {
        deadPlayersNum = 0;
        monster = GameObject.FindGameObjectWithTag("Monster");
        bots = GameObject.FindGameObjectsWithTag("Bot");
        enemies = GameObject.FindGameObjectsWithTag("Enemy");
        vitaChambers = GameObject.FindGameObjectsWithTag("VitaChamber");


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

    // Update is called once per frame
    void Update()
    {
        if (globalDebug)
        {
            deadPlayersText.text = "Dead Players: " + deadPlayersNum.ToString();
        }
        else
        {
            deadPlayersText.text = "";
        }

        if ((playerLives <= 0) && (deadPlayersNum == players.Length))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        players = GameObject.FindGameObjectsWithTag("Player");

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

            foreach (GameObject enemy in enemies)
            {
                enemy.GetComponent<ThrallController>().debug = true;
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

            foreach (GameObject enemy in enemies)
            {
                enemy.GetComponent<ThrallController>().debug = false;
            }
        }

    }

    public void AddLives(int lives)
    {
        playerLives = playerLives + lives;
    }

    public void TakeLives(int lives)
    {
        playerLives -= lives;
    }

    public void AddDeadPlayer(int amount)
    {
        deadPlayersNum = deadPlayersNum + amount;
    }

    public void TakeDeadPlayer(int amount)
    {
        deadPlayersNum -= amount;
    }

    public GameObject GetClosestVitaChamber(Vector3 position_)
    {
        GameObject closestWaypoint = null;
        float distance = Mathf.Infinity;
        foreach (GameObject vitaChamber in vitaChambers)
        {
            Vector3 diff = vitaChamber.transform.position - position_;
            float curDistance = diff.sqrMagnitude;
            if (curDistance < distance)
            {
                closestWaypoint = vitaChamber;
                distance = curDistance;
            }
        }

        return closestWaypoint;
    }

    void DiffEasy()
    {
        playerLives = 30;
        monster.GetComponent<MonsterController>().intelligence = MonsterController.MonsterIntelligence.Dumb;
    }

    void DiffNormal()
    {
        playerLives = 15;
        monster.GetComponent<MonsterController>().intelligence = MonsterController.MonsterIntelligence.Incompetent;
    }

    void DiffHard()
    {
        playerLives = 10;
        monster.GetComponent<MonsterController>().intelligence = MonsterController.MonsterIntelligence.Competent;
    }

    void DiffNightmare()
    {
        playerLives = 5;
        monster.GetComponent<MonsterController>().intelligence = MonsterController.MonsterIntelligence.Smart;
    }
    void DiffUnrelenting()
    {
        playerLives = 0;
        monster.GetComponent<MonsterController>().intelligence = MonsterController.MonsterIntelligence.ApexPredator;
    }
}
