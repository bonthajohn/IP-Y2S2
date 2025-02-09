using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzlePiece : MonoBehaviour
{
    public string requiredTag; // Tag required for the puzzle piece
    private bool isPlacedCorrectly = false;

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag(requiredTag) && !isPlacedCorrectly)
        {
            isPlacedCorrectly = true;
            Debug.Log($"{other.name} is correctly placed!");
        }
    }

    public bool IsPlacedCorrectly()
    {
        return isPlacedCorrectly;
    }

    public void ResetPiece()
    {
        isPlacedCorrectly = false;
    }
}
