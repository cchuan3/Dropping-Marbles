using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // Detect if pause or menu is called
	static bool pauseCalled = false;
    static bool menuCalled = false;
    public static bool paused = false;
    static bool gameOver = false;

    // Menus
    public Canvas pauseMenu;
    public Canvas statPanel;
    public Canvas levelMenu;
    public Canvas gameOverMenu;

    // Game vars
    public static float timer = 0;
    public static int score = 0;
    public static int level = 0;

    // For score
    private static float lastLevelStart = 0;
    public Text scoreDisplay;
    private bool inGame = false;
    public Text levelDisplay;

    // Player stats
    public PlayerControl player;
    public Text strengthDisplay;
    public Text speedDisplay;
    public Text controlDisplay;
    public Button levelOption1;
    public Button levelOption2;
    int[] statChange1;
    int[] statChange2;

    // Chaser
    public ChaserControl chaser;

    // Progress bar
    public ProgressBarControl playerProgress;
    public ProgressBarControl chaserProgress;
    private float levelStart = 0;

    // Level prefab
    public GameObject levelPrefab;

    // Loaded levels
    List<GameObject> loadedLevels = new List<GameObject>();

    private void Start() {
        NextLevel();
        DisplayScore(score);
        DisplayLevel();
        inGame = true;
    }

    private void Update() {
        // Increment timer
        timer += Time.deltaTime;

        // Increment Score
        if (inGame) {
            DisplayScore2();
        }

        // Bring menu up if Escape is pressed
        if (Input.GetKeyDown(KeyCode.Escape)) {
            Menu();
        }
        
        // Toggle pause
        if (pauseCalled) {
            Time.timeScale = (1 + Time.timeScale) % 2;
            paused = !paused;
            pauseCalled = !pauseCalled;
        }

        // Toggle menu
        if (menuCalled && inGame) {
            menuCalled = !menuCalled;
            Pause();
            pauseMenu.enabled = !pauseMenu.enabled;
            DisplayStats();
        }

        // Toggle game over
        if (gameOver) {
            gameOverMenu.enabled = true;
        }

        // Update progress bars
        ProgressBar();
    }

    // Allow other scripts to call pause
    public void Pause() {
        pauseCalled = true;
    }

    // Allow other scripts to call menu
    public void Menu() {
        menuCalled = true;
    }

    // Call game over scene
    private void GameOver() {
        gameOver = true;
        Pause();
    }

    // Disable menus
    private void DisableMenus() {
        pauseMenu.enabled = false;
        gameOverMenu.enabled = false;
        statPanel.enabled = false;
    }

    // End of level
    public void EndLevel() {
        NextProgressBar();
        CalculateScore();
        DisplayLevel();
        LevelUp();
        if (level > 1) {
            chaser.JumpToLevel(level);
        }
    }

    // Levelup menu
    private void LevelUp() {
        Pause();
        statChange1 = GenerateLevelup();
        statChange2 = GenerateLevelup();
        DisplayLevelup(levelOption1, statChange1);
        DisplayLevelup(levelOption2, statChange2);
        levelMenu.enabled = true;
        inGame = false;
    }

    // Generate levelup stats
    private int[] GenerateLevelup() {
        int increaseNum = Random.Range(1, Mathf.Max(1, Mathf.FloorToInt(level / 2)));
        int decreaseNum = 0;
        if (Random.Range(0, 6) < 2) {
            increaseNum++;
        }
        if (increaseNum >= 3 && Random.Range(0, 5) != 0) {
            decreaseNum = -Random.Range(1, Mathf.Max(1, Mathf.FloorToInt(level / 3)));
        }
        int[] statChange = new int[3];
        int increaseStat = Random.Range(0, 3);
        int decreaseStat = Random.Range(1, 3);
        statChange[increaseStat] = increaseNum;
        statChange[(increaseStat + decreaseStat) % 3] = decreaseNum;
        return statChange;
    }

    // Change button display
    private void DisplayLevelup(Button b, int[] statChange) {
        Text[] bText = b.GetComponentsInChildren<Text>();
        for (int i = 0; i < 3; i++) {
            Text curr = bText[i+1];
            curr.color = Color.gray;
            curr.text = ": " + statChange[i].ToString();
            if (statChange[i] > 0) {
                curr.color = Color.green;
            }
            else if (statChange[i] < 0) {
                curr.color = Color.red;
            }
        }
    }

    // On choosing level up option
    public void IncreaseStats(int option) {
        if (option == 1) {
            player.ChangeStrength(statChange1[0]);
            player.ChangeSpeed(statChange1[1]);
            player.ChangeControl(statChange1[2]);
        }
        else if (option == 2) {
            player.ChangeStrength(statChange2[0]);
            player.ChangeSpeed(statChange2[1]);
            player.ChangeControl(statChange2[2]);
        }
        player.CalculateStats();
        levelMenu.enabled = false;
        inGame = true;
        Pause();
    }

    // End current game
    public void EndGame() {
        inGame = false;
        FinalScore();
        // DisplayScore();
        DisplayStats();
        GameOver();
    }

    public void Restart() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name, LoadSceneMode.Single);
        NextLevelNoLevelup();
        timer = 0;
        score = 0;
        level = 0;
        inGame = true;
        gameOver = false;
        Pause();
        DisableMenus();
    }

    // Allow other scripts to quit game
    public void Quit() {
        Application.Quit();
        // UnityEditor.EditorApplication.isPlaying = false; // For editor
    }

    // Instantiate the next level
    public void NextLevel() {
        level++;
        GameObject nextLevel = Instantiate(levelPrefab, new Vector3(0, 0, (level) * 210), Quaternion.identity);
        nextLevel.GetComponent<LevelManager>().level = level+1;
        nextLevel.GetComponent<LevelManager>().timer = timer;
        loadedLevels.Add(nextLevel);
        DestroyOldLevel();
    }

    private void NextLevelNoLevelup() {
        GameObject nextLevel = Instantiate(levelPrefab, new Vector3(0, 0, (level) * 210), Quaternion.identity);
        nextLevel.GetComponent<LevelManager>().level = level+1;
        nextLevel.GetComponent<LevelManager>().timer = timer;
        loadedLevels.Add(nextLevel);
        DestroyOldLevel();
    }

    // Destroy old level
    private void DestroyOldLevel() {
        if (loadedLevels.Capacity > 6) {
            GameObject target = loadedLevels[0];
            loadedLevels.RemoveAt(0);
            Destroy(target);
        }
    }

    // Calculate current score
    private void CalculateScore() {
        float lastLevelEnd = timer;
        int levelsCompleted = level - 1;
        if (levelsCompleted > 0) {
            score += 1000;
            score += (int) ((float) levelsCompleted * (60 / (lastLevelEnd - lastLevelStart)));
        }
        lastLevelStart = timer;
        if (score > 999999) {
            score = 999999;
        }
        DisplayScore((score + 50 * (int) (timer / 2)));
    }

    // Calculate final score
    private void FinalScore() {
        float lastLevelEnd = timer;
        int levelsCompleted = level - 1;
        score += 50 * (int) (lastLevelEnd / 2);
        score += (int) (lastLevelEnd / 10);
        score += (int) ((float) levelsCompleted * (60 / (lastLevelEnd - lastLevelStart)));
        lastLevelStart = timer;
        if (score > 999999) {
            score = 999999;
        }
        DisplayScore(score);
    }

    private void DisplayScore(int levelScore) {
        scoreDisplay.text = "Score: " + levelScore.ToString();
    }

    private void DisplayScore2() {
        int levelScore = (score + 50 * (int) (timer / 2));
        scoreDisplay.text = "Score: " + levelScore.ToString();
    }

    private void DisplayLevel() {
        levelDisplay.text = "Level: " + level.ToString();
    }

    private void DisplayStats() {
        int[] stats = player.GetStats();
        strengthDisplay.text = ":" + stats[0].ToString();
        speedDisplay.text = ":" + stats[1].ToString();
        controlDisplay.text = ":" + stats[2].ToString();
        statPanel.enabled = !statPanel.enabled;
    }

    // Get start point for progress bar
    private void NextProgressBar() {
        levelStart = (level - 1) * 210;
    }

    private void ProgressBar() {
        // float levelStart = (level - 1) * 210;
        float playerPos = (player.GetProgress() - levelStart) / 210;
        float chaserPos = (chaser.GetProgress() - levelStart) / 210;
        playerProgress.CurrVal = playerPos;
        chaserProgress.CurrVal = chaserPos;
    }
}

