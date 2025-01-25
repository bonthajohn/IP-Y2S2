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
    public TMP_InputField loginField; // Both username or email input
    public TMP_InputField passwordLoginField; // Password field
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

    // Unified function for login (email or username)
    public void LoginButton()
    {
        StartCoroutine(Login(loginField.text, passwordLoginField.text));
    }

    private IEnumerator Login(string loginInput, string password)
    {
        if (string.IsNullOrEmpty(loginInput))
        {
            warningLoginText.text = "Missing username/email";
            yield break;
        }

        if (IsEmail(loginInput))
        {
            // If it's an email, directly attempt to login with email
            StartCoroutine(LoginWithEmail(loginInput, password));
        }
        else
        {
            // If it's a username, retrieve the associated email from the database and then login
            StartCoroutine(LoginWithUsername(loginInput, password));
        }
    }

    private bool IsEmail(string input)
    {
        return input.Contains("@");
    }

    private IEnumerator LoginWithUsername(string username, string password)
    {
        // Get the user ID from the username
        var getUserIdTask = dbReference.Child("Usernames").Child(username).GetValueAsync();
        yield return new WaitUntil(() => getUserIdTask.IsCompleted);

        if (getUserIdTask.Exception != null || !getUserIdTask.Result.Exists)
        {
            warningLoginText.text = "Username not found!";
            yield break;
        }

        string userId = getUserIdTask.Result.Value.ToString();

        // Get the email associated with the user ID
        var getEmailTask = dbReference.Child("Players").Child(userId).Child("email").GetValueAsync();
        yield return new WaitUntil(() => getEmailTask.IsCompleted);

        if (getEmailTask.Exception != null || !getEmailTask.Result.Exists)
        {
            warningLoginText.text = "Failed to retrieve user email!";
            yield break;
        }

        string email = getEmailTask.Result.Value.ToString();

        // Now login using the retrieved email
        StartCoroutine(LoginWithEmail(email, password));
    }

    private IEnumerator LoginWithEmail(string email, string password)
    {
        Task<AuthResult> loginTask = auth.SignInWithEmailAndPasswordAsync(email, password);
        yield return new WaitUntil(() => loginTask.IsCompleted);

        if (loginTask.Exception != null)
        {
            Debug.LogWarning($"Failed to login task with {loginTask.Exception}");
            FirebaseException firebaseEx = loginTask.Exception.GetBaseException() as FirebaseException;
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
            User = loginTask.Result.User;
            Debug.LogFormat("User signed in successfully: {0} ({1})", User.DisplayName, User.Email);
            warningLoginText.text = "";
            confirmLoginText.text = "Logged In";
        }
    }

    private IEnumerator Register(string _email, string _password, string _username)
    {
        if (string.IsNullOrEmpty(_username))
        {
            warningRegisterText.text = "Missing Username";
            yield break;
        }
        if (_password != passwordRegisterVerifyField.text)
        {
            warningRegisterText.text = "Passwords do not match!";
            yield break;
        }

        Task<AuthResult> registerTask = auth.CreateUserWithEmailAndPasswordAsync(_email, _password);
        yield return new WaitUntil(() => registerTask.IsCompleted);

        if (registerTask.Exception != null)
        {
            Debug.LogWarning($"Failed to register task with {registerTask.Exception}");
            FirebaseException firebaseEx = registerTask.Exception.GetBaseException() as FirebaseException;
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
            User = registerTask.Result.User;

            if (User != null)
            {
                UserProfile profile = new UserProfile { DisplayName = _username };
                Task ProfileTask = User.UpdateUserProfileAsync(profile);
                yield return new WaitUntil(() => ProfileTask.IsCompleted);

                if (ProfileTask.Exception != null)
                {
                    Debug.LogWarning($"Failed to update profile: {ProfileTask.Exception}");
                    warningRegisterText.text = "Username Set Failed!";
                }
                else
                {
                    Debug.Log($"Successfully created user: {User.DisplayName}, UID: {User.UserId}");
                    StartCoroutine(SaveUserData(_username, _email, User.UserId));
                    warningRegisterText.text = "Registration Successful!";
                }
            }
        }
    }

    private IEnumerator SaveUserData(string username, string email, string uid)
    {
        Progress progress = new Progress();

        var playerData = new Dictionary<string, object>
        {
            { "name", username },
            { "Uid", uid },
            { "email", email },
            { "badges", new List<string>() },
            { "progress", progress.ToDictionary() }
        };

        // Save username-to-UID mapping
        var usernameMappingTask = dbReference.Child("Usernames").Child(username).SetValueAsync(uid);
        yield return new WaitUntil(() => usernameMappingTask.IsCompleted);

        if (usernameMappingTask.Exception != null)
        {
            Debug.LogWarning($"Failed to save username mapping: {usernameMappingTask.Exception}");
            yield break;
        }

        // Save user data
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

    public Dictionary<string, object> ToDictionary()
    {
        return gameProgress;
    }
}
