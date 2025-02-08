using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class CoinGameManager : MonoBehaviour
{
    public ChestCounter cube1;
    public ChestCounter cube2;
    public ChestCounter cube3;
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

        cube1.SetRequiredCount(num1);
        cube2.SetRequiredCount(num2);
        cube3.SetRequiredCount(num3);
    }

    private void OnButtonPressed(SelectEnterEventArgs args)
    {
        CheckResults();
    }

    public void CheckResults()
    {
        int total = cube1.GetCurrentCount() + cube2.GetCurrentCount() + cube3.GetCurrentCount();

        if (total == 10 &&
            cube1.GetCurrentCount() == cube1.requiredCount &&
            cube2.GetCurrentCount() == cube2.requiredCount &&
            cube3.GetCurrentCount() == cube3.requiredCount)
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
