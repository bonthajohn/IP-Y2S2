using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Auth;
using Firebase.Database;
using TMPro;
using System.Threading.Tasks;

public class AuthManager : MonoBehaviour
{
    // Firebase variables
    [Header("Firebase")]
    public DependencyStatus dependencyStatus;
    public FirebaseAuth auth;
    public FirebaseUser User;
    public DatabaseReference dbReference;

    // Login variables
    [Header("Login")]
    public TMP_InputField emailLoginField;
    public TMP_InputField passwordLoginField;
    public TMP_Text warningLoginText;
    public TMP_Text confirmLoginText;

    // Register variables
    [Header("Register")]
    public TMP_InputField usernameRegisterField;
    public TMP_InputField emailRegisterField;
    public TMP_InputField passwordRegisterField;
    public TMP_InputField passwordRegisterVerifyField;
    public TMP_Text warningRegisterText;

    void Awake()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
        {
            dependencyStatus = task.Result;
            if (dependencyStatus == DependencyStatus.Available)
            {
                InitializeFirebase();
            }
            else
            {
                Debug.LogError("Could not resolve all Firebase dependencies: " + dependencyStatus);
            }
        });
    }

    private void InitializeFirebase()
    {
        Debug.Log("Setting up Firebase Auth and Database");
        auth = FirebaseAuth.DefaultInstance;
        dbReference = FirebaseDatabase.DefaultInstance.RootReference;
    }

    public void LoginButton()
    {
        StartCoroutine(Login(emailLoginField.text, passwordLoginField.text));
    }

    public void RegisterButton()
    {
        StartCoroutine(Register(emailRegisterField.text, passwordRegisterField.text, usernameRegisterField.text));
    }

    private IEnumerator Login(string _email, string _password)
    {
        Task<AuthResult> LoginTask = auth.SignInWithEmailAndPasswordAsync(_email, _password);
        yield return new WaitUntil(() => LoginTask.IsCompleted);

        if (LoginTask.Exception != null)
        {
            Debug.LogWarning($"Failed to login task with {LoginTask.Exception}");
            FirebaseException firebaseEx = LoginTask.Exception.GetBaseException() as FirebaseException;
            AuthError errorCode = (AuthError)firebaseEx.ErrorCode;

            string message = "Login Failed!";
            switch (errorCode)
            {
                case AuthError.MissingEmail: message = "Missing Email"; break;
                case AuthError.MissingPassword: message = "Missing Password"; break;
                case AuthError.WrongPassword: message = "Wrong Password"; break;
                case AuthError.InvalidEmail: message = "Invalid Email"; break;
                case AuthError.UserNotFound: message = "Account does not exist"; break;
            }
            warningLoginText.text = message;
        }
        else
        {
            User = LoginTask.Result.User;
            Debug.LogFormat("User signed in successfully: {0} ({1})", User.DisplayName, User.Email);
            warningLoginText.text = "";
            confirmLoginText.text = "Logged In";
        }
    }

    private IEnumerator Register(string _email, string _password, string _username)
    {
        if (_username == "")
        {
            warningRegisterText.text = "Missing Username";
        }
        else if (passwordRegisterField.text != passwordRegisterVerifyField.text)
        {
            warningRegisterText.text = "Password Does Not Match!";
        }
        else
        {
            Task<AuthResult> RegisterTask = auth.CreateUserWithEmailAndPasswordAsync(_email, _password);
            yield return new WaitUntil(() => RegisterTask.IsCompleted);

            if (RegisterTask.Exception != null)
            {
                Debug.LogWarning($"Failed to register task with {RegisterTask.Exception}");
                FirebaseException firebaseEx = RegisterTask.Exception.GetBaseException() as FirebaseException;
                AuthError errorCode = (AuthError)firebaseEx.ErrorCode;

                string message = "Register Failed!";
                switch (errorCode)
                {
                    case AuthError.MissingEmail: message = "Missing Email"; break;
                    case AuthError.MissingPassword: message = "Missing Password"; break;
                    case AuthError.WeakPassword: message = "Weak Password"; break;
                    case AuthError.EmailAlreadyInUse: message = "Email Already In Use"; break;
                }
                warningRegisterText.text = message;
            }
            else
            {
                User = RegisterTask.Result.User;

                if (User != null)
                {
                    UserProfile profile = new UserProfile { DisplayName = _username };
                    Task ProfileTask = User.UpdateUserProfileAsync(profile);
                    yield return new WaitUntil(() => ProfileTask.IsCompleted);

                    if (ProfileTask.Exception != null)
                    {
                        Debug.LogWarning($"Failed to register task with {ProfileTask.Exception}");
                        warningRegisterText.text = "Username Set Failed!";
                    }
                    else
                    {
                        StartCoroutine(SaveUserData(_username, User.UserId));
                        warningRegisterText.text = "";
                        UIManager.instance.LoginScreen();
                    }
                }
            }
        }
    }

    // Save user data to Firebase
    private IEnumerator SaveUserData(string username, string uid)
    {
        // Create an instance of the Progress class
        Progress progress = new Progress();

        // Create a dictionary for the user data
        var playerData = new Dictionary<string, object>
        {
            { "name", username },
            { "Uid", uid },
            { "badges", new List<string>() }, // List of badges can be populated later if needed
            { "progress", progress.ToDictionary() } // Convert progress to a dictionary to store
        };

        // Save player data to Firebase using userId (uid) as the key
        var saveTask = dbReference.Child("Players").Child(uid).SetValueAsync(playerData);
        yield return new WaitUntil(() => saveTask.IsCompleted);

        if (saveTask.Exception != null)
        {
            Debug.LogWarning($"Failed to save player data: {saveTask.Exception}");
        }
        else
        {
            Debug.Log("Player data saved successfully.");
        }
    }
}

// Progress class to handle progress data dynamically
[System.Serializable]
public class Progress
{
    public Dictionary<string, object> gameProgress;

    public Progress()
    {
        gameProgress = new Dictionary<string, object>
        {
            { "peppermintPuzzle", new Dictionary<string, object> { { "status", "in_progress" }, { "score", 0 } } },
            { "chocolateCounting", new Dictionary<string, object> { { "status", "in_progress" }, { "score", 0 } } }
        };
    }

    // Convert the progress data to a dictionary
    public Dictionary<string, object> ToDictionary()
    {
        return gameProgress;
    }
}
