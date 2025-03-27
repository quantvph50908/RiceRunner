using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Networking;
using System.Collections;
using System.Text.RegularExpressions;
using SecPlayerPrefs;

public class LoginManager : MonoBehaviour
{
    // Thành phần giao diện đăng nhập
    public TMP_InputField usernameInput;
    public TMP_InputField passwordInput;
    public Button loginButton;
    public Button registerButton;
    public Button forgotPasswordButton;
    public TMP_Text messageText;

    // Thành phần giao diện đăng ký
    public TMP_InputField registerUsernameInput;
    public TMP_InputField registerPasswordInput;
    public TMP_InputField confirmPasswordInput;
    public TMP_InputField registerEmailInput;
    public Button submitRegisterButton;
    public Button backButton;
    public TMP_Text registerMessageText;

    // Thành phần giao diện quên mật khẩu
    public TMP_InputField forgotPasswordEmailInput;
    public Button submitEmailButton;
    public Button forgotPasswordBackButton;
    public TMP_Text forgotPasswordMessageText;
    public GameObject loginPanel;
    public GameObject registerPanel;
    public GameObject forgotPasswordPanel;

    [System.Serializable]
    public class User
    {
        public string Username;
        public string Password;
        public string Email;
        public int Score;
        public int Rice;
    }

    [System.Serializable]
    public class UserResponse
    {
        public int UserId;
        public int Score;
        public int Rice;
        public string Message;
    }

    [System.Serializable]
    public class ErrorResponse
    {
        public string message;
    }

    [System.Serializable]
    public class ForgotPasswordRequest
    {
        public string Email;
    }

    [System.Serializable]
    public class ForgotPasswordResponse
    {
        public string Message;
    }

    void Start()
    {
        if (messageText == null || registerMessageText == null || forgotPasswordMessageText == null)
        {
            Debug.LogError("MessageText, RegisterMessageText, or ForgotPasswordMessageText is not assigned in the Inspector!");
        }

        loginButton.onClick.AddListener(OnLoginButtonClicked);
        registerButton.onClick.AddListener(OnRegisterButtonClicked);
        submitRegisterButton.onClick.AddListener(OnSubmitRegisterButtonClicked);
        backButton.onClick.AddListener(OnBackButtonClicked);
        forgotPasswordButton.onClick.AddListener(OnForgotPasswordButtonClicked);
        submitEmailButton.onClick.AddListener(OnSubmitEmailButtonClicked);
        forgotPasswordBackButton.onClick.AddListener(OnForgotPasswordBackButtonClicked);

        loginPanel.SetActive(true);
        registerPanel.SetActive(false);
        forgotPasswordPanel.SetActive(false);
    }

    void OnLoginButtonClicked()
    {
        string errorMessage = ValidateLoginInput();
        if (errorMessage != null)
        {
            DisplayMessage(errorMessage, Color.red, true);
            return;
        }
        StartCoroutine(LoginCoroutine());
    }

    void OnRegisterButtonClicked()
    {
        loginPanel.SetActive(false);
        registerPanel.SetActive(true);
        messageText.text = "";
        registerMessageText.text = "";
    }

    void OnSubmitRegisterButtonClicked()
    {
        string errorMessage = ValidateRegisterInput();
        if (errorMessage != null)
        {
            DisplayMessage(errorMessage, Color.red, false);
            return;
        }
        StartCoroutine(RegisterCoroutine());
    }

    void OnBackButtonClicked()
    {
        registerPanel.SetActive(false);
        loginPanel.SetActive(true);
        messageText.text = "";
        registerMessageText.text = "";
        registerUsernameInput.text = "";
        registerPasswordInput.text = "";
        confirmPasswordInput.text = "";
        registerEmailInput.text = "";
    }

    void OnForgotPasswordButtonClicked()
    {
        loginPanel.SetActive(false);
        forgotPasswordPanel.SetActive(true);
        messageText.text = "";
        forgotPasswordMessageText.text = "";
    }

    void OnSubmitEmailButtonClicked()
    {
        string errorMessage = ValidateForgotPasswordInput();
        if (errorMessage != null)
        {
            DisplayMessage(errorMessage, Color.red, false, true);
            return;
        }
        StartCoroutine(ForgotPasswordCoroutine());
    }

