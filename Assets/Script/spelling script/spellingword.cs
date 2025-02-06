using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class WordGame : MonoBehaviour
{
    public TextMeshProUGUI wordText;
    public TextMeshProUGUI timerText;
    public GameObject letterPrefab;
    public Transform letterSpawnArea;

    public int totalSpawnedLetters = 5;
    public float gameDuration = 60f;

    private List<string> easyWords = new List<string>() { "cat", "dog", "hat", "bat", "sun" };
    private List<string> mediumWords = new List<string>() { "banana", "cherry", "pencil", "orange", "rocket" };
    private List<string> hardWords = new List<string>() { "elephant", "computer", "umbrella", "airplane", "mountain" };

    private List<string> currentWordList = new List<string>();
    private string currentWord;
    private string puzzleWord;
    private List<char> missingLetters = new List<char>();
    private List<int> missingLetterPositions = new List<int>();

    private List<char> wrongLetterPool = new List<char>() { 'X', 'Y', 'Z', 'Q', 'V', 'J', 'K', 'M', 'W' };

    private bool gameActive = false;
    private float timeRemaining;
    private int currentWordIndex = 0; // Keeps track of the current word in the list

    void Start()
    {
        // Game starts when the player chooses difficulty
    }

    public void StartGame(float duration)
    {
        gameDuration = duration;
        timeRemaining = gameDuration;
        gameActive = true;
        currentWordIndex = 0; // Reset the word index when starting the game

        UpdateTimerUI(); // Show initial time
        SetWordToPlay(); // Set the first word
        StartCoroutine(GameTimer());
    }

    private void SetWordToPlay()
    {
        if (currentWordList.Count > 0 && currentWordIndex < currentWordList.Count)
        {
            currentWord = currentWordList[currentWordIndex];
            puzzleWord = CreatePuzzleWord(currentWord);
            wordText.text = puzzleWord;
            SpawnLetterChoices();
        }
    }

    private IEnumerator GameTimer()
    {
        while (timeRemaining > 0)
        {
            yield return new WaitForSeconds(1);
            timeRemaining -= 1;
            UpdateTimerUI(); // Update the timer display every second
        }

        EndGame();
    }

    private void UpdateTimerUI()
    {
        if (timerText != null)
        {
            timerText.text = "Time: " + Mathf.Ceil(timeRemaining).ToString() + "s";
        }
    }

    public void EndGame()
    {
        gameActive = false;
        Debug.Log("⏳ Time is up! Game Over.");

        if (timerText != null)
        {
            timerText.text = "Game Over!";
        }
    }

    public void SetEasyDifficulty()
    {
        SetDifficulty(easyWords);
    }

    public void SetMediumDifficulty()
    {
        SetDifficulty(mediumWords);
    }

    public void SetHardDifficulty()
    {
        SetDifficulty(hardWords);
    }

    private void SetDifficulty(List<string> wordList)
    {
        currentWordList = new List<string>(wordList);
        StartGame(gameDuration); // Start the game with the given difficulty level
    }

    private void CheckForCompletion()
    {
        if (missingLetters.Count == 0 && gameActive)
        {
            Debug.Log("✅ Word Completed!");

            // Wait for a short moment to complete the current word before moving on
            Invoke("MoveToNextWord", 1f);
        }
    }

    private void MoveToNextWord()
    {
        if (currentWordIndex < currentWordList.Count - 1)
        {
            currentWordIndex++; // Go to the next word in the list
            SetWordToPlay(); // Load the next word
        }
        else
        {
            Debug.Log("✅ All words completed!");
            EndGame(); // End the game if all words are completed
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Letter"))
        {
            LetterObject letterScript = other.GetComponent<LetterObject>();

            if (letterScript != null)
            {
                char placedLetter = letterScript.GetLetter();

                if (missingLetters.Contains(placedLetter))
                {
                    int letterIndex = missingLetters.IndexOf(placedLetter);
                    int correctPosition = missingLetterPositions[letterIndex];

                    char[] puzzleArray = wordText.text.ToCharArray();
                    puzzleArray[correctPosition] = placedLetter;
                    wordText.text = new string(puzzleArray);

                    missingLetters.RemoveAt(letterIndex);
                    missingLetterPositions.RemoveAt(letterIndex);

                    CheckForCompletion();

                    Destroy(other.gameObject);
                }
            }
        }
    }

    private string CreatePuzzleWord(string word)
    {
        char[] puzzleArray = word.ToCharArray();
        missingLetters.Clear();
        missingLetterPositions.Clear();

        int missingCount = Mathf.Max(1, word.Length / 3);

        for (int i = 0; i < missingCount; i++)
        {
            int randomIndex = Random.Range(0, word.Length);
            if (puzzleArray[randomIndex] != '_')
            {
                missingLetters.Add(word[randomIndex]);
                missingLetterPositions.Add(randomIndex);
                puzzleArray[randomIndex] = '_';
            }
        }

        return new string(puzzleArray);
    }

    private void SpawnLetterChoices()
    {
        List<char> letterChoices = new List<char>();

        foreach (char correctLetter in missingLetters)
        {
            letterChoices.Add(correctLetter);
        }

        while (letterChoices.Count < totalSpawnedLetters)
        {
            char randomWrongLetter = wrongLetterPool[Random.Range(0, wrongLetterPool.Count)];
            if (!letterChoices.Contains(randomWrongLetter))
            {
                letterChoices.Add(randomWrongLetter);
            }
        }

        letterChoices = ShuffleList(letterChoices);

        float spawnRadius = 2.0f;

        foreach (char letter in letterChoices)
        {
            Vector3 randomOffset = new Vector3(
                Random.Range(-spawnRadius, spawnRadius),
                Random.Range(-spawnRadius, spawnRadius),
                0
            );

            Vector3 spawnPosition = letterSpawnArea.position + randomOffset;

            GameObject letterObj = Instantiate(letterPrefab, spawnPosition, Quaternion.identity);
            LetterObject letterScript = letterObj.GetComponent<LetterObject>();

            if (letterScript != null)
            {
                letterScript.SetLetter(letter);
            }
        }
    }

    private List<char> ShuffleList(List<char> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);
            char temp = list[i];
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
        return list;
    }
}
