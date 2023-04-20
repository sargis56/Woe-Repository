using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using Unity.Netcode;

public class GameController : NetworkBehaviour
{
    public TextMeshProUGUI deadPlayersText;
    public TextMeshProUGUI cheatText;
    public TextMeshProUGUI cheatMonsterText;
    public TextMeshProUGUI livesScreenText;
    public TextMeshProUGUI playerNumScreenText;
    public TextMeshProUGUI monsterAIScreenText;
    public TextMeshProUGUI diffText;
    public TextMeshProUGUI livesText;
    public GameObject diffDial;
    public Material deactiveMaterial;
    private bool showMonsterCheatList = false;

    public enum GameDifficulty { Easy, Normal, Hard, Nightmare, Unrelenting};
    public GameDifficulty gameDifficulty;

    public GameObject[] players;
    public NetworkVariable<int> deadPlayersNum = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone);
    public GameObject monster;

    public bool menaceSystem;

    public GameObject[] bots;
    public GameObject[] enemies;
    public GameObject[] safeZones;
    public GameObject[] secBarriers;

    public GameObject[] vitaChambers;

    public NetworkVariable<int> playerLives = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone);

    public bool globalDebug = false;
    public bool cheatCodes = false;

    public bool decontamination = false;
    public float decontaminationTime = 180.0f;
    public bool updateDiff = false;

    [SerializeField]
    private int diffIndex = 0;

    public bool unlockLabDoor = false;
    public NetworkVariable<bool> lockDown = new NetworkVariable<bool>(true, NetworkVariableReadPermission.Everyone);
    public GameObject dirLight;
    public GameObject labDoor;

    public bool firstBotShutdown = true;

    [SerializeField]
    private bool removeDoors = false;
    public GameObject[] doors;

    // Start is called before the first frame update
    public override void OnNetworkSpawn()
    //public void Start()
    {
        //deadPlayersNum = 0;
        //monster = GameObject.FindGameObjectWithTag("Director").GetComponent<GameController>().monster;
        bots = GameObject.FindGameObjectsWithTag("Bot");
        enemies = GameObject.FindGameObjectsWithTag("Enemy");
        vitaChambers = GameObject.FindGameObjectsWithTag("VitaChamber");
        safeZones = GameObject.FindGameObjectsWithTag("Safe Zone");
        secBarriers = GameObject.FindGameObjectsWithTag("Security Barrier");
        dirLight = GameObject.FindGameObjectWithTag("DirLight");
        doors = GameObject.FindGameObjectsWithTag("Door");

        firstBotShutdown = true;

        foreach (GameObject enemy in enemies)
        {
            Physics.IgnoreCollision(monster.GetComponent<CapsuleCollider>(), enemy.GetComponent<CapsuleCollider>(), true);
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
        
        players = GameObject.FindGameObjectsWithTag("Player");

        if (!IsSpawned)
        {
            print("I am not on a client");
            return;
        }

        if (globalDebug)
        {
            cheatCodes = true;

            cheatText.GetComponent<TextMeshProUGUI>().enabled = true;

            if (showMonsterCheatList)
            {
                cheatMonsterText.GetComponent<TextMeshProUGUI>().enabled = true;
            }
            else
            {
                cheatMonsterText.GetComponent<TextMeshProUGUI>().enabled = false;
            }

            deadPlayersText.text = "Dead Players: " + deadPlayersNum.Value.ToString() + " | Players: " + players.Length.ToString();
        }
        else
        {
            cheatText.GetComponent<TextMeshProUGUI>().enabled = false;
            cheatMonsterText.GetComponent<TextMeshProUGUI>().enabled = false;
            deadPlayersText.text = "";
        }

        if (cheatCodes)
        {
            if (Input.GetKeyDown("m"))
            {
                lockDown.Value = false;
                players = GameObject.FindGameObjectsWithTag("Player");
            }
            if (Input.GetKeyDown("n"))
            {
                decontamination = true;
            }
            if (Input.GetKeyDown("b"))
            {
                foreach (GameObject player in players)
                {
                    if (player != null)
                    {
                        player.GetComponent<PlayerController>().hasPestFlask = true;
                    }
                }
            }
            if (Input.GetKeyDown("v"))
            {
                foreach (GameObject player in players)
                {
                    if (player != null)
                    {
                        player.GetComponent<PlayerController>().pesticideMachine.GetComponent<VialMachineController>().comp1Found = true;
                        player.GetComponent<PlayerController>().pesticideMachine.GetComponent<VialMachineController>().comp2Found = true;
                        player.GetComponent<PlayerController>().pesticideMachine.GetComponent<VialMachineController>().comp3Found = true;
                        player.GetComponent<PlayerController>().pesticideMachine.GetComponent<VialMachineController>().comp4Found = true;
                        player.GetComponent<PlayerController>().pesticideMachine.GetComponent<VialMachineController>().comp5Found = true;
                        player.GetComponent<PlayerController>().pesticideMachine.GetComponent<VialMachineController>().comp6Found = true;
                        player.GetComponent<PlayerController>().pesticideMachine.GetComponent<VialMachineController>().comp7Found = true;
                        player.GetComponent<PlayerController>().pesticideMachine.GetComponent<VialMachineController>().comp8Found = true;
                        player.GetComponent<PlayerController>().pesticideMachine.GetComponent<VialMachineController>().comp9Found = true;
                    }
                }
            }
            if (Input.GetKeyDown("c"))
            {
                unlockLabDoor = true;
            }
            if (Input.GetKeyDown("x"))
            {
                if (removeDoors)
                {
                    removeDoors = false;
                }
                else
                {
                    removeDoors = true;
                }
            }
            if (Input.GetKeyDown("z"))
            {
                if (showMonsterCheatList)
                {
                    showMonsterCheatList = false;
                }
                else
                {
                    showMonsterCheatList = true;
                }
            }
        }

        if ((playerLives.Value <= 0) && (deadPlayersNum.Value == players.Length))
        {
            //SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            SceneManager.LoadScene("LoseScreen");
        }

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

        foreach (GameObject vitaChamber in vitaChambers)
        {
            foreach (GameObject player in players)
            {
                if ((player != null) && (vitaChamber != null))
                {
                    if (Vector3.Distance(vitaChamber.transform.position, player.transform.position) < 2.0f) 
                    {
                        vitaChamber.GetComponent<VitaManager>().isActive = false;
                    }
                    else
                    {
                        vitaChamber.GetComponent<VitaManager>().isActive = true;
                    }
                }
            }
           
        }

        if (!lockDown.Value)
        {
            dirLight.SetActive(false);
            foreach (GameObject secBarrier in secBarriers)
            {
                secBarrier.SetActive(false);
            }
            monster.GetComponent<MonsterController>().pausePatrol = false;

            foreach (GameObject bot in bots)
            {
                if ((bot != null) && (firstBotShutdown) && (!decontamination))
                {
                    bot.GetComponent<BotController>().shutDown = false;
                    bot.GetComponent<BotController>().currentState = BotController.BotState.Idle;
                }
            }
            firstBotShutdown = false;
            players = GameObject.FindGameObjectsWithTag("Player");
            diffDial.GetComponent<DialController>().enabled = false;
            diffDial.GetComponent<MaterialReset>().enabled = false;
            diffDial.GetComponent<MeshRenderer>().material = deactiveMaterial;
        }
        else
        {
            switch (diffDial.GetComponent<DialController>().dialCount)
            {
                case 0:
                    DiffEasy();
                    break;

                case 1:
                    DiffNormal();
                    break;

                case 2:
                    DiffHard();
                    break;

                case 3:
                    DiffNightmare();
                    break;

                case 4:
                    DiffUnrelenting();
                    break;
            }

            foreach (GameObject bot in bots)
            {
                if (bot != null)
                {
                    bot.GetComponent<BotController>().shutDown = true;
                    bot.GetComponent<BotController>().currentState = BotController.BotState.ShutDown;
                }
            }
        }

        if (removeDoors)
        {
            foreach (GameObject door in doors)
            {
                door.SetActive(false);
            }
        }
        else
        {
            foreach (GameObject door in doors)
            {
                door.SetActive(true);
            }
        }

        if (unlockLabDoor)
        {
            labDoor.SetActive(false);
        }
        else
        {
            labDoor.SetActive(true);
        }

        if (menaceSystem)
        {
            monster.GetComponent<MonsterController>().menaceSystemActive = true;
        }
        else
        {
            monster.GetComponent<MonsterController>().menaceSystemActive = false;
        }

        livesText.text = "Lives: " + playerLives.Value.ToString();
        livesScreenText.text = playerLives.Value.ToString();
        playerNumScreenText.text = players.Length.ToString();

        if (decontamination)
        {
            Decontaminate();
        }

    }

    public void AddLives(int lives)
    {
        playerLives.Value = playerLives.Value + lives;
    }

    public void TakeLives(int lives)
    {
        playerLives.Value = playerLives.Value - lives;
    }

    public void AddDeadPlayer(int amount)
    {
        deadPlayersNum.Value = deadPlayersNum.Value + amount;
    }

    public void TakeDeadPlayer(int amount)
    {
        deadPlayersNum.Value -= amount;
    }

    public GameObject GetClosestVitaChamber(Vector3 position_)
    {
        GameObject closestWaypoint = null;
        float distance = Mathf.Infinity;
        foreach (GameObject vitaChamber in vitaChambers)
        {
            Vector3 diff = vitaChamber.transform.position - position_;
            float curDistance = diff.sqrMagnitude;
            if ((curDistance < distance) && vitaChamber.GetComponent<VitaManager>().isActive)
            {
                closestWaypoint = vitaChamber;
                distance = curDistance;
            }
        }

        return closestWaypoint;
    }

    void Decontaminate()
    {
        //removeDoors = true;
        menaceSystem = false;

        decontaminationTime -= Time.deltaTime;
        if (decontaminationTime < 0.0f)
        {
            if (monster != null)
            {
                Destroy(monster.gameObject);
            }
            SceneManager.LoadScene("WinScreen");
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
        diffText.text = "Easy";
        monsterAIScreenText.text = "Dumb";
        playerLives.Value = 30;
        monster.GetComponent<MonsterController>().enableSprintDetection = false;
        menaceSystem = true;
        monster.GetComponent<MonsterController>().intelligence = MonsterController.MonsterIntelligence.Dumb;
    }

    void DiffNormal()
    {
        diffText.text = "Normal";
        monsterAIScreenText.text = "Incompetent";
        playerLives.Value = 15;
        monster.GetComponent<MonsterController>().enableSprintDetection = false;
        menaceSystem = true;
        monster.GetComponent<MonsterController>().intelligence = MonsterController.MonsterIntelligence.Incompetent;
    }

    void DiffHard()
    {
        diffText.text = "Hard";
        monsterAIScreenText.text = "Competent";
        playerLives.Value = 10;
        monster.GetComponent<MonsterController>().enableSprintDetection = true;
        menaceSystem = true;
        monster.GetComponent<MonsterController>().intelligence = MonsterController.MonsterIntelligence.Competent;
    }

    void DiffNightmare()
    {
        diffText.text = "Nightmare";
        monsterAIScreenText.text = "Smart";
        playerLives.Value = 5;
        monster.GetComponent<MonsterController>().enableSprintDetection = true;
        menaceSystem = false;
        monster.GetComponent<MonsterController>().intelligence = MonsterController.MonsterIntelligence.Smart;
    }
    void DiffUnrelenting()
    {
        diffText.text = "Unrelenting";
        monsterAIScreenText.text = "Apex Predator";
        playerLives.Value = 0;
        monster.GetComponent<MonsterController>().enableSprintDetection = true;
        menaceSystem = false;
        monster.GetComponent<MonsterController>().intelligence = MonsterController.MonsterIntelligence.ApexPredator;
    }
    [ServerRpc(RequireOwnership = false)]
    public void AddLivesServerRpc()
    {
        playerLives.Value = playerLives.Value + 1;
    }

    [ServerRpc(RequireOwnership = false)]
    public void TakeLivesServerRpc()
    {
        playerLives.Value = playerLives.Value - 1;
    }

    [ServerRpc(RequireOwnership = false)]
    public void TurnOffLockDownServerRpc()
    {
        lockDown.Value = false;
    }

    [ServerRpc(RequireOwnership = false)]
    public void AddDeadPlayerServerRpc()
    {
        deadPlayersNum.Value = deadPlayersNum.Value + 1;
    }

    [ServerRpc(RequireOwnership = false)]
    public void TakeDeadPlayerServerRpc()
    {
        deadPlayersNum.Value -= 1;
    }
}