    void OnForgotPasswordBackButtonClicked()
    {
        forgotPasswordPanel.SetActive(false);
        loginPanel.SetActive(true);
        messageText.text = "";
        forgotPasswordMessageText.text = "";
        forgotPasswordEmailInput.text = "";
    }

    string ValidateLoginInput()
    {
        bool isUsernameEmpty = string.IsNullOrEmpty(usernameInput.text);
        bool isPasswordEmpty = string.IsNullOrEmpty(passwordInput.text);

        if (isUsernameEmpty && isPasswordEmpty)
            return "Vui lòng nhập Username và Password";
        if (isUsernameEmpty)
            return "Vui lòng nhập Username";
        if (isPasswordEmpty)
            return "Vui lòng nhập Password";

        if (usernameInput.text.Length < 3)
            return "Username phải có ít nhất 3 ký tự";
        if (passwordInput.text.Length < 6)
            return "Password phải có ít nhất 6 ký tự";

        if (!Regex.IsMatch(usernameInput.text, @"^[a-zA-Z0-9]+$"))
            return "Username chỉ được chứa chữ cái và số";
        if (passwordInput.text.Contains(" "))
            return "Password không được chứa khoảng trắng";

        return null;
    }

    string ValidateRegisterInput()
    {
        bool isUsernameEmpty = string.IsNullOrEmpty(registerUsernameInput.text);
        bool isPasswordEmpty = string.IsNullOrEmpty(registerPasswordInput.text);
        bool isConfirmPasswordEmpty = string.IsNullOrEmpty(confirmPasswordInput.text);
        bool isEmailEmpty = string.IsNullOrEmpty(registerEmailInput.text);

        if (isUsernameEmpty || isPasswordEmpty || isConfirmPasswordEmpty || isEmailEmpty)
            return "Vui lòng nhập đầy đủ thông tin";

        if (registerUsernameInput.text.Length < 3)
            return "Username phải có ít nhất 3 ký tự";
        if (registerPasswordInput.text.Length < 6)
            return "Password phải có ít nhất 6 ký tự";

        if (!Regex.IsMatch(registerUsernameInput.text, @"^[a-zA-Z0-9]+$"))
            return "Username chỉ được chứa chữ cái và số";
        if (registerPasswordInput.text.Contains(" "))
            return "Password không được chứa khoảng trắng";

        if (registerPasswordInput.text != confirmPasswordInput.text)
            return "Mật khẩu nhập lại không khớp";

        if (!Regex.IsMatch(registerEmailInput.text, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            return "Email không hợp lệ";

        return null;
    }

    string ValidateForgotPasswordInput()
    {
        if (string.IsNullOrEmpty(forgotPasswordEmailInput.text))
            return "Vui lòng nhập Email";

        if (!Regex.IsMatch(forgotPasswordEmailInput.text, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            return "Email không hợp lệ";

        return null;
    }

    void DisplayMessage(string message, Color color, bool isLoginPanel, bool isForgotPasswordPanel = false)
    {
        if (isLoginPanel && !isForgotPasswordPanel)
        {
            if (messageText != null)
            {
                messageText.text = message;
                messageText.color = color;
            }
            else
            {
                Debug.LogError("Cannot display message: MessageText is not assigned!");
            }
        }
        else if (!isLoginPanel && !isForgotPasswordPanel)
        {
            if (registerMessageText != null)
            {
                registerMessageText.text = message;
                registerMessageText.color = color;
            }
            else
            {
                Debug.LogError("Cannot display message: RegisterMessageText is not assigned!");
            }
        }
        else if (isForgotPasswordPanel)
        {
            if (forgotPasswordMessageText != null)
            {
                forgotPasswordMessageText.text = message;
                forgotPasswordMessageText.color = color;
            }
            else
            {
                Debug.LogError("Cannot display message: ForgotPasswordMessageText is not assigned!");
            }
        }
    }

    IEnumerator RegisterCoroutine()
    {
        User user = new User
        {
            Username = registerUsernameInput.text,
            Password = registerPasswordInput.text,
            Email = registerEmailInput.text,
            Score = 0,
            Rice = 0
        };
        string json = JsonUtility.ToJson(user);
        Debug.Log("Sending JSON: " + json);
        using (UnityWebRequest www = UnityWebRequest.Post("https://localhost:7032/api/auth/register", json, "application/json"))
        {
            www.certificateHandler = new BypassCertificate();
            yield return www.SendWebRequest();
            if (www.result == UnityWebRequest.Result.Success)
            {
                UserResponse response = JsonUtility.FromJson<UserResponse>(www.downloadHandler.text);
                DisplayMessage("Đăng ký thành công!", Color.green, false);
                yield return new WaitForSeconds(2f);
                registerPanel.SetActive(false);
                loginPanel.SetActive(true);
                registerUsernameInput.text = "";
                registerPasswordInput.text = "";
                confirmPasswordInput.text = "";
                registerEmailInput.text = "";
                registerMessageText.text = "";
            }
            else
            {
                Debug.Log("Error response: " + www.downloadHandler.text);
                ErrorResponse error = JsonUtility.FromJson<ErrorResponse>(www.downloadHandler.text);
                string errorMessage = error?.message ?? "Lỗi: UserName Đã Tồn Tại!";
                DisplayMessage(errorMessage, Color.red, false);
            }
        }
    }

    IEnumerator LoginCoroutine()
    {
        User user = new User
        {
            Username = usernameInput.text,
            Password = passwordInput.text,
            Score = 0,
            Rice = 0
        };
        string json = JsonUtility.ToJson(user);
        Debug.Log("Sending JSON: " + json);
        using (UnityWebRequest www = UnityWebRequest.Post("https://localhost:7032/api/auth/login", json, "application/json"))
        {
            www.certificateHandler = new BypassCertificate();
            yield return www.SendWebRequest();
            if (www.result == UnityWebRequest.Result.Success)
            {
                UserResponse response = JsonUtility.FromJson<UserResponse>(www.downloadHandler.text);
                SecurePlayerPrefs.SetInt("UserId", response.UserId);
                SecurePlayerPrefs.SetInt("Rice", response.Rice);
                SecurePlayerPrefs.Save();
                DisplayMessage("Đăng nhập thành công!", Color.green, true);
                yield return new WaitForSeconds(2f);
                UnityEngine.SceneManagement.SceneManager.LoadScene("GameScene");
            }
            else
            {
                Debug.Log("Error response: " + www.downloadHandler.text);
                ErrorResponse error = JsonUtility.FromJson<ErrorResponse>(www.downloadHandler.text);
                string errorMessage = error?.message ?? "Lỗi: Tài khoản Hoặc Mật Khẩu Bị Sai!";
                if (errorMessage == "Tài khoản không tồn tại")
                {
                    errorMessage = "Tài khoản chưa được đăng ký. Vui lòng nhấn Register để tạo tài khoản.";
                }
                DisplayMessage(errorMessage, Color.red, true);
            }
        }
    }

    IEnumerator ForgotPasswordCoroutine()
    {
        ForgotPasswordRequest request = new ForgotPasswordRequest
        {
            Email = forgotPasswordEmailInput.text
        };
        string json = JsonUtility.ToJson(request);
        Debug.Log("Sending JSON: " + json);
        using (UnityWebRequest www = UnityWebRequest.Post("https://localhost:7032/api/auth/forgot-password", json, "application/json"))
        {
            www.certificateHandler = new BypassCertificate();
            yield return www.SendWebRequest();
            if (www.result == UnityWebRequest.Result.Success)
            {
                ForgotPasswordResponse response = JsonUtility.FromJson<ForgotPasswordResponse>(www.downloadHandler.text);
                DisplayMessage(response.Message, Color.green, false, true);
                yield return new WaitForSeconds(5f);
                forgotPasswordPanel.SetActive(false);
                loginPanel.SetActive(true);
                forgotPasswordEmailInput.text = "";
                forgotPasswordMessageText.text = "";
            }
            else
            {
                Debug.Log("Error response: " + www.downloadHandler.text);
                ErrorResponse error = JsonUtility.FromJson<ErrorResponse>(www.downloadHandler.text);
                string errorMessage = error?.message ?? "Lỗi: " + www.error;
                DisplayMessage(errorMessage, Color.red, false, true);
            }
        }
    }
}

public class BypassCertificate : CertificateHandler
{
    protected override bool ValidateCertificate(byte[] certificateData)
    {
        return true;
    }
}