using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SignInUI : MonoBehaviour {

    InputField _email;
    InputField _password;
    InputField _displayName;

    Text _displayNameText;
    Text _status;

    Button _btnSignIn;
    Button _btnCreate;


	// Use this for initialization
	void Start ()
    {
        Transform panel = transform.FindChild("Panel");

        _email = panel.FindChild("Email").GetComponent<InputField>();
        _password = panel.FindChild("Password").GetComponent<InputField>();
        _displayName = panel.FindChild("DisplayName").GetComponent<InputField>();

        _displayNameText = panel.FindChild("DisplayNameText").GetComponent<Text>();
        _status = panel.FindChild("Status").GetComponent<Text>();

        _btnSignIn = panel.FindChild("SignInButton").GetComponent<Button>();
        _btnCreate = panel.FindChild("CreateAccountButton").GetComponent<Button>();
        
        HideCreate(true);
        EnableControls(true);
        
        _status.GetComponent<CanvasGroup>().alpha = 0;
        
        _btnSignIn.onClick.AddListener(OnSignIn);
        _btnCreate.onClick.AddListener(OnCreate);
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (Globals.Network != null && Globals.Network.Online != NetworkManager.SignInStatus.SignedOut)
        {
            switch (Globals.Network.Online)
            {
                case NetworkManager.SignInStatus.SigningIn:
                    break;
                case NetworkManager.SignInStatus.SignedIn:
                    break;
                case NetworkManager.SignInStatus.InvalidPassword:
                    Globals.Network.ResetSignIn();
                    _status.text = "Invalid Password";
                    EnableControls(true);
                    HideCreate(true);
                    break;
                case NetworkManager.SignInStatus.AccountDoesntExist:
                    Globals.Network.ResetSignIn();
                    EnableControls(true);
                    HideCreate(false);
                    _status.text = "Account doesn't exist";
                    break;
            }
        }
	}

    void HideCreate(bool hide)
    {
        _displayNameText.GetComponent<CanvasGroup>().alpha = hide ? 0 : 1;
        _displayName.GetComponent<CanvasGroup>().alpha = hide ? 0 : 1;
        _btnCreate.GetComponent<CanvasGroup>().alpha = hide ? 0 : 1;
    }

    void EnableControls(bool enable)
    {
        _email.interactable = enable;
        _password.interactable = enable;
        _displayName.interactable = enable;
        _btnCreate.interactable = enable;
        _btnSignIn.interactable = enable;
    }

    void OnSignIn()
    {
        // Disable controls
        EnableControls(false);

        // Set status
        _status.GetComponent<CanvasGroup>().alpha = 1;
        _status.text = "Signing In";

        // Do sign in
        Globals.Network.SignIn(_email.text, _password.text);
    }

    void OnCreate()
    {
        if (_email.text.Length <= 0)
            _status.text = "Please enter an email address";
        else if (_password.text.Length <= 0)
            _status.text = "Please enter a password";
        else if (_displayName.text.Length <= 0)
            _status.text = "Please enter a display name";
        else
        {
            EnableControls(false);

            // Set Status
            _status.text = "Creating Account";

            Globals.Network.CreateAccount(_email.text, _password.text, _displayName.text);
        }
    }
}
