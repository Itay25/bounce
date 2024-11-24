using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class BallBounce : MonoBehaviour
{
    public float bounceForce; // כוח הקפיצה המוגבר
    public AudioClip bounceSound; // קובץ הסאונד שיתנגן בעת פגיעת הכדור בפלטפורמה
    private Rigidbody2D rb;
    private AudioSource audioSource; // מרכיב להפקת סאונד
    public GameManager gameManager; // שדה חיצוני לאובייקט GameManager
    private int lastScore = 0;
    public PlatformMovement platformMovement;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>(); // משייך את ה-AudioSource

        // אם לא שויכה דרך ה-Inspector, נחפש את ה-GameManager אוטומטית
        if (gameManager == null)
        {
            gameManager = FindFirstObjectByType<GameManager>(); // מחפש את אובייקט ה-GameManager בסצנה
        }
    }
    void IncreaseBounceForce()
    {
        // מבצע את ההגברה בכל פעם שהניקוד עלה ב-5
        if (gameManager.score > lastScore && gameManager.score % 5 == 0)
        {
            bounceForce = Mathf.Min(bounceForce + 0.02f, 22f); // מגדיל ב-0.05 עד שמגיע ל-17
            platformMovement.speed = Mathf.Min(platformMovement.speed + 0.01f, 22f);
        }
    }
    void Update()
    {
        // אם הניקוד השתנה ב-5, מגדילים את כוח הקפיצה
        if (gameManager.score != lastScore)
        {
            IncreaseBounceForce(); // מגדיל את כוח הקפיצה
            lastScore = gameManager.score; // עדכון הניקוד הקודם
        }
    }
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Platform"))
        {
            // ניגון סאונד כאשר הכדור פוגע בפלטפורמה
            if (bounceSound != null && audioSource != null)
            {
                audioSource.PlayOneShot(bounceSound);
            }

            ContactPoint2D contact = collision.GetContact(0);
            float hitPoint = contact.point.x - collision.collider.bounds.center.x;
            Vector2 newDirection = new Vector2(hitPoint, 1.5f).normalized;
            rb.linearVelocity = newDirection * bounceForce; // חישוב כיוון קפיצה חדש
        }
        else if (collision.gameObject.CompareTag("Boundary"))
        {
            // שינוי כיוון הכדור כאשר הוא פוגע בגבול
            rb.linearVelocity = new Vector2(-rb.linearVelocity.x, rb.linearVelocity.y);

            // הוספת כוח כדי למנוע שהכדור ייתקע בגבול
            rb.AddForce(new Vector2(-Mathf.Sign(rb.linearVelocity.x) * 10f, 0));
        }
    }
}