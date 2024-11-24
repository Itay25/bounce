using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public AudioClip backgroundMusic; // משתנה עבור קובץ המוזיקה
    private AudioSource audioSource;   // רכיב ה- AudioSource שינגן את המוזיקה

    void Start()
    {
        audioSource = GetComponent<AudioSource>(); // משיג את רכיב ה-AudioSource
        if (audioSource != null && backgroundMusic != null)
        {
            audioSource.clip = backgroundMusic; // משייך את קובץ המוזיקה
            audioSource.loop = true; // מאפשר למוזיקה לחזור בלופים
            audioSource.Play(); // מנגן את המוזיקה
        }
    }
}
