using UnityEngine;
using TMPro;

public class ObjectPlacerWithScore : MonoBehaviour
{
    public TMP_Text scoreText; // Reference to the TextMeshPro text element
    public int defaultPoints = 10; // Default points for scorable objects

    private int score = 0; // Current score

    private void Start()
    {
        UpdateScoreText(); // Initialize score text
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check if the object has a specific tag or component for scoring
        if (other.CompareTag("Scorable"))
        {
            // Add default points to the score
            AddScore(defaultPoints);

            // Optionally destroy the object after placement
            Destroy(other.gameObject);
        }
    }

    private void AddScore(int points)
    {
        score += points; // Update score
        UpdateScoreText(); // Refresh displayed score
    }

    private void UpdateScoreText()
    {
        if (scoreText != null)
        {
            scoreText.text = "Score: " + score;
        }
    }
}
