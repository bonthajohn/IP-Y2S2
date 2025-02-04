using UnityEngine;
using UnityEngine.SceneManagement; // Needed for scene loading

public class SceneChanger : MonoBehaviour
{
    public string sceneName; // Set this in the Inspector

    private void OnTriggerEnter(Collider other)
    {
        // Check if the object entering the trigger has the "Player" tag
        if (other.CompareTag("Player"))
        {
            SceneManager.LoadScene(sceneName);
        }
    }
}
