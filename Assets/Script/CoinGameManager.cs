using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class CoinGameManager : MonoBehaviour
{
    public PiggyBankCounter piggyBank1;
    public PiggyBankCounter piggyBank2;
    public PiggyBankCounter piggyBank3;
    public TextMeshProUGUI messageText;
    public XRBaseInteractable submitButton; // Reference to XR UI Button

    private void Start()
    {
        submitButton.selectEntered.AddListener(OnButtonPressed); // VR button interaction
        RandomizeNumbers();
    }

    private void RandomizeNumbers()
    {
        int num1 = Random.Range(1, 8);
        int num2 = Random.Range(1, 10 - num1);
        int num3 = 10 - (num1 + num2);

        piggyBank1.SetRequiredCount(num1);
        piggyBank2.SetRequiredCount(num2);
        piggyBank3.SetRequiredCount(num3);
    }

    private void OnButtonPressed(SelectEnterEventArgs args)
    {
        CheckResults();
    }

    public void CheckResults()
    {
        int total = piggyBank1.GetCurrentCount() + piggyBank2.GetCurrentCount() + piggyBank3.GetCurrentCount();

        if (total == 10 &&
            piggyBank1.GetCurrentCount() == piggyBank1.requiredCount &&
            piggyBank2.GetCurrentCount() == piggyBank2.requiredCount &&
            piggyBank3.GetCurrentCount() == piggyBank3.requiredCount)
        {
            messageText.text = "Correct! Well done!";
        }
        else
        {
            messageText.text = "Incorrect. Restarting...";
            Invoke("RestartGame", 2f);
        }
    }

    private void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
