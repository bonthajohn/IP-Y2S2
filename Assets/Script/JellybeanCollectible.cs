using UnityEngine;
using TMPro;

public class JellybeanCollectible : MonoBehaviour
{
    public TextMeshProUGUI feedbackText; // Reference to the UI Text element for feedback
    public float feedbackDuration = 2f; // Duration to show the feedback message
    public Transform jarBottomPosition; // Assign in Inspector: The bottom position inside the jar

    // Dictionary to track the number of jellybeans in each jar
    private static int redJarCount = 0;
    private static int orangeJarCount = 0;
    private static int yellowJarCount = 0;
    private static int greenJarCount = 0;
    private static int blueJarCount = 0;
    private static int purpleJarCount = 0;

    // Constant to determine when the jar is full
    private const int jellybeansRequired = 3;

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
            // Place the jellybean in the jar
            PlaceJellybeanInJar(collision.gameObject);

            // Update the count for the corresponding jar
            UpdateJarCount(jarName);

            // Check if all jars are full
            if (AllJarsFull())
            {
                ShowFeedback("Congratulations! All jars are full!");
            }
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

    private void UpdateJarCount(string jarName)
    {
        // Increase the count for the corresponding jar
        if (jarName == "RedJarCollider") redJarCount++;
        if (jarName == "OrangeJarCollider") orangeJarCount++;
        if (jarName == "YellowJarCollider") yellowJarCount++;
        if (jarName == "GreenJarCollider") greenJarCount++;
        if (jarName == "BlueJarCollider") blueJarCount++;
        if (jarName == "PurpleJarCollider") purpleJarCount++;
    }

    private bool AllJarsFull()
    {
        // Check if all jars have exactly 3 jellybeans
        return redJarCount == jellybeansRequired &&
               orangeJarCount == jellybeansRequired &&
               yellowJarCount == jellybeansRequired &&
               greenJarCount == jellybeansRequired &&
               blueJarCount == jellybeansRequired &&
               purpleJarCount == jellybeansRequired;
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
