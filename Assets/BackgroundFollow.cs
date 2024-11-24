using UnityEngine;

public class BackgroundFollow : MonoBehaviour
{
    public Transform ball; // הכדור
    public Vector3 offset; // מרחק קבוע בין הרקע לכדור
    public float movementFactor = 5f; // ככל שהמספר גבוה יותר, הרקע יזוז פחות

    void Update()
    {
        if (ball != null)
        {
            // עדכון מיקום הרקע בהתאם לתנועה של הכדור, מחולקת במקדם
            float newY = ball.position.y / movementFactor + offset.y;
            transform.position = new Vector3(transform.position.x, newY, transform.position.z);
        }
        else
        {
            Debug.LogWarning("Ball is not assigned in the Inspector!");
        }
    }
}
