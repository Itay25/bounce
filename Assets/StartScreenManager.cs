using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class StartScreenManager : MonoBehaviour
{
    public GameObject startScreen; // Canvas של מסך ההתחלה
    public TMP_Text highScoreText; // טקסט להצגת השיא
    public TMP_Text finalScoreText; // טקסט להצגת הניקוד האחרון
    public AudioSource musicSource; // מקור המוזיקה במשחק
    public GameObject muteButton;  // כפתור ההשתקה
    public Sprite soundOnIcon; // אייקון למצב "צליל פעיל"
    public Sprite soundOffIcon; // אייקון למצב "צליל מושתק"
    public Image muteButtonImage; // רכיב תמונה עבור כפתור ההשתקה
    public UnityEvent OnStartScreenClicked; // האירוע שיתפרסם כאשר יש לחיצה על מסך ההתחלה
    private bool isMuted = false;  // מצב ההשתקה
    private RectTransform muteButtonRectTransform; // RectTransform של כפתור ההשתקה
    public GameObject gameOverPanel; // הפאנל שמכיל את כפתור ההתחלה
    public Button restartButton;     // כפתור "התחל מחדש"
    public GameManager gameManager;
    public TMP_Text gameOverText; // שדה עבור הודעת "Game Over"

    void Start()
    {
        // עדכון השיא במסך ההתחלה
        UpdateHighScoreDisplay();

        // הצגת מסך ההתחלה
        ShowStartScreen();

        // הסתרת פאנל ה-Game Over וכפתור ההתחלה בהתחלה
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }
        if (restartButton != null)
        {
            restartButton.onClick.AddListener(OnRestartButtonClick);
        }

        // שמירת RectTransform של כפתור ההשתקה
        muteButtonRectTransform = muteButton.GetComponent<RectTransform>();
        PositionMuteButton();
        UpdateMuteButtonIcon();

        // חיבור כפתור ההתחלה לאירוע הלחיצה
        if (restartButton != null)
        {
            restartButton.onClick.AddListener(OnRestartButtonClick);
        }
    }
    public void TurnOffGameOverText()
    {
        Debug.Log("gameover disabled");
        gameOverText.enabled = false;
    }
    public void OnRestartButtonClick()
    {
        Debug.Log("Restarting game...");
        // הפעלת המשחק מחדש
        Time.timeScale = 1;
        OnStartScreenClicked.Invoke(); // האירוע שמפעיל את התחלת המשחק
    }


    void Update()
    {
        // זיהוי לחיצה או מגע על המסך
        if ((Input.GetMouseButtonDown(0) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)) &&
        startScreen.activeSelf && !IsClickInsideMuteButton())
        {
            OnStartScreenClicked.Invoke(); // קריאה לאירוע כאשר נעשתה לחיצה
        }
    }

    public void ShowStartScreen()
    {
        Debug.Log("Returning to start screen...");
        // עדכון השיא במסך ההתחלה
        UpdateHighScoreDisplay();
        gameManager.scoreText.enabled = false;
        // הסתרת טקסטים אחרים (כמו Game Over)
        if (finalScoreText != null)
        {
            finalScoreText.text = ""; // איפוס טקסט של ניקוד אחרון
        }

        // עצירת המשחק
        Time.timeScale = 0;

        // הפעלת מסך ההתחלה
        startScreen.SetActive(true);
        TurnOffGameOverText();
    }

    public void ShowGameOverScreen(int score)
    {
        // הצגת פאנל Game Over
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }

        // הצגת כפתור "שחק שוב"
        if (restartButton != null)
        {
            restartButton.gameObject.SetActive(true);
        }
    }
    public void StartGame()
    {
        Debug.Log("Starting game...");

        Time.timeScale = 1; // הפעלת המשחק
        startScreen.SetActive(false); // הסתרת מסך ההתחלה

        // ודא שכל הפאנלים האחרים מוסתרים
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }
        restartButton.gameObject.SetActive(false); // הסתרת כפתור ההתחלה מחדש
    }


    public void RestartGame()
    {
        Debug.Log("Restarting game...");

        // איפוס מסך הסיום והפעלת המשחק מחדש
        startScreen.SetActive(false);
        Time.timeScale = 1;

        gameManager.StartGame();
    }

    private void UpdateHighScoreIfBeaten(int score)
    {
        int highScore = PlayerPrefs.GetInt("HighScore", 0);

        if (score > highScore)
        {
            PlayerPrefs.SetInt("HighScore", score);
            PlayerPrefs.Save();
            Debug.Log("New high score saved: " + score);
        }
    }

    private void UpdateHighScoreDisplay()
    {
        int highScore = PlayerPrefs.GetInt("HighScore", 0);
        highScoreText.text = "High Score " + highScore;
    }

    private bool IsClickInsideMuteButton()
    {
        Vector2 localPoint = muteButtonRectTransform.InverseTransformPoint(Input.mousePosition);
        return muteButtonRectTransform.rect.Contains(localPoint);
    }

    public void ToggleMute()
    {
        isMuted = !isMuted;
        if (musicSource != null)
        {
            musicSource.mute = isMuted;
        }
        UpdateMuteButtonIcon();
    }

    private void UpdateMuteButtonIcon()
    {
        if (muteButtonImage != null)
        {
            muteButtonImage.sprite = isMuted ? soundOffIcon : soundOnIcon;
        }
    }

    private void PositionMuteButton()
    {
        if (muteButtonRectTransform != null)
        {
            muteButtonRectTransform.anchorMin = new Vector2(1, 1); // מיקום בפינה הימנית העליונה
            muteButtonRectTransform.anchorMax = new Vector2(1, 1);
            muteButtonRectTransform.pivot = new Vector2(1, 1);
            muteButtonRectTransform.anchoredPosition = new Vector2(-20, -20); // מרחק מהקצה
        }
    }
}