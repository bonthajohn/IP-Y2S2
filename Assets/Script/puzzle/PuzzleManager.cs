using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleManager : MonoBehaviour
{
    [Header("Puzzle Settings")]
    public PuzzleSlot[] puzzleSlots; // Array of slots, each linking to a puzzle piece

    private void Start()
    {
        // Initialize puzzle slots
        foreach (var slot in puzzleSlots)
        {
            if (slot.slotObject == null)
            {
                Debug.LogError("A slot is missing its assigned GameObject!");
            }
        }
    }

    public void CheckPuzzleCompletion()
    {
        foreach (var slot in puzzleSlots)
        {
            if (!slot.IsPieceCorrect())
            {
                Debug.Log("Puzzle is not yet complete!");
                return;
            }
        }
        Debug.Log("Puzzle is complete!");
        // Trigger puzzle completion actions here
    }

    public void ResetPuzzle()
    {
        foreach (var slot in puzzleSlots)
        {
            slot.ResetSlot();
        }
    }
}

[System.Serializable]
public class PuzzleSlot
{
    public GameObject slotObject; // The actual GameObject representing the slot
    public PuzzlePiece puzzlePiece; // The puzzle piece that should go in this slot

    public bool IsPieceCorrect()
    {
        if (puzzlePiece == null || slotObject == null) return false;
        return Vector3.Distance(puzzlePiece.transform.position, slotObject.transform.position) < 0.1f;
    }

    public void ResetSlot()
    {
        if (puzzlePiece != null)
        {
            puzzlePiece.ResetPiece();
        }
    }
}
