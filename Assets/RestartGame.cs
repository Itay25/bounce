using UnityEngine;
using UnityEngine.SceneManagement;

public class RestartGame : MonoBehaviour
{
    public void Restart()
    {
        // טוען מחדש את הסצנה הנוכחית
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
