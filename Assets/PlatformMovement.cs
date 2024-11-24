using UnityEngine;

public class PlatformMovement : MonoBehaviour
{
    public float speed = 17f;

    private float leftBoundary;  // ����� ������
    private float rightBoundary; // ����� �����
    private float platformWidth; // ���� ���������

    void Start()
    {
        // ����� ������ ����
        float screenWidthInWorldUnits = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, 0, 0)).x;
        leftBoundary = -screenWidthInWorldUnits;
        rightBoundary = screenWidthInWorldUnits;

        // ����� ���� ���������
        platformWidth = GetComponent<SpriteRenderer>().bounds.extents.x; // ��� ���� ���������
    }

    void Update()
    {

        Vector3 newPosition = transform.position;

        // ����� �����
        if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
        {
            newPosition += Vector3.left * speed * Time.deltaTime;
        }
        else if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
        {
            newPosition += Vector3.right * speed * Time.deltaTime;
        }

        // ����� ������
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

        // ����� ������� ���� ���� ���� ������� (�� ������� ����� ���������)
        newPosition.x = Mathf.Clamp(newPosition.x, leftBoundary + platformWidth, rightBoundary - platformWidth);

        // ����� ����� ���������
        transform.position = newPosition;
    }
}