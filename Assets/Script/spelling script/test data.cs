using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Database;
using Firebase.Extensions;

public class FirebaseWordUploader : MonoBehaviour
{
    private DatabaseReference dbRef;

    private List<string> easyWords = new List<string>()
    {
        "cat", "dog", "hat", "bat", "sun", "cup", "pen", "box", "fish", "tree", "moon", "car", "bus", "book", "lamp"
    };

    private List<string> mediumWords = new List<string>()
    {
        "banana", "cherry", "pencil", "orange", "rocket", "guitar", "castle", "tunnel", "planet", "glacier", "ladder", "basket", "window", "bottle", "jungle"
    };

    private List<string> hardWords = new List<string>()
    {
        "elephant", "computer", "umbrella", "airplane", "mountain", "microscope", "kangaroo", "astronaut", "volcano", "architecture", "laboratory", "mechanism", "waterfall", "satellite", "telescope"
    };

    void Start()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            if (task.Result == DependencyStatus.Available)
            {
                dbRef = FirebaseDatabase.DefaultInstance.RootReference;
                UploadWords();
            }
            else
            {
                Debug.LogError("Could not resolve all Firebase dependencies.");
            }
        });
    }

    void UploadWords()
    {
        Dictionary<string, object> wordsData = new Dictionary<string, object>
        {
            { "easy", easyWords },
            { "medium", mediumWords },
            { "hard", hardWords }
        };

        dbRef.Child("words").SetValueAsync(wordsData).ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted)
            {
                Debug.Log("Words uploaded successfully.");
            }
            else
            {
                Debug.LogError("Failed to upload words: " + task.Exception);
            }
        });
    }
}
