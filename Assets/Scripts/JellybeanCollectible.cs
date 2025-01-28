using UnityEngine;
using TMPro;

public class JellybeanCollectible : MonoBehaviour
{
    public TextMeshProUGUI feedbackText; // Reference to the UI Text element for feedback
    public float feedbackDuration = 2f; // Duration to show the feedback message

    private void OnCollisionEnter(Collision collision)
    {
        string tag = collision.gameObject.tag;
        string jarName = gameObject.name;

        // Check if the litter matches the bin
        if ((jarName == "RedJarCollider" && tag == "RedJellybean") ||
            (jarName == "OrangeJarCollider" && tag == "OrangeJellybean") ||
            (jarName == "YellowJarCollider" && tag == "YellowJellybean") ||
            (jarName == "GreenJarCollider" && tag == "GreenJellybean") ||
            (jarName == "BlueJarCollider" && tag == "BlueJellybean") ||
            (jarName == "PurpleJarCollider" && tag == "PurpleJellybean"))
        {
            // If the litter matches the bin, add the score and destroy the litter
            Destroy(collision.gameObject);
        }
        else
        {
            // If the litter does not match the bin, show the "Incorrect bin!" message
            ShowFeedback("Incorrect jar!");
        }
    }

    // Method to display feedback
    private void ShowFeedback(string message)
    {
        feedbackText.text = message; // Set the feedback text
        Invoke("HideFeedback", feedbackDuration); // Hide the feedback after a set duration
    }

    // Method to hide the feedback message
    private void HideFeedback()
    {
        feedbackText.text = ""; // Clear the feedback message
    }
}
