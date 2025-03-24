using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Networking;
using System.Collections;
using System.Text.RegularExpressions;

public class LoginManager : MonoBehaviour
{
    public TMP_InputField usernameInput;
    public TMP_InputField passwordInput;
    public Button loginButton;
    public Button registerButton;
    public TMP_Text messageText; // Đảm bảo đã gán TextMeshProUGUI trong Inspector

    [System.Serializable]
    public class User
    {
        public string Username;
        public string Password;
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
        public string message { get; set; } // Sửa thành chữ 'm' thường để khớp với JSON từ API
    }

    void Start()
    {
        if (messageText == null)
        {
            Debug.LogError("MessageText is not assigned in the Inspector!");
        }

        loginButton.onClick.AddListener(OnLoginButtonClicked);
        registerButton.onClick.AddListener(OnRegisterButtonClicked);
    }

    void OnLoginButtonClicked()
    {
        string errorMessage = ValidateInput(false); // false: không kiểm tra ký tự (cho Login)
        if (errorMessage != null)
        {
            DisplayMessage(errorMessage, Color.red);
            return;
        }
        StartCoroutine(LoginCoroutine());
    }

    void OnRegisterButtonClicked()
    {
        string errorMessage = ValidateInput(true); // true: kiểm tra ký tự (cho Register)
        if (errorMessage != null)
        {
            DisplayMessage(errorMessage, Color.red);
            return;
        }
        StartCoroutine(RegisterCoroutine());
    }

    string ValidateInput(bool checkCharacters)
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

        if (checkCharacters)
        {
            if (!Regex.IsMatch(usernameInput.text, @"^[a-zA-Z0-9]+$"))
                return "Username chỉ được chứa chữ cái và số";
            if (passwordInput.text.Contains(" "))
                return "Password không được chứa khoảng trắng";
        }

        return null; // Không có lỗi
    }

    void DisplayMessage(string message, Color color)
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

    IEnumerator RegisterCoroutine()
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
        using (UnityWebRequest www = UnityWebRequest.Post("https://localhost:7032/api/auth/register", json, "application/json"))
        {
            www.certificateHandler = new BypassCertificate();
            yield return www.SendWebRequest();
            if (www.result == UnityWebRequest.Result.Success)
            {
                UserResponse response = JsonUtility.FromJson<UserResponse>(www.downloadHandler.text);
                DisplayMessage("Đăng ký thành công!", Color.green); // Thêm thông báo đăng ký thành công
            }
            else
            {
                Debug.Log("Error response: " + www.downloadHandler.text);
                ErrorResponse error = JsonUtility.FromJson<ErrorResponse>(www.downloadHandler.text);
                string errorMessage = error?.message ?? "Lỗi: UserName Đã Tồn Tại!  " + www.error; // Sửa thành 'message'
                DisplayMessage(errorMessage, Color.red);
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
                PlayerPrefs.SetInt("UserId", response.UserId);
                PlayerPrefs.SetInt("Rice", response.Rice);
                PlayerPrefs.Save();
                DisplayMessage("Đăng nhập thành công!", Color.green); // Thêm thông báo đăng nhập thành công
                yield return new WaitForSeconds(2f);
                UnityEngine.SceneManagement.SceneManager.LoadScene("GameScene");
            }
            else
            {
                Debug.Log("Error response: " + www.downloadHandler.text);
                ErrorResponse error = JsonUtility.FromJson<ErrorResponse>(www.downloadHandler.text);
                string errorMessage = error?.message ?? "Lỗi: Tài khoản Hoặc Mật Khẩu Bị Sai!  " + www.error; // Sửa thành 'message'
                if (errorMessage == "Tài khoản không tồn tại")
                {
                    errorMessage = "Tài khoản chưa được đăng ký. Vui lòng nhấn Register để tạo tài khoản.";
                }
                DisplayMessage(errorMessage, Color.red);
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