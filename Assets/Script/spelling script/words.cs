using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LetterObject : MonoBehaviour
{
    public TextMeshProUGUI letterText; // Assign this in the inspector
    private char assignedLetter;

    // Method to set the letter dynamically
    public void SetLetter(char letter)
    {
        assignedLetter = letter;
        letterText.text = letter.ToString();
    }

    // Retrieve the assigned letter when checking
    public char GetLetter()
    {
        return assignedLetter;
    }
}
