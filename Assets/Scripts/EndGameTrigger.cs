using UnityEngine;
using UnityEngine.SceneManagement;

public class EndGameTrigger : MonoBehaviour
{
    public string endSceneName = "EndGameScene";

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Hi");
            SceneManager.LoadScene(endSceneName);
        }
    }
}