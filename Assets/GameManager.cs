using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public Rigidbody2D ballRigidbody;
    public TMP_Text scoreText; // טקסט להצגת הניקוד
    public TMP_Text highScoreText; // טקסט להצגת השיא
    public int score = 0; // הניקוד
    private int highScore = 0; // השיא
    public Obstacle obstacleManager; // ניהול המכשולים
    public float minBallSize = 0.5f;
    public float maxBallSize = 1f;
    private float ballSizeIncreaseFactor; // גורם הגדלת הכדור
    public float growthAnimationTime = 0.2f;
    public float shrinkAnimationTime = 0.2f;
    private Vector2 initialPosition = new Vector2(-0.08f, -1.64f);

    public TMP_Text gameOverText; // שדה עבור הודעת "Game Over"
    public bool restrtClicked = false;
    public GameObject gameOverPanel; // הפאנל שמכיל את כפתור ההתחלה
    public Button restartButton;     // כפתור "התחל מחדש"
    public StartScreenManager startScreenManager; // מנהל מסך התחלה
    public GameObject platform; // התייחסות לפלטפורמה
    public GameObject coin; // התייחסות לפלטפורמה
    public GameObject ball; // התייחסות לכדור
    public AudioSource musicSource; // מקור המוזיקה
    private bool isMuted = false;  // משתנה לבדוק אם המוזיקה מושתקת
    public Sprite soundOnIcon; // אייקון הצליל הפעיל
    public Sprite soundOffIcon; // אייקון הצליל המושתק
    public Image muteButtonImage; // תמונה של כפתור ההשתקה (כדי לשנות את האייקון)
    public RectTransform muteButtonRectTransform; // RectTransform של כפתור ההשתקה
    public StartScreenManager StartScreenManager;
    void Start()
    {
        restartButton.gameObject.SetActive(false);
        restartButton.enabled = false; // בהתחלה לא מציגים את הפאנל
        // טעינת השיא מהשמירה
        highScore = PlayerPrefs.GetInt("HighScore", 0);
        UpdateHighScoreText();

        UpdateScoreText();
        ballSizeIncreaseFactor = (maxBallSize - minBallSize) / 20f;

        // הפעלת מסך ההתחלה
        startScreenManager.ShowStartScreen();

        // התחלת המשחק בלחיצה על המסך
        if (startScreenManager != null)
        {
            startScreenManager.OnStartScreenClicked.AddListener(StartGame); // חיבור לאירוע
        }

        // הפעלת המוזיקה במידת הצורך
        if (musicSource != null && !isMuted)
        {
            musicSource.Play();
        }

        UpdateMuteButtonIcon();
        PositionMuteButton();
    }

    // קריאה לפונקציה IncreaseSpeed מתוך GameManager
    public void IncreaseObstacleSpeed()
    {
        if (score % 10 == 0) // כל 10 נקודות
        {
            ObstacleMovement obstacleMovement = obstacleManager.GetComponent<ObstacleMovement>();
            if (obstacleMovement != null)
            {
                obstacleMovement.IncreaseSpeed(0.2f); // הגדלת מהירות המכשול ב-0.2
            }
        }
    }



    private void PositionMuteButton()
    {
        if (muteButtonRectTransform != null)
        {
            muteButtonRectTransform.anchorMin = new Vector2(0, 1);
            muteButtonRectTransform.anchorMax = new Vector2(0, 1);
            muteButtonRectTransform.pivot = new Vector2(0, 1);
            muteButtonRectTransform.anchoredPosition = new Vector2(100, -100);
        }
    }
    public void ResetBallPhysics()
    {
        if (ballRigidbody != null)
        {
            // איפוס מהירות הכדור
            ballRigidbody.linearVelocity = Vector2.zero;

            // אופציונלי: איפוס מהירות סיבובית
            ballRigidbody.angularVelocity = 0f;
        }
        else
        {
            Debug.LogError("ballRigidbody is not assigned in the GameManager!");
        }
    }
    public void StartGame()
    {
        Debug.Log("Game started!");

        score = 0;
        UpdateScoreText();
        ResetBallSize(); // איפוס גודל הכדור
        startScreenManager.StartGame(); // קריאה למסך ההתחלה להתחלת המשחק
        obstacleManager.ResetObstacles(); // איפוס מכשולים
    }

    public void AddScore(int amount)
    {
        score += amount;
        UpdateScoreText();

        // עדכון השיא במידת הצורך
        if (score > highScore)
        {
            highScore = score;
            PlayerPrefs.SetInt("HighScore", highScore); // שמירת השיא
            UpdateHighScoreText();
        }

        IncreaseBallSize();

        // הגדלת מהירות המכשול לאחר כל 10 נקודות
        IncreaseObstacleSpeed();
    }
    private void UpdateScoreText()
    {
        scoreText.text = score.ToString();
    }

    private void UpdateHighScoreText()
    {
        highScoreText.text = "High Score " + highScore.ToString();
    }

    private void IncreaseBallSize()
    {
        GameObject ball = GameObject.FindWithTag("Player");
        if (ball != null)
        {
            StartCoroutine(GrowBall(ball));
        }
    }

    public void DecreaseBallSize(float decreaseAmount)
    {
        GameObject ball = GameObject.FindWithTag("Player");
        if (ball != null)
        {
            float newSize = ball.transform.localScale.x - decreaseAmount;

            if (newSize < minBallSize)
            {
                newSize = minBallSize;
            }

            StartCoroutine(ShrinkBall(ball, newSize));
        }
    }

    private IEnumerator GrowBall(GameObject ball)
    {
        float elapsedTime = 0f;
        float originalSize = ball.transform.localScale.x;
        float targetSize = Mathf.Min(originalSize + ballSizeIncreaseFactor, maxBallSize);

        while (elapsedTime < growthAnimationTime)
        {
            float newSize = Mathf.Lerp(originalSize, targetSize, elapsedTime / growthAnimationTime);
            ball.transform.localScale = new Vector3(newSize, newSize, 1f);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        ball.transform.localScale = new Vector3(targetSize, targetSize, 1f);
    }

    private IEnumerator ShrinkBall(GameObject ball, float targetSize)
    {
        float elapsedTime = 0f;
        float originalSize = ball.transform.localScale.x;

        while (elapsedTime < shrinkAnimationTime)
        {
            float newSize = Mathf.Lerp(originalSize, targetSize, elapsedTime / shrinkAnimationTime);
            ball.transform.localScale = new Vector3(newSize, newSize, 1f);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        ball.transform.localScale = new Vector3(targetSize, targetSize, 1f);
    }
    private void ResetPlatformPosition()
    {
        if (platform != null)
        {
            platform.transform.position = new Vector3(0, -2.745f, 0);
            Debug.Log("Platform position reset to (0, -2.745, 0)");
        }
        else
        {
            Debug.LogWarning("Platform GameObject is not assigned!");
        }
    }
    private void ResetBallSize()
    {
        GameObject ball = GameObject.FindWithTag("Player");
        if (ball != null)
        {
            ball.transform.localScale = new Vector3(minBallSize, minBallSize, 1f);
        }
    }
    public void ResetBallPosition()
    {
        if (ball != null)
        {
            // איפוס מיקום
            ball.transform.position = new Vector3(0, 3.036f, 0);

            // איפוס מהירות וזווית
            Rigidbody ballRigidbody = ball.GetComponent<Rigidbody>();
            if (ballRigidbody != null)
            {
                ballRigidbody.linearVelocity = Vector3.zero; // איפוס מהירות
                ballRigidbody.angularVelocity = Vector3.zero; // איפוס מהירות סיבובית
            }

            // איפוס זווית
            ball.transform.rotation = Quaternion.identity; // זווית התחלתית
        }
        else
        {
            Debug.LogWarning("Ball GameObject is not assigned!");
        }
    }


    public void EndGame()
    {
        Debug.Log("gyaaaat");

        // איפוס מצב המשחק
        ResetBallSize();
        ResetBallPosition();
        ResetPlatformPosition();
        ResetCoinPosition(coin);
        obstacleManager.ResetObstacles();
        // הצגת מסך סיום המשחק
        startScreenManager.ShowGameOverScreen(score);

        // אפס את הניקוד
        score = 0;
        UpdateScoreText();
        Time.timeScale = 0;
        //הצגת כפתור הפעלה מחדש
        //לחיצה על כפתור
        //gameOverPanel.SetActive(true);
        restartButton.enabled = true;
        scoreText.enabled = false;
        restartButton.onClick.AddListener(OnRestartButtonClicked); // חיבור ללחיצה על הכפתור
        if (restrtClicked == true)
        {
            restrtClicked = false;
        }
    }
    private void OnRestartButtonClicked()
    {
        restrtClicked = true;
        gameOverText.enabled = false;
        // מחזירים את מצב המשחק להתחלה מחדש
        restartButton.enabled = false;
        ResetBallPhysics();
        startScreenManager.ShowStartScreen();
    }
    public void ResetCoinPosition(GameObject coin)
    {
        if (coin != null)
        {
            coin.transform.position = initialPosition;
            Debug.Log("מיקום המטבע אותחל בהצלחה ל-" + initialPosition);
        }
        else
        {
            Debug.LogError("שגיאה: אובייקט המטבע לא הוגדר.");
        }
    }

    public void ToggleMute()
    {
        isMuted = !isMuted;
        if (musicSource != null)
        {
            musicSource.mute = isMuted;
            if (!isMuted && !musicSource.isPlaying)
            {
                musicSource.Play();
            }
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
}