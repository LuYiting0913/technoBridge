using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows;
using Firebase;
using Firebase.Auth;
using Firebase.Database;
using TMPro;

public class AuthManager : MonoBehaviour
{

    // all user data
    private string currentUser;
    
    //Firebase variables
    [Header("Firebase")]
    public DependencyStatus dependencyStatus;
    public FirebaseAuth auth;    
    public FirebaseUser User;
    public FirebaseDatabase db;
    public DatabaseReference dbReference;
    // public Levels levels;


    //Login variables
    [Header("Login")]
    public TMP_InputField emailLoginField;
    public TMP_InputField passwordLoginField;
    public TMP_Text warningLoginText;
    public TMP_Text confirmLoginText;

    //Register variables
    [Header("Register")]
    public TMP_InputField usernameRegisterField;
    public TMP_InputField emailRegisterField;
    public TMP_InputField passwordRegisterField;
    public TMP_InputField passwordRegisterVerifyField;
    public TMP_Text warningRegisterText;

    void Awake()
    {
        //Check that all of the necessary dependencies for Firebase are present on the system
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
        {
            dependencyStatus = task.Result;
            if (dependencyStatus == DependencyStatus.Available)
            {
                //If they are avalible Initialize Firebase
                InitializeFirebase(); // This function is written below
            }
            else
            {
                Debug.LogError("Could not resolve all Firebase dependencies: " + dependencyStatus);
            }
        });
    }

    private void InitializeFirebase()
    {
        // Debug.Log("Setting up Firebase Auth");
        //Set the authentication instance object
        auth = FirebaseAuth.DefaultInstance;

        // Debug.Log("Setting up Firebase Storage");
        db = FirebaseDatabase.DefaultInstance;
        // Debug.Log(FirebaseDatabase.DefaultInstance);
        dbReference = FirebaseDatabase.DefaultInstance.RootReference;
        // Debug.Log("Set up Firebase Storage");
        
    }

    //Function for the login button
    public void LoginButton()
    {
        //Call the login coroutine passing the email and password
        StartCoroutine(Login(emailLoginField.text, passwordLoginField.text));
    }
    //Function for the register button
    public void RegisterButton()
    {
        //Call the register coroutine passing the email, password, and username
        StartCoroutine(Register(emailRegisterField.text, passwordRegisterField.text, usernameRegisterField.text));
    }

    // public void UploadLevelsData() {

    // }

    private IEnumerator Login(string _email, string _password)
    {
        //Call the Firebase auth signin function passing the email and password
        var LoginTask = auth.SignInWithEmailAndPasswordAsync(_email, _password);
        //Wait until the task completes
        yield return new WaitUntil(predicate: () => LoginTask.IsCompleted);

        if (LoginTask.Exception != null)
        {
            //If there are errors handle them
            Debug.LogWarning(message: $"Failed to register task with {LoginTask.Exception}");
            FirebaseException firebaseEx = LoginTask.Exception.GetBaseException() as FirebaseException;
            AuthError errorCode = (AuthError)firebaseEx.ErrorCode;

            string message = "Login Failed!";
            switch (errorCode)
            {
                case AuthError.MissingEmail:
                    message = "Missing Email";
                    break;
                case AuthError.MissingPassword:
                    message = "Missing Password";
                    break;
                case AuthError.WrongPassword:
                    message = "Wrong Password";
                    break;
                case AuthError.InvalidEmail:
                    message = "Invalid Email";
                    break;
                case AuthError.UserNotFound:
                    message = "Account does not exist";
                    break;
            }
            warningLoginText.text = message;
        }
        else
        {
            //User is now logged in
            //Now get the result
            
            User = LoginTask.Result;
            Debug.LogFormat("User signed in successfully: {0} ({1})", User.DisplayName, User.Email);
            warningLoginText.text = "";
            confirmLoginText.text = "Logged In";

            currentUser = User.DisplayName;
            
            Levels.currentUserName = currentUser;

            Load();
            // And load main menu
            gameObject.GetComponent<MainMenu>().BackToMain();
        }
    }

    private IEnumerator Register(string _email, string _password, string _username)
    {
        if (_username == "")
        {
            //If the username field is blank show a warning
            warningRegisterText.text = "Missing Username";
        }
        else if(passwordRegisterField.text != passwordRegisterVerifyField.text)
        {
            //If the password does not match show a warning
            warningRegisterText.text = "Password Does Not Match!";
        }
        else 
        {
            //Call the Firebase auth signin function passing the email and password
            var RegisterTask = auth.CreateUserWithEmailAndPasswordAsync(_email, _password);
            //Wait until the task completes
            yield return new WaitUntil(predicate: () => RegisterTask.IsCompleted);

            if (RegisterTask.Exception != null)
            {
                //If there are errors handle them
                Debug.LogWarning(message: $"Failed to register task with {RegisterTask.Exception}");
                FirebaseException firebaseEx = RegisterTask.Exception.GetBaseException() as FirebaseException;
                AuthError errorCode = (AuthError)firebaseEx.ErrorCode;

                string message = "Register Failed!";
                switch (errorCode)
                {
                    case AuthError.MissingEmail:
                        message = "Missing Email";
                        break;
                    case AuthError.MissingPassword:
                        message = "Missing Password";
                        break;
                    case AuthError.WeakPassword:
                        message = "Weak Password";
                        break;
                    case AuthError.EmailAlreadyInUse:
                        message = "Email Already In Use";
                        break;
                }
                warningRegisterText.text = message;
            }
            else
            {
                //User has now been created
                //Now get the result
                User = RegisterTask.Result;

                if (User != null)
                {
                    //Create a user profile and set the username
                    UserProfile profile = new UserProfile{DisplayName = _username};

                    //Call the Firebase auth update user profile function passing the profile with the username
                    var ProfileTask = User.UpdateUserProfileAsync(profile);
                    //Wait until the task completes
                    yield return new WaitUntil(predicate: () => ProfileTask.IsCompleted);

                    if (ProfileTask.Exception != null)
                    {
                        //If there are errors handle them
                        Debug.LogWarning(message: $"Failed to register task with {ProfileTask.Exception}");
                        FirebaseException firebaseEx = ProfileTask.Exception.GetBaseException() as FirebaseException;
                        AuthError errorCode = (AuthError)firebaseEx.ErrorCode;
                        warningRegisterText.text = "Username Set Failed!";
                    }
                    else
                    {
                        //Username is now set
                        //Now return to login screen
                        // UIManager.instance.LoginScreen(); -- No UIManager
                        Debug.Log("Register successful");
                        warningRegisterText.text = "";
                    }
                }
            }
        }
    }

    public void Upload() {
        // Debug.Log(dbReference);
        // Debug.Log(Levels.currentUserName);
        DatabaseReference usersRef = dbReference.Child("Users").Child(Levels.currentUserName);
        DatabaseReference levelsRef = dbReference.Child("Levels");

        Dictionary<string, int> allScores = Levels.GetAllBestScores();
        Dictionary<string, int> updates = new Dictionary<string, int>();
        foreach (string i in allScores.Keys) {
            usersRef.Child(i).SetValueAsync(allScores[i]).ContinueWith((task) => {
                if (task.IsFaulted || task.IsCanceled) {
                    Debug.Log(task.Exception.ToString());
                } else {
                    Debug.Log("upload successfully ");
                }
            });

            levelsRef.Child(i).Child(Levels.currentUserName).SetValueAsync(allScores[i]).ContinueWith((task) => {
                if (task.IsFaulted || task.IsCanceled) {
                    Debug.Log(task.Exception.ToString());
                } else {
                    Debug.Log("upload successfully ");
                }
            });
        }

       
        // Debug.Log(Levels.GetNumOfLevelCompleted());

        dbReference.Child("LevelsCompleted").Child(Levels.currentUserName).SetValueAsync(GlobalData.GetNumOfLevelCompleted()).ContinueWith((task) => {
                if (task.IsFaulted || task.IsCanceled) {
                    Debug.Log(task.Exception.ToString());
                } else {
                    Debug.Log("upload level completed successfully ");
                }
        });
    }

    public void Load() {

        DatabaseReference usersRef = dbReference.Child("Users").Child(Levels.currentUserName);
        usersRef.GetValueAsync().ContinueWith(task => {
            if (task.IsFaulted) {
            // Handle the error...
            } else if (task.IsCompleted) {
                DataSnapshot snapshot = task.Result;
                Dictionary<string, int> allScores = new Dictionary<string, int>();
                for (int i = 1; i < 25; i++) {
                    if (snapshot.Child(i.ToString()) != null) {
                        int j = int.Parse(snapshot.Child(i.ToString()).GetValue(true).ToString());
                        GlobalData.AddLocalData(i.ToString(), j);
                    }
                    
                }
            }
        });

        dbReference.Child("LevelsCompleted").GetValueAsync().ContinueWith(task => {
            if (task.IsFaulted) {
            // Handle the error...
            } else if (task.IsCompleted) {
                DataSnapshot snapshot = task.Result;
                foreach(var player in snapshot.Children){
                    int j = int.Parse(snapshot.Child(player.Key.ToString()).GetValue(true).ToString());
                    GlobalData.AddLevelCompleted(player.Key.ToString(), j);
                }
            }
        });

        // load global data;
        DatabaseReference levelsRef = dbReference.Child("Levels");
        levelsRef.GetValueAsync().ContinueWith(task => {
            if (task.IsFaulted) {
            // Handle the error...
            } else if (task.IsCompleted) {
                DataSnapshot snapshot = task.Result;
                var dictionary1 = snapshot.Value as Dictionary<string, object>;
                Dictionary<string, List<int>> allScores = new Dictionary<string, List<int>>();

                if (dictionary1 != null) {
                    foreach (string level in dictionary1.Keys) {
                        List<int> currentLevel = new List<int>();
                        var dictionary2 = dictionary1[level] as Dictionary<string, object>;
                        foreach (string player in dictionary2.Keys) {
                            currentLevel.Add((int)dictionary2[player]);
                        }
                        allScores[level] = currentLevel;
                    }
                    
                }

                GlobalData.StoreAllGlobalScores(allScores);
                Debug.Log("loaded global data");
            }
        });

    }

    // private IEnumerator Load(string url) {

    // }


}
