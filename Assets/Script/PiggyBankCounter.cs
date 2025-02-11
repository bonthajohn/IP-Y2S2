using UnityEngine;
using TMPro;

public class PiggyBankCounter : MonoBehaviour
{
    public int requiredCount;  // Randomized number from GameManager
    private int currentCount = 0;
    public TextMeshProUGUI numberText;  // Assign in Inspector
    public Transform coinInsertPoint;  // Assign a Transform representing the coin slot

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
        numberText.text = requiredCount.ToString(); // Update the text on top of piggy bank
    }
}
