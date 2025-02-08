using UnityEngine;
using TMPro;

public class ChestCounter : MonoBehaviour
{
    public int requiredCount;  // Randomized number from GameManager
    private int currentCount = 0;
    public TextMeshProUGUI numberText;  // Assign in Inspector

    public void AddCoin()
    {
        currentCount++;
    }

    public int GetCurrentCount()
    {
        return currentCount;
    }

    public void SetRequiredCount(int newCount)
    {
        requiredCount = newCount;
        numberText.text = requiredCount.ToString(); // Update the text on top of cube
    }
}
