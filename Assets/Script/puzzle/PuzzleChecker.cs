using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzlePiece : MonoBehaviour
{
    public string requiredTag; // Tag required for the puzzle piece
    public Transform correctPosition; // The correct position for the piece
    public float positionTolerance = 1f; // Allowable distance from correct position for "correct placement"

    private bool isPlacedCorrectly = false;

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag(requiredTag) && !isPlacedCorrectly)
        {
            CheckPosition(other.transform);
        }
    }

    private void CheckPosition(Transform piece)
    {
        // Check if the piece is close enough to the correct position
        if (Vector3.Distance(piece.position, correctPosition.position) < positionTolerance)
        {
            isPlacedCorrectly = true;
            Debug.Log($"{piece.name} is correctly placed!");
        }
    }

    public bool IsPlacedCorrectly()
    {
        return isPlacedCorrectly;
    }

    public void ResetPiece()
    {
        // Reset the piece to its original position if needed
        transform.position = correctPosition.position;
        transform.rotation = correctPosition.rotation;
        isPlacedCorrectly = false;
    }
}
