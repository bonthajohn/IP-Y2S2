using UnityEngine;
using TMPro;

public class PiggyBankCounter : MonoBehaviour
{
    public int requiredCount;
    private int currentCount = 0;
    public TextMeshProUGUI numberText;
    public Transform coinInsertPoint;

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
        numberText.text = requiredCount.ToString();
        currentCount = 0; // Reset count at the start
    }
}
