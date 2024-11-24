using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Obstacle : MonoBehaviour
{
    public TMP_Text gameOverText;
    public GameObject obstaclePrefab;
    public float minX = -10f, maxX = 10f;
    public float minY = -1f, maxY = 8f;
    public float spawnInterval = 4f;
    private float timer;
    public int scoreToDoubleObstacles = 20;
    private int currentScore;
    public Image obstaclePreview;
    public float fadeInTime = 2f;
    public float disappearTime = 2f;
    private bool isSpawning = false;

    public AudioClip collisionSound; // סאונד לפגיעה
    private AudioSource audioSource; // מקור קול

    private Camera mainCamera; // מצלמה עיקרית
    private GameManager gameManager; // רפרנס למחלקת GameManager

    public float platformY = 0f; // גובה הפלטפורמה

    void Start()
    {
        currentScore = 0; // התחלת הניקוד ב-0
        timer = spawnInterval;
        obstaclePreview.enabled = false;

        audioSource = gameObject.AddComponent<AudioSource>();
        mainCamera = Camera.main;
        UpdateCameraBounds();
        gameManager = FindFirstObjectByType<GameManager>();
    }
    
    void UpdateCameraBounds()
    {
        float cameraHeight = 2f * mainCamera.orthographicSize;
        float cameraWidth = cameraHeight * mainCamera.aspect;

        minX = -cameraWidth / 2f + 1f; // תוספת שוליים קטנה
        maxX = cameraWidth / 2f - 1f;
        minY = platformY; // לא מתחת לפלטפורמה
        maxY = mainCamera.orthographicSize - 1f; // לא מעל המסך
    }

    void Update()
    {
        timer -= Time.deltaTime;

        if (timer <= 0f)
        {
            if (currentScore >= scoreToDoubleObstacles)
            {
                StartCoroutine(SpawnMultipleObstacles());  // אם הניקוד גבוה מ-20, מופיעים שני מכשולים
            }
            else
            {
                SpawnObstacle();  // אחרת, מכשול אחד
            }
            timer = spawnInterval;
        }
    }

    public void SpawnObstacle()
    {
        float randomX = Random.Range(minX, maxX);
        float randomY = Random.Range(minY, maxY);
        Vector3 spawnPosition = new Vector3(randomX, randomY, 0f);

        if (!isSpawning)
        {
            isSpawning = true;
            StartCoroutine(ShowPreview(spawnPosition));
        }

        GameObject obstacle = Instantiate(obstaclePrefab, spawnPosition, Quaternion.identity);
        obstacle.transform.localScale = Vector3.zero;
        StartCoroutine(ScaleUp(obstacle));
        StartCoroutine(AnimateAndDestroyObstacle(obstacle));
    }

    IEnumerator AnimateAndDestroyObstacle(GameObject obstacle)
    {
        yield return new WaitForSeconds(1.5f);

        // אפקט הקטנה
        Vector3 initialScale = obstacle.transform.localScale;
        Vector3 targetScale = Vector3.zero;
        float time = 0f;

        while (time < disappearTime)
        {
            obstacle.transform.localScale = Vector3.Lerp(initialScale, targetScale, time / disappearTime);
            time += Time.deltaTime;
            yield return null;
        }

        Destroy(obstacle);
    }

    IEnumerator SpawnMultipleObstacles()
    {
        SpawnObstacle(); // מכשול ראשון
        yield return new WaitForSeconds(2f); // זמן עיכוב בין מכשול למכשול
        SpawnObstacle(); // מכשול שני
    }

    IEnumerator ScaleUp(GameObject obstacle)
    {
        float time = 0f;
        Vector3 initialScale = obstacle.transform.localScale;
        Vector3 targetScale = new Vector3(1f, 1f, 1f);

        while (time < disappearTime)
        {
            obstacle.transform.localScale = Vector3.Lerp(initialScale, targetScale, time / disappearTime);
            time += Time.deltaTime;
            yield return null;
        }

        obstacle.transform.localScale = targetScale;
    }

    IEnumerator ShowPreview(Vector3 spawnPosition)
    {
        obstaclePreview.enabled = true;
        obstaclePreview.transform.position = spawnPosition;
        obstaclePreview.CrossFadeAlpha(1f, 0f, false);
        obstaclePreview.CrossFadeAlpha(0f, 3f, false);
        yield return new WaitForSeconds(3f);
        obstaclePreview.enabled = false;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Rigidbody2D ballRb = collision.gameObject.GetComponent<Rigidbody2D>();
            if (ballRb != null)
            {
                Vector2 force = new Vector2(0f, 300f);
                ballRb.AddForce(force);
            }

            // ניגון סאונד פגיעה
            if (collisionSound != null && audioSource != null)
            {
                audioSource.PlayOneShot(collisionSound);
            }

            // קריאה לפונקציה להקטנת גודל הכדור בעת פגיעה במכשול
            if (gameManager != null)
            {
                gameManager.DecreaseBallSize(0.1f); // עדכון הגודל של הכדור על פי הצורך
            }
        }
    }
    public void UpdateScore(int scoreIncrease)
    {
        currentScore += scoreIncrease;
        if (currentScore >= scoreToDoubleObstacles)
        {
            spawnInterval = 1.5f; // מקצר את זמן הרווח בין המכשולים
        }
    }

    // פונקציה לניקוי המכשולים ואיפוס הערכים
    public void ResetObstacles()
    {
        // מחיקת כל המכשולים הקיימים
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject); // השמדת כל המכשולים
        }
        obstaclePreview.enabled = false;
        // איפוס המשתנים
        currentScore = 0; // איפוס הניקוד
        spawnInterval = 5f; // זמן הופעה מחדש לברירת מחדל
        isSpawning = false; // איפוס מצב יצירת מכשולים
    }

    public void UpdateSpawnInterval(float newInterval)
    {
        spawnInterval = newInterval; // עדכון המרווח בין הופעות המכשולים
    }
}