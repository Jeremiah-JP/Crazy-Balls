using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum GameState { Setup, Running, Won, Lost }

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    static int carriedMoney = -1;

    public Ball ballPrefab;
    public Launcher launcher;
    public HandUI hand;
    public int startingMoney = 5;
    public int clearReward = 3;
    public int moneyPerLeftoverBall = 1;
    public int maxBalls = 300;
    public int fastForwardSteps = 3;

    public TMP_Text moneyText;
    public TMP_Text ballsText;
    public TMP_Text targetText;
    public TMP_Text resultText;
    public GameObject resultPanel;
    public Button launchButton;
    public Button nextButton;

    public GameState State { get; private set; } = GameState.Setup;
    public int Money { get; private set; }
    public bool FastForward { get; set; }

    readonly List<Ball> balls = new List<Ball>();
    readonly List<Ball> stepBuffer = new List<Ball>();
    readonly List<Target> targets = new List<Target>();
    int moneyAtLevelStart;

    void Awake()
    {
        Instance = this;
        Physics2D.simulationMode = SimulationMode2D.Script;
        Money = carriedMoney >= 0 ? carriedMoney : startingMoney;
        moneyAtLevelStart = Money;
        targets.AddRange(FindObjectsByType<Target>(FindObjectsSortMode.None));
        if (resultPanel) resultPanel.SetActive(false);
    }

    void Start()
    {
        RefreshUI();
    }

    void FixedUpdate()
    {
        if (State != GameState.Running) return;
        float dt = Time.fixedDeltaTime;
        int steps = FastForward ? fastForwardSteps : 1;
        for (int i = 0; i < steps && State == GameState.Running; i++)
        {
            launcher.Tick(dt);
            Physics2D.Simulate(dt);
            stepBuffer.Clear();
            stepBuffer.AddRange(balls);
            foreach (var b in stepBuffer) if (b) b.Step(dt);
            CheckEnd();
        }
        RefreshUI();
    }

    public void Launch()
    {
        if (State != GameState.Setup) return;
        if (SetupInput.Instance) SetupInput.Instance.ClearAll();
        State = GameState.Running;
        launcher.Begin();
        if (launchButton) launchButton.interactable = false;
        RefreshUI();
    }

    public bool TrySpend(int amount)
    {
        if (amount > Money) return false;
        Money -= amount;
        RefreshUI();
        return true;
    }

    public void SpawnBall(Vector3 position, Vector2 velocity, IEnumerable<Gate> inheritedGates)
    {
        if (balls.Count >= maxBalls) return;
        Ball b = Instantiate(ballPrefab, position, Quaternion.identity);
        b.Init(velocity, inheritedGates);
        balls.Add(b);
    }

    public void UnregisterBall(Ball ball)
    {
        balls.Remove(ball);
    }

    public void OnTargetChanged()
    {
        RefreshUI();
    }

    void CheckEnd()
    {
        if (targets.TrueForAll(t => t.Destroyed))
        {
            Win();
            return;
        }
        if (launcher.Finished && balls.Count == 0) Lose();
    }

    void Win()
    {
        State = GameState.Won;
        int leftover = balls.Count;
        int earned = clearReward + leftover * moneyPerLeftoverBall;
        Money += earned;
        ShowResult($"Level clear!\nLeftover balls: {leftover}\n+${earned}", true);
    }

    void Lose()
    {
        State = GameState.Lost;
        ShowResult($"Out of balls.\n{RemainingHealth()} hits left.", false);
    }

    void ShowResult(string message, bool won)
    {
        if (resultPanel) resultPanel.SetActive(true);
        if (resultText) resultText.text = message;
        if (nextButton) nextButton.gameObject.SetActive(won);
        RefreshUI();
    }

    public void Retry()
    {
        carriedMoney = moneyAtLevelStart;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void NextLevel()
    {
        carriedMoney = Money;
        int next = SceneManager.GetActiveScene().buildIndex + 1;
        if (next < SceneManager.sceneCountInBuildSettings) SceneManager.LoadScene(next);
        else SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    int RemainingHealth()
    {
        int total = 0;
        foreach (var t in targets) total += Mathf.Max(0, t.health);
        return total;
    }

    public void RefreshUI()
    {
        if (moneyText) moneyText.text = "$" + Money;
        if (ballsText) ballsText.text = State == GameState.Setup ? launcher.ballCount.ToString() : (balls.Count + launcher.RemainingToFire).ToString();
        if (targetText) targetText.text = RemainingHealth().ToString();
        if (hand) hand.RefreshAffordability(Money, State == GameState.Setup);
    }
}
