using UnityEngine;

public class BackgroundFollow : MonoBehaviour
{
    public Transform ball; // �����
    public Vector3 offset; // ���� ���� ��� ���� �����
    public float movementFactor = 5f; // ��� ������ ���� ����, ���� ���� ����

    void Update()
    {
        if (ball != null)
        {
            // ����� ����� ���� ����� ������ �� �����, ������ �����
            float newY = ball.position.y / movementFactor + offset.y;
            transform.position = new Vector3(transform.position.x, newY, transform.position.z);
        }
        else
        {
            Debug.LogWarning("Ball is not assigned in the Inspector!");
        }
    }
}
