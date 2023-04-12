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
    public GameObject[] safeZones;

    public GameObject[] vitaChambers;

    public int playerLives;

    public bool globalDebug = false;

    public bool decontamination = false;
    public float decontaminationTime = 180.0f;
    [SerializeField]
    private float decontaminationTime_ORG;
    public bool updateDiff = false;

    public int diffIndex = 0;

    // Start is called before the first frame update
    void Start()
    {
        deadPlayersNum = 0;
        monster = GameObject.FindGameObjectWithTag("Monster");
        bots = GameObject.FindGameObjectsWithTag("Bot");
        enemies = GameObject.FindGameObjectsWithTag("Enemy");
        vitaChambers = GameObject.FindGameObjectsWithTag("VitaChamber");
        safeZones = GameObject.FindGameObjectsWithTag("Safe Zone");
        decontaminationTime_ORG = decontaminationTime;

        foreach (GameObject enemy in enemies)
        {
            Physics.IgnoreCollision(monster.GetComponent<CapsuleCollider>(), enemy.GetComponent<CapsuleCollider>(), true);
            Physics.IgnoreCollision(monster.GetComponent<BoxCollider>(), enemy.GetComponent<CapsuleCollider>(), true);
            foreach (GameObject bot in bots)
            {
                Physics.IgnoreCollision(enemy.GetComponent<CapsuleCollider>(), bot.GetComponent<BoxCollider>(), true);
            }
        }

        foreach (GameObject bot in bots)
        {
            foreach (GameObject botOther in bots)
            {
                Physics.IgnoreCollision(bot.GetComponent<BoxCollider>(), botOther.GetComponent<BoxCollider>(), true);
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
            if (monster != null)
            {
                monster.GetComponent<MonsterController>().debug = true;
            }

            foreach (GameObject player in players)
            {
                if (player != null)
                {
                    player.GetComponent<PlayerController>().debug = true;
                    player.GetComponent<MovementController>().debug = true;
                }
            }

            foreach (GameObject bot in bots)
            {
                if (bot != null)
                {
                    bot.GetComponent<BotController>().debug = true;
                }
            }

            foreach (GameObject enemy in enemies)
            {
                if (enemy != null)
                {
                    enemy.GetComponent<ThrallController>().debug = true;
                }
            }

        }
        else
        {
            if (monster != null)
            {
                monster.GetComponent<MonsterController>().debug = false;
            }

            foreach (GameObject player in players)
            {
                if (player != null)
                {
                    player.GetComponent<PlayerController>().debug = false;
                    player.GetComponent<MovementController>().debug = false;
                }
            }

            foreach (GameObject bot in bots)
            {
                if (bot != null)
                {
                    bot.GetComponent<BotController>().debug = false;
                }
            }

            foreach (GameObject enemy in enemies)
            {
                if (enemy != null)
                {
                    enemy.GetComponent<ThrallController>().debug = false;
                }
            }
        }

        if (decontamination)
        {
            Decontaminate();
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

    void Decontaminate()
    {
        decontaminationTime -= Time.deltaTime;
        if (decontaminationTime < 0.0f)
        {
            Destroy(monster.gameObject);
        }

        if (monster != null)
        {
            monster.GetComponent<MonsterController>().decon = true;
        }

        if ( ((decontaminationTime < 150) && diffIndex == 0) || 
            ((decontaminationTime < 135) && diffIndex == 1) ||
            ((decontaminationTime < 100) && diffIndex == 2) ||
            ((decontaminationTime < 60) && diffIndex == 3))
        {
            updateDiff = true;
        }

        foreach (GameObject bot in bots)
        {
            if (bot != null)
            {
                bot.GetComponent<BotController>().shutDown = true;
                bot.GetComponent<BotController>().currentState = BotController.BotState.ShutDown;
            }
        }

        foreach (GameObject enemy in enemies)
        {
            Destroy(enemy);
        }

        foreach (GameObject safeZone in safeZones)
        {
            Destroy(safeZone);
        }

        if (updateDiff)
        {

            diffIndex = diffIndex + 1;

            if (monster.GetComponent<MonsterController>().intelligence == MonsterController.MonsterIntelligence.Dumb)
            {
                monster.GetComponent<MonsterController>().intelligence = MonsterController.MonsterIntelligence.Incompetent;
            }
            else if (monster.GetComponent<MonsterController>().intelligence == MonsterController.MonsterIntelligence.Incompetent)
            {
                monster.GetComponent<MonsterController>().intelligence = MonsterController.MonsterIntelligence.Competent;
            }
            else if (monster.GetComponent<MonsterController>().intelligence == MonsterController.MonsterIntelligence.Competent)
            {
                monster.GetComponent<MonsterController>().intelligence = MonsterController.MonsterIntelligence.Smart;
            }
            else if (monster.GetComponent<MonsterController>().intelligence == MonsterController.MonsterIntelligence.Smart)
            {
                monster.GetComponent<MonsterController>().intelligence = MonsterController.MonsterIntelligence.ApexPredator;
            }
            else
            {
                monster.GetComponent<MonsterController>().intelligence = MonsterController.MonsterIntelligence.ApexPredator;
            }


            monster.GetComponent<MonsterController>().currentState = MonsterController.MonsterState.Idle;
            updateDiff = false;
        }

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
