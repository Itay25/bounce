using UnityEngine;

public class PlatformMovement : MonoBehaviour
{
    public float speed = 17f;

    private float leftBoundary;  // הגבול השמאלי
    private float rightBoundary; // הגבול הימני
    private float platformWidth; // רוחב הפלטפורמה

    void Start()
    {
        // חישוב גבולות המסך
        float screenWidthInWorldUnits = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, 0, 0)).x;
        leftBoundary = -screenWidthInWorldUnits;
        rightBoundary = screenWidthInWorldUnits;

        // חישוב רוחב הפלטפורמה
        platformWidth = GetComponent<SpriteRenderer>().bounds.extents.x; // חצי רוחב הפלטפורמה
    }

    void Update()
    {

        Vector3 newPosition = transform.position;

        // שליטה במחשב
        if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
        {
            newPosition += Vector3.left * speed * Time.deltaTime;
        }
        else if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
        {
            newPosition += Vector3.right * speed * Time.deltaTime;
        }

        // שליטה בטלפון
        foreach (Touch touch in Input.touches)
        {
            if (touch.position.x < Screen.width / 2)
            {
                newPosition += Vector3.left * speed * Time.deltaTime;
            }
            else if (touch.position.x > Screen.width / 2)
            {
                newPosition += Vector3.right * speed * Time.deltaTime;
            }
        }

        // בדיקה שהמיקום החדש נמצא בתוך הגבולות (עם התחשבות ברוחב הפלטפורמה)
        newPosition.x = Mathf.Clamp(newPosition.x, leftBoundary + platformWidth, rightBoundary - platformWidth);

        // עדכון מיקום הפלטפורמה
        transform.position = newPosition;
    }
}