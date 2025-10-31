using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    [Header("Refs")]
    public MazeGenerator generator;
    public Transform player;
    public PlayerController playerController;
    public Transform cameraPivot;

    [Header("UI")]
    public TextMeshProUGUI candyText;
    public TextMeshProUGUI timerText;
    public GameObject winPanel;
    public GameObject losePanel;

    [Header("Rules")]
    public float startTime = 90f;
    public int candiesToUnlock = 8;
    public float timePerCandy = 5f;

    private int candies;
    private float timeLeft;
    private ExitGate gate;

    void Start()
    {
        NewRun();
    }

    public void NewRun()
    {
        Time.timeScale = 1f; // Restore normal time before regenerating
        timeLeft = startTime;
        candies = 0;
        winPanel.SetActive(false);
        losePanel.SetActive(false);

        generator.Generate();
        gate = FindObjectOfType<ExitGate>();

        // Move player to spawn
        player.position = generator.PlayerSpawnWorldPos;
        player.tag = "Player";
        playerController.cameraPivot = cameraPivot;

        UpdateHUD();
    }

    void Update()
    {
        timeLeft -= Time.deltaTime;
        if (timeLeft <= 0f)
        {
            timeLeft = 0;
            GameOver(false);
        }
        UpdateHUD();
    }

    void UpdateHUD()
    {
        if (candyText) candyText.text = $"Candy: {candies}/{candiesToUnlock}";
        if (timerText) timerText.text = Mathf.CeilToInt(timeLeft).ToString();
    }

    public void OnCandyCollected()
    {
        candies++;
        timeLeft += timePerCandy;
        if (candies >= candiesToUnlock && gate) gate.Unlock();
        UpdateHUD();
    }

    public void TryExit(Vector3 pos)
    {
        if (gate && gate.unlocked)
        {
            float dist = Vector3.Distance(pos, gate.transform.position);
            if (dist < 1.5f) GameOver(true);
        }
    }

    void GameOver(bool win)
    {
        winPanel.SetActive(win);
        losePanel.SetActive(!win);
        Time.timeScale = 0f;
    }
}
