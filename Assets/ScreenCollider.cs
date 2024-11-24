using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SocialPlatforms.Impl;

public class ScreenBoundaries : MonoBehaviour
{
    public Camera mainCamera; // יש לשייך את המצלמה הראשית
    public AudioClip collisionSound; // קובץ שמע בעת התנגשות
    private AudioSource audioSource;
    private EdgeCollider2D edgeCollider;
    public float bottomBoundary = -5f; // גבול תחתון למשחק
    public TMP_Text gameOverText; // שדה עבור הודעת "Game Over"
    public float rightBoundaryOffset = 0.8f; // להזיז את גבול ימין שמאלה
    public float leftBoundaryOffset = -1f; // להזיז את גבול שמאלה ימין
    public float boundaryThickness = 0.2f; // עובי השכבה שתחזיר את הכדור

    private float lastXPosition = 0f; // ערך X האחרון של הכדור
    private float timeSinceLastMovement = 0f; // זמן שעבר מאז הכדור לא זז
    private float movementTimeout = 0.25f; // רבע שנייה
    public UnityEvent OnGameEnd;
    public GameManager gameManager;
    void Awake()
    {
        edgeCollider = gameObject.AddComponent<EdgeCollider2D>();
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.clip = collisionSound;
        CreateBoundaries();
        if (OnGameEnd == null)
        {
            OnGameEnd = new UnityEvent(); // יצירת מופע של האירוע
        }
    }

    private void EndGame(GameObject player)
    {
        Debug.Log("rizz");
        if (player != null)
        {
            OnGameEnd.Invoke();
            // הצגת הודעת Game Over
            gameOverText.enabled = true;
            // עצירת המשחק
            Time.timeScale = 0;
            player.SetActive(true);
            gameManager.EndGame();
        }
    }

    void Update()
    {
        GameObject ball = GameObject.FindWithTag("Player");
        if (ball != null)
        {
            // בדיקה אם הכדור הגיע לגבול התחתון
            if (ball.transform.position.y < bottomBoundary)
            {
                EndGame(ball);
            }

            // בדיקה אם X של הכדור לא השתנה למשך יותר מרבע שנייה
            if (Mathf.Approximately(ball.transform.position.x, lastXPosition))
            {
                timeSinceLastMovement += Time.deltaTime;
                if (timeSinceLastMovement >= movementTimeout)
                {
                    // הכדור לא זז במשך רבע שנייה - נותנים לו דחיפה חזרה
                    Rigidbody2D rb = ball.GetComponent<Rigidbody2D>();
                    timeSinceLastMovement = 0f; // אפס את הזמן
                }
            }
            else
            {
                // אם הכדור זז, אפס את הזמן
                timeSinceLastMovement = 0f;
            }

            // עדכון ה-X האחרון
            lastXPosition = ball.transform.position.x;
        }
    }

    private void CreateBoundaries()
    {
        List<Vector2> edges = new List<Vector2>();

        // חישוב גבולות המסך
        Vector2 bottomLeft = mainCamera.ViewportToWorldPoint(new Vector2(0, 0)) + new Vector3(leftBoundaryOffset, 0, 0); // פינה שמאלית תחתונה
        Vector2 topRight = mainCamera.ViewportToWorldPoint(new Vector2(1, 1)) - new Vector3(rightBoundaryOffset, 0, 0); // ימין עליון

        // יצירת גבולות (ללא גבול תחתון)
        edges.Add(new Vector2(topRight.x, bottomLeft.y));  // ימין תחתון
        edges.Add(new Vector2(topRight.x, topRight.y));    // ימין עליון
        edges.Add(new Vector2(bottomLeft.x, topRight.y));  // שמאל עליון
        edges.Add(new Vector2(bottomLeft.x, bottomLeft.y)); // שמאל תחתון (כחיבור לולאה)

        edgeCollider.SetPoints(edges);

        // יצירת שכבת גבול דקה לקרוב לגבולות הימין, שמאל וקדימה (למעלה)
        CreateBounceLayer(topRight.x + boundaryThickness, topRight.y, new Vector2(1, 0)); // שכבת גבול ימין
        CreateBounceLayer(bottomLeft.x - boundaryThickness, topRight.y, new Vector2(-1, 0)); // שכבת גבול שמאל
        CreateBounceLayer(topRight.x, topRight.y + boundaryThickness, new Vector2(0, -1)); // שכבת גבול עליון
    }

    private void CreateBounceLayer(float x, float y, Vector2 direction)
    {
        GameObject bounceLayer = new GameObject("BounceLayer");
        bounceLayer.tag = "BounceLayer"; // ניתן להוסיף תגית שונה אם תרצה
        bounceLayer.transform.position = new Vector3(x, y, 0f);
        BoxCollider2D collider = bounceLayer.AddComponent<BoxCollider2D>();
        collider.isTrigger = true;
        collider.size = new Vector2(boundaryThickness, boundaryThickness); // יצירת גבול דק
        //bounceLayer.AddComponent<Rigidbody2D>().isKinematic = true; // להוסיף גוף דינמי שלא יפעל על עצמו
    }

    private void PlayCollisionSound()
    {
        if (collisionSound != null && audioSource != null)
        {
            audioSource.Play();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // בודק אם הפגיעה היא עם האובייקט "Player"
        if (collision.collider.CompareTag("Player"))
        {
            PlayCollisionSound(); // נגן את הסאונד אם מדובר ב-Player

            Rigidbody2D rb = collision.collider.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                // חישוב החזרה מיידית
                Vector2 normal = collision.contacts[0].normal;
                Vector2 reflectVelocity = Vector2.Reflect(rb.linearVelocity, normal);

                // הבטחת מינימום מהירות
                float minimumSpeed = 2f; // מהירות מינימלית
                if (reflectVelocity.magnitude < minimumSpeed)
                {
                    reflectVelocity = reflectVelocity.normalized * minimumSpeed;
                }

                rb.linearVelocity = reflectVelocity;

                // דחיפה קטנה להבטחת התרחקות מהגבול
                rb.position += normal * 0.1f;

                // מניעת סיבוב יתר
                rb.angularVelocity = 0f;
            }
        }
    }
}