using UnityEngine;

public class ObstacleMovement : MonoBehaviour
{
    public float speed = 2f; // ������ ������ (public ��� ����� ����� �� �� ���� ������)
    public float range = 3f; // ���� ������

    private Vector3 startPos;

    void Start()
    {
        startPos = transform.position; // ���� �� ������ ������� �� ������
    }
    public void IncreaseSpeed(float amount)
    {
        speed += amount; // ����� ������ ������
    }
    void Update()
    {
        transform.position = startPos + Vector3.right * Mathf.Sin(Time.time * speed) * range;
    }

}