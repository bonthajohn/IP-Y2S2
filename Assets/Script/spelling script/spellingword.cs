using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Firebase;
using Firebase.Database;
using Firebase.Extensions;

public class WordGame : MonoBehaviour
{
    public TextMeshProUGUI wordText;
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI scoreText;  // 🏆 Added to show score
    public GameObject letterPrefab;
    public Transform letterSpawnArea;

    public int totalSpawnedLetters = 5;
    public float gameDuration = 60f;

    private List<string> easyWords = new List<string>();
    private List<string> mediumWords = new List<string>();
    private List<string> hardWords = new List<string>();

    private List<string> currentWordList = new List<string>();
    private string currentWord;
    private string puzzleWord;
    private List<char> missingLetters = new List<char>();
    private List<int> missingLetterPositions = new List<int>();

    private List<char> wrongLetterPool = new List<char>() { 'X', 'Y', 'Z', 'Q', 'V', 'J', 'K', 'M', 'W' };

    private bool gameActive = false;
    private float timeRemaining;
    private int currentWordIndex = 0;

    private int score = 0;  // 🏆 Added score variable

    void Start()
    {
        LoadWordsFromFirebase();
    }

    private void LoadWordsFromFirebase()
    {
        FirebaseDatabase.DefaultInstance
            .GetReference("words")
            .GetValueAsync().ContinueWithOnMainThread(task =>
            {
                if (task.IsFaulted)
                {
                    Debug.LogError("❌ Failed to load words from Firebase.");
                    return;
                }
                else if (task.IsCompleted)
                {
                    DataSnapshot snapshot = task.Result;
                    easyWords = ShuffleList(ExtractWordList(snapshot, "easy"));
                    mediumWords = ShuffleList(ExtractWordList(snapshot, "medium"));
                    hardWords = ShuffleList(ExtractWordList(snapshot, "hard"));

                    Debug.Log("✅ Words loaded and randomized from Firebase.");
                }
            });
    }

    private List<string> ExtractWordList(DataSnapshot snapshot, string difficulty)
    {
        List<string> words = new List<string>();
        if (snapshot.HasChild(difficulty))
        {
            foreach (DataSnapshot child in snapshot.Child(difficulty).Children)
            {
                words.Add(child.Value.ToString());
            }
        }
        return words;
    }

    public void StartGame(float duration)
    {
        gameDuration = duration;
        timeRemaining = gameDuration;
        gameActive = true;
        currentWordIndex = 0;
        score = 0;  // Reset score when starting a new game
        UpdateScoreUI();  // Display initial score
        UpdateTimerUI();  // Show initial time
        SetWordToPlay();
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
            UpdateTimerUI();
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

        if (score >= 100)
        {
            Debug.Log("🏆 You Win!");
        }
        else
        {
            Debug.Log("❌ Try Again!");
        }
    }

    public void SetEasyDifficulty()
    {
        SetDifficulty(easyWords, 10);  // Easy gives 10 points per word
    }

    public void SetMediumDifficulty()
    {
        SetDifficulty(mediumWords, 20);  // Medium gives 20 points per word
    }

    public void SetHardDifficulty()
    {
        SetDifficulty(hardWords, 30);  // Hard gives 30 points per word
    }

    private void SetDifficulty(List<string> wordList, int pointsPerWord)
    {
        if (wordList.Count == 0)
        {
            Debug.LogError("⚠ No words found for this difficulty.");
            return;
        }

        currentWordList = ShuffleList(new List<string>(wordList));  // Randomize words before starting
        StartGame(gameDuration);
    }

    private void CheckForCompletion(int pointsPerWord)
    {
        if (missingLetters.Count == 0 && gameActive)
        {
            Debug.Log("✅ Word Completed!");
            score += pointsPerWord;  // Add points based on difficulty
            UpdateScoreUI();  // Update the score UI
            Invoke("MoveToNextWord", 1f);
        }
    }

    private void MoveToNextWord()
    {
        if (currentWordIndex < currentWordList.Count - 1)
        {
            currentWordIndex++;
            SetWordToPlay();
        }
        else
        {
            Debug.Log("✅ All words completed!");
            EndGame();
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

                    CheckForCompletion(10);  // Pass points based on difficulty (for now, 10 for example)
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
        foreach (Transform child in letterSpawnArea)
        {
            Destroy(child.gameObject);
        }

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
                Random.Range(-spawnRadius, spawnRadius), // Random X position
                0, // Fixed Y position
                0
            );

            Vector3 spawnPosition = new Vector3(
                letterSpawnArea.position.x + randomOffset.x,
                letterSpawnArea.position.y, // Keep the same Y position
                letterSpawnArea.position.z
            );

            GameObject letterObj = Instantiate(letterPrefab, spawnPosition, Quaternion.identity, letterSpawnArea);
            LetterObject letterScript = letterObj.GetComponent<LetterObject>();

            if (letterScript != null)
            {
                letterScript.SetLetter(letter);
            }
        }

    }

    private List<T> ShuffleList<T>(List<T> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);
            T temp = list[i];
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
        return list;
    }

    private void UpdateScoreUI()
    {
        if (scoreText != null)
        {
            scoreText.text = "Score: " + score.ToString();
        }
    }
}