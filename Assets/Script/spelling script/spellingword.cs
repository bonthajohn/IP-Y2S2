using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class WordGame : MonoBehaviour
{
    public TextMeshProUGUI wordText;   // UI Text to display the puzzle word
    public GameObject letterPrefab;    // Prefab for draggable letters

    private List<string> words = new List<string>(); // List to store words
    private string currentWord;
    private string puzzleWord;           // Word with missing letters
    private List<char> missingLetters = new List<char>(); // Stores missing letters

    void Start()
    {
        // Static word list (you can add more words here)
        words.Add("apple");
        words.Add("banana");
        words.Add("cherry");
        words.Add("elephant");
        words.Add("computer");

        GenerateNewWord(); // Generate a word puzzle when the game starts
    }

    void GenerateNewWord()
    {
        if (words.Count == 0) return;

        int randomIndex = Random.Range(0, words.Count);  // Pick a random word
        currentWord = words[randomIndex]; // Get the random word

        puzzleWord = CreatePuzzleWord(currentWord); // Generate the word with missing letters
        wordText.text = puzzleWord; // Display it on the UI
        missingLetters.Clear(); // Reset missing letters list

        GenerateLetterObjects(); // Spawn missing letters as objects
    }

    string CreatePuzzleWord(string word)
    {
        char[] puzzleArray = word.ToCharArray();
        int missingCount = Mathf.Max(1, word.Length / 3); // Remove 1/3 of letters (at least 1)

        for (int i = 0; i < missingCount; i++)
        {
            int randomIndex = Random.Range(0, word.Length);
            if (puzzleArray[randomIndex] != '_') // Ensure unique letters are removed
            {
                missingLetters.Add(word[randomIndex]); // Store the missing letter
                puzzleArray[randomIndex] = '_'; // Replace with _
            }
        }

        return new string(puzzleArray);
    }

    void GenerateLetterObjects()
    {
        // Clear any existing objects before generating new ones
        Transform existingLetters = transform.Find("Letters");
        if (existingLetters != null)
        {
            DestroyImmediate(existingLetters.gameObject); // Clean up previous letters
        }

        // Create an empty container to hold the spawned letters in the Hierarchy
        GameObject lettersContainer = new GameObject("Letters");
        lettersContainer.transform.SetParent(transform);

        foreach (char letter in missingLetters)
        {
            // Debug log to see when we start spawning letters
            Debug.Log($"Spawning letter: {letter}");

            // Instantiate the letter prefab at a random position
            GameObject letterObj = Instantiate(letterPrefab, RandomSpawnPosition(), Quaternion.identity);

            if (letterObj != null)
            {
                // Set the letter's text
                letterObj.GetComponentInChildren<TextMeshPro>().text = letter.ToString();

                // Add rigidbody for interaction (optional)
                letterObj.AddComponent<Rigidbody>();
                letterObj.AddComponent<BoxCollider>();

                // Set the tag for detection
                letterObj.tag = "Letter";

                // Set this letter as a child of the container to keep it organized in the hierarchy
                letterObj.transform.SetParent(lettersContainer.transform);
            }
            else
            {
                Debug.LogError("Failed to instantiate letter prefab.");
            }
        }
    }

    Vector3 RandomSpawnPosition()
    {
        // Ensure the spawned objects are within camera view
        float xPos = Random.Range(-5f, 5f);
        float zPos = Random.Range(-5f, 5f);
        return new Vector3(xPos, 1f, zPos); // Adjust the y value if needed
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Letter"))
        {
            string placedLetter = other.GetComponentInChildren<TextMeshPro>().text;
            if (missingLetters.Contains(placedLetter[0])) // Check if letter is needed
            {
                missingLetters.Remove(placedLetter[0]); // Mark as placed
                Destroy(other.gameObject); // Remove from scene

                // Check if all missing letters are placed
                if (missingLetters.Count == 0)
                {
                    wordText.text = currentWord; // Display full word
                    Debug.Log("✅ Word Completed!");
                }
            }
        }
    }
}
