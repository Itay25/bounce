using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Coin : MonoBehaviour
{
    public GameManager gameManager;  // Reference to the GameManager
    public float minX = -2f;         // Minimum X position for spawning
    public float maxX = 2f;          // Maximum X position for spawning
    public float minY = 1f;          // Minimum Y position for spawning
    public float maxY = 3f;          // Maximum Y position for spawning
    public AudioClip collectSound;   // Sound to play when the coin is collected
    private AudioSource audioSource; // Audio source to play the sound

    public float scaleIncrease = 0.005f; // ��� ����� ����� ��� ����� ����
    public float maxBallSize = 1f;  // ����� �������� �� �����

    public AnimationCurve scaleCurve;  // An AnimationCurve for smooth scaling effect
    public GameObject particleEffectPrefab;
    void Start()
    {
        // ���� �� �-AudioSource ��������� ������
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            // ����� ���� AudioSource, ����� ���
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }
    private void SpawnParticleEffect()
    {
        if (particleEffectPrefab != null)
        {
            // ����� �������� ������ �����
            GameObject particles = Instantiate(particleEffectPrefab, transform.position, Quaternion.identity);

            // ����� �������� ���� ��� ���
            Destroy(particles, 2f); // ��� ���� ������ ��������
        }
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))  // When the player touches the coin
        {
            // Play the collect sound
            if (collectSound != null)
            {
                audioSource.PlayOneShot(collectSound);
            }
            SpawnParticleEffect();
            // Update score in GameManager
            gameManager.scoreText.enabled = false;
            gameManager.AddScore(1);// ����� ������ �� ����� ������, ���� �������� AddScore
            if(gameManager.score > 0)
                gameManager.scoreText.enabled = true;

            // ����� ����� ������, �� ���� �� �������
            StartCoroutine(ScaleBallWithAnimation(other));

     

            // Respawn the coin at a random X and Y position within the defined ranges
            float randomX = Random.Range(minX, maxX);
            float randomY = Random.Range(minY, maxY);
            transform.position = new Vector3(randomX, randomY, transform.position.z);
        }
    }

    private IEnumerator ScaleBallWithAnimation(Collider2D other)
    {
        Vector3 originalScale = other.transform.localScale;
        float duration = 0.2f;  // ��� ��������
        float time = 0f;

        while (time < duration)
        {
            // ����� ��������� �� ���� �������� ����� �� ����� �����
            float scaleFactor = scaleCurve.Evaluate(time / duration);
            other.transform.localScale = Vector3.Lerp(originalScale, new Vector3(maxBallSize, maxBallSize, 1), scaleFactor);
            time += Time.deltaTime;
            yield return null;
        }
    }
}
