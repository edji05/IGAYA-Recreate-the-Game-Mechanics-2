using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // --- Assign in Inspector ---
    [Header("Spawning References")]
    public GameObject obstaclePrefab;
    public GameObject collectiblePrefab;
    public Transform centerPoint;
    public GameObject gameOverPanel; // UI Panel to show on Game Over

    // --- Difficulty Configuration ---
    [Header("Difficulty Scaling")]
    public float baseSpawnInterval = 1.5f;
    public float baseSpeed = 3f;
    public float spawnDistance = 5f;
    public float speedIncreasePerPoint = 0.1f;
    public float intervalDecreasePerPoint = 0.05f;
    public float minSpawnInterval = 0.5f;

    private float currentSpawnInterval;
    private float timer;

    void Start()
    {
        Time.timeScale = 1; // Ensure the game starts running
        timer = baseSpawnInterval;

        // Ensure the Game Over panel is hidden at the start
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }
    }

    void Update()
    {
        // --- Spawning and Difficulty Logic (Unchanged) ---
        int currentScore = PlayerOrbit.CurrentScore;

        float desiredInterval = baseSpawnInterval - (currentScore * intervalDecreasePerPoint);
        currentSpawnInterval = Mathf.Max(desiredInterval, minSpawnInterval);

        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            SpawnNewItem(currentScore);
            timer = currentSpawnInterval;
        }
    }

    void SpawnNewItem(int currentScore)
    {
        float dynamicSpeed = baseSpeed + (currentScore * speedIncreasePerPoint);

        float randomAngle = Random.Range(0f, 360f);
        float radians = randomAngle * Mathf.Deg2Rad;
        float posX = centerPoint.position.x + spawnDistance * Mathf.Cos(radians);
        float posY = centerPoint.position.y + spawnDistance * Mathf.Sin(radians);
        Vector3 spawnPosition = new Vector3(posX, posY, 0);

        GameObject itemToSpawn = (Random.value < 0.6f) ? collectiblePrefab : obstaclePrefab;

        GameObject newItem = Instantiate(itemToSpawn, spawnPosition, Quaternion.identity);

        ItemMover mover = newItem.AddComponent<ItemMover>();
        mover.centerPoint = centerPoint;
        mover.moveSpeed = dynamicSpeed;
    }

    // Function called by PlayerOrbit when an obstacle is hit
    public void EndGame()
    {
        Debug.Log("Game Over Initiated. Final Score: " + PlayerOrbit.CurrentScore);
        Time.timeScale = 0; // Freeze the game

        // 🚨 FIX: Activate the Game Over panel 🚨
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }
    }

    // Function called by the UI Button to restart the game
    public void RestartGame()
    {
        Time.timeScale = 1; // Unfreeze time
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}