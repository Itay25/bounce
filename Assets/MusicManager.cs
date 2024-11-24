using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public AudioClip backgroundMusic; // ����� ���� ���� �������
    private AudioSource audioSource;   // ���� �- AudioSource ����� �� �������

    void Start()
    {
        audioSource = GetComponent<AudioSource>(); // ���� �� ���� �-AudioSource
        if (audioSource != null && backgroundMusic != null)
        {
            audioSource.clip = backgroundMusic; // ����� �� ���� �������
            audioSource.loop = true; // ����� ������� ����� ������
            audioSource.Play(); // ���� �� �������
        }
    }
}
