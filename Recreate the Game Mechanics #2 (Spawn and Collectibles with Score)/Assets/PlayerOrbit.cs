using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class PlayerOrbit : MonoBehaviour
{
    // --- Configuration (Set in Inspector) ---
    public Transform centerPoint;
    public float orbitRadius = 2.5f;
    public float angularSpeed = 300f;
    public TextMeshProUGUI scoreText;

    // --- Static Property (Accessible by Spawner) ---
    public static int CurrentScore { get; private set; } = 0;

    // --- Private Variables ---
    private GameManager gameManager;
    private float currentAngle;

    void Start()
    {
        if (centerPoint == null)
        {
            Debug.LogError("ERROR: CenterPoint is not assigned! Orbit calculation will fail.");
            return;
        }

        // Get reliable reference to the GameManager
        gameManager = FindObjectOfType<GameManager>();
        if (gameManager == null)
        {
            Debug.LogError("ERROR: Cannot find GameManager in the scene! Game Over will not function.");
        }

        CurrentScore = 0;
        UpdateScoreText();

        Vector3 initialDirection = transform.position - centerPoint.position;
        currentAngle = Mathf.Atan2(initialDirection.y, initialDirection.x) * Mathf.Rad2Deg;
    }

    void Update()
    {
        // === 1. Input: Reverse Orbit Direction (Click/Tap) ===
        if (Input.GetMouseButtonDown(0))
        {
            angularSpeed *= -1;
        }

        // === 2. Continuous Orbit Calculation ===
        currentAngle += angularSpeed * Time.deltaTime;
        float radians = currentAngle * Mathf.Deg2Rad;

        float posX = centerPoint.position.x + orbitRadius * Mathf.Cos(radians);
        float posY = centerPoint.position.y + orbitRadius * Mathf.Sin(radians);

        transform.position = new Vector2(posX, posY);
    }

    // === 3. Game Over (Physical Hit with Obstacle) ===
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Obstacle"))
        {
            // 1. Call the Game Over function immediately
            if (gameManager != null)
            {
                gameManager.EndGame();
            }

            // 2. Destroy the player's Rigidbody to instantly stop movement
            Rigidbody2D rb = GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                Destroy(rb);
            }

            // 3. Destroy the obstacle after a slight delay to ensure the EndGame call registers
            Destroy(collision.gameObject, 0.05f);
        }
    }

    // === 4. Score Point (Pass Through Collectible) ===
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Collectible"))
        {
            CurrentScore += 1;
            UpdateScoreText();

            Destroy(other.gameObject);
        }
    }

    void UpdateScoreText()
    {
        if (scoreText != null)
        {
            scoreText.text = CurrentScore.ToString();
        }
    }
}