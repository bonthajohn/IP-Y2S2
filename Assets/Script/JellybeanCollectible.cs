using UnityEngine;
using TMPro;

public class JellybeanCollectible : MonoBehaviour
{
    public TextMeshProUGUI feedbackText; // Reference to the UI Text element for feedback
    public float feedbackDuration = 2f; // Duration to show the feedback message
    public Transform jarBottomPosition; // Assign in Inspector: The bottom position inside the jar

    private void OnCollisionEnter(Collision collision)
    {
        string tag = collision.gameObject.tag;
        string jarName = gameObject.name;

        // Check if the jellybean matches the jar
        if ((jarName == "RedJarCollider" && tag == "RedJellybean") ||
            (jarName == "OrangeJarCollider" && tag == "OrangeJellybean") ||
            (jarName == "YellowJarCollider" && tag == "YellowJellybean") ||
            (jarName == "GreenJarCollider" && tag == "GreenJellybean") ||
            (jarName == "BlueJarCollider" && tag == "BlueJellybean") ||
            (jarName == "PurpleJarCollider" && tag == "PurpleJellybean"))
        {
            // Move the jellybean to the bottom of the jar
            PlaceJellybeanInJar(collision.gameObject);
        }
        else
        {
            // Show incorrect feedback
            ShowFeedback("Incorrect jar!");
        }
    }

    private void PlaceJellybeanInJar(GameObject jellybean)
    {
        if (jarBottomPosition != null)
        {
            // Move the jellybean to the bottom position
            jellybean.transform.position = jarBottomPosition.position;

            // Disable physics so it stays in place
            Rigidbody rb = jellybean.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = true;
                rb.useGravity = false;
            }
        }
    }

    private void ShowFeedback(string message)
    {
        feedbackText.text = message; // Set the feedback text
        Invoke("HideFeedback", feedbackDuration); // Hide the feedback after a set duration
    }

    private void HideFeedback()
    {
        feedbackText.text = ""; // Clear the feedback message
    }
}
