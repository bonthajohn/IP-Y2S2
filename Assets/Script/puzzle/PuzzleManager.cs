using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleManager : MonoBehaviour
{
    [Header("Puzzle Settings")]
    public Puzzle[] puzzles; // Array of puzzle sets, each containing puzzle pieces

    private void Start()
    {
        // Initialize puzzles if needed
        foreach (var puzzle in puzzles)
        {
            if (puzzle.puzzlePieces.Length == 0)
            {
                Debug.LogError("Puzzle is missing puzzle pieces!");
            }
        }
    }

    private void Update()
    {
        // Check if all puzzles are completed
        foreach (var puzzle in puzzles)
        {
            if (IsPuzzleCompleted(puzzle))
            {
                Debug.Log($"{puzzle.puzzleName} is complete!");
                // Trigger puzzle completion actions here (e.g., unlock next puzzle, etc.)
            }
        }
    }

    private bool IsPuzzleCompleted(Puzzle puzzle)
    {
        int placedPiecesCount = 0;
        foreach (var piece in puzzle.puzzlePieces)
        {
            if (piece.IsPlacedCorrectly())
            {
                placedPiecesCount++;
            }
        }

        // If all pieces are placed correctly, the puzzle is complete
        return placedPiecesCount == puzzle.puzzlePieces.Length;
    }

    public void ResetPuzzle(int puzzleIndex)
    {
        if (puzzleIndex >= 0 && puzzleIndex < puzzles.Length)
        {
            Puzzle puzzle = puzzles[puzzleIndex];
            foreach (var piece in puzzle.puzzlePieces)
            {
                piece.ResetPiece();
            }
        }
    }
}

[System.Serializable]
public class Puzzle
{
    public string puzzleName; // Name for the puzzle (for better identification)
    public PuzzlePiece[] puzzlePieces; // Array of puzzle pieces for this specific puzzle
}
