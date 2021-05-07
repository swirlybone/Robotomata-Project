using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using static NotificationCenter;
using static RayCastingHelper;
using static MessageHelper;
using UnityEditor;

public class GameManager : MonoBehaviour
{
    RobotController robotController;

    int rent = 1;
    int number = 0;

    [SerializeField] int money = 0;
    [SerializeField] GameObject robot;
    [SerializeField] GameObject spawner;
    [SerializeField] GameObject deposit;
    [SerializeField] GameObject bigRobot;
    [SerializeField] GameObject repairBot;
    [SerializeField] GameObject buyMenu;
    [SerializeField] GameObject upgradeMenu;
    [SerializeField] TextMeshProUGUI moneyTracker;

    GameObject spawn;

    public enum PlayerState { PLACING, OPEN }
    public enum GameState { TUTORIAL, PLAYING, PAUSED, LOST }

    PlayerState state;
    GameState playing;

    void Start()
    {
        buyMenu.SetActive(false);
        upgradeMenu.SetActive(false);
        UpdateMoney();
        state = PlayerState.OPEN;
        playing = GameState.TUTORIAL;
    }

    private void OnEnable()
    {
        AddObserver("AwardMoney", GetMoney);
        AddObserver("AwardMoney", GetTutorialMoney);
        AddObserver("SpendMoney", Spend);
        AddObserver("DonePlacing", DonePlacing);
        AddObserver("ReplaceMe", ReplaceRobot);
        AddObserver("TrySellThis", TrySelling);
        AddObserver("TutorialFinished", TutorialFinished);
        AddObserver("Zoom", OpenUpgradeMenu);

    }
    private void OnDisable()
    {
        RemoveObserver("AwardMoney", GetMoney);
        RemoveObserver("AwardMoney", GetTutorialMoney);
        RemoveObserver("SpendMoney", Spend);
        RemoveObserver("DonePlacing", DonePlacing);
        RemoveObserver("ReplaceMe", ReplaceRobot);
        RemoveObserver("TrySellThis", TrySelling);
        RemoveObserver("TutorialFinished", TutorialFinished);
        RemoveObserver("Zoom", OpenUpgradeMenu);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("r") && state == PlayerState.OPEN) buyMenu.SetActive(!buyMenu.activeSelf);
        if (Input.GetKeyDown(KeyCode.Escape)) { buyMenu.SetActive(false); upgradeMenu.SetActive(false); }
    }

    IEnumerator PlacingSpawnable(Spawnable toSpawn, int cost)
    {
        buyMenu.SetActive(false);
        state = PlayerState.PLACING;
        yield return new WaitForEndOfFrame();
        while (toSpawn.Floating())
        {
            if (CameraRayHit("Floor"))
            {
                Vector3 fixedVector = new Vector3(GetHit().point.x, GetHit().point.y + toSpawn.SpawnHeight, GetHit().point.z);
                toSpawn.transform.position = fixedVector;
                if (Input.GetMouseButtonUp(0) && CanSpendMoney(cost))
                {
                    SpendMoney(cost);
                    number++;
                    toSpawn.name = number.ToString();
                    toSpawn.PlaceDown();
                    PostNotification("Added", toSpawn.gameObject);
                }
            }
            else toSpawn.transform.position = GetRay().GetPoint(10);

            if (Input.GetMouseButtonDown(1)) { Destroy(toSpawn.gameObject); StopAllCoroutines(); }
            yield return null;
        }
    }

    public void MakeBot(int index)
    {
        int cost;
        GameObject toSpawn;

        switch (index)
        {
            case 1:
                toSpawn = robot;
                cost = 100;
                break;
            case 2:
                toSpawn = bigRobot;
                cost = 500;
                break;
            case 3:
                toSpawn = repairBot;
                cost = 300;
                break;
            default:
                cost = 0;
                toSpawn = new GameObject();
                break;
        }

        if (state == PlayerState.OPEN && CanSpendMoney(cost))
        {
            StartCoroutine(PlacingSpawnable(Instantiate(toSpawn).GetComponent<RobotController>(), cost));
        }
    }

    void GetMoney(Notification notification)
    {
        if (!InTutorial())
        {
            int value = (int)notification.Object;
            Log(value.ToString() + " Money Awarded\n");
            money += value; UpdateMoney();
        }
    }

    void GetTutorialMoney(Notification notification)
    {
        if (InTutorial())
        {
            int value = (int)notification.Object;
            Log(value.ToString() + " Money Awarded\n");
            money += value; UpdateMoney();
        }
    }

    void SpendMoney(int spent) { money -= spent; UpdateMoney(); }
    void Spend(Notification notification) { SpendMoney((int)notification.Object); }
    void UpdateMoney() { moneyTracker.text = "You have $" + money.ToString(); }
    void DonePlacing(Notification notification) { state = PlayerState.OPEN; }
    void ReplaceRobot(Notification notification)
    {
        if (!InTutorial())
        {
            RobotController robotController = (RobotController)notification.Object;
            robotController.Initializer();
            StartCoroutine(PlacingSpawnable(robotController, 0));
        }
    }

    void TrySelling(Notification notification)
    {
        DestructibleWall destructible = (DestructibleWall)notification.Object;

        if (CanSpendMoney(destructible.Cost))
        {
            PostNotification("Sell", destructible);
            SpendMoney(destructible.Cost);
        }
    }

    void OpenUpgradeMenu(Notification notification)
    {
        upgradeMenu.SetActive(true);
        GameObject notJect = (GameObject)notification.Object;
        RobotController robot = notJect.GetComponent<RobotController>();
        robotController = robot.GetComponent<RobotController>();
    }

    public delegate void Method();

    public void SpeedUpgrade()
    {
        if (!InTutorial())
        {
            UpgradeBot(robotController.UpgradeSpeed, 25);
        }
    }

    public void LifeSpanUpgrade()
    {
        if (!InTutorial())
        {
            UpgradeBot(robotController.UpgradeLifeSpan, 20);
        }
    }


    void UpgradeBot(Method method, int cost)
    {
        if (!InTutorial())
        {
            if (CanSpendMoney(cost) && robotController.CanBeUpgraded())
            {
                SpendMoney(cost);
                method();
            }
            else { Log("Can't afford, or too many upgrades already!"); }
        }
    }

    bool CanSpendMoney(int amount) => (money - amount) >= 0;

    void MoneyLoss() { SpendMoney(rent); if (money <= 0) { playing = GameState.LOST; } }
    void RentUp() { rent++; }

    void TutorialFinished(Notification notification)
    {
        playing = GameState.PLAYING;
        InvokeRepeating(nameof(MoneyLoss), 1, 1);
        InvokeRepeating(nameof(RentUp), 30, 30);
        Invoke(nameof(FirstHint), 10);
    }

    bool InTutorial() => playing == GameState.TUTORIAL;

    void FirstHint() => Message("Hey buddy, you should try to break down that wall in front of you for about $1250. No pressure. It's just a cool thing to do. Click on it when you can afford it!");
}

#if UNITY_EDITOR
[CustomEditor(typeof(GameManager))]
public class GameTester : Editor
{
    int money;
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        money = EditorGUILayout.IntField("Money to add: ", money);

        if (GUILayout.Button("Earn Money")) PostNotification("AwardMoney", money);
    }
}
#endif