using UnityEngine;

public class ObstacleMovement : MonoBehaviour
{
    public float speed = 2f; // מהירות התנועה (public כדי שנוכל לשנות את זה מחוץ למחלקה)
    public float range = 3f; // טווח התנועה

    private Vector3 startPos;

    void Start()
    {
        startPos = transform.position; // שומר את המיקום ההתחלתי של המכשול
    }
    public void IncreaseSpeed(float amount)
    {
        speed += amount; // הגברת מהירות המכשול
    }
    void Update()
    {
        transform.position = startPos + Vector3.right * Mathf.Sin(Time.time * speed) * range;
    }

}