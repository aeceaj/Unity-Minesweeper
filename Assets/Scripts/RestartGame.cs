using UnityEngine;
using UnityEngine.SceneManagement;

public class RestartGame : MonoBehaviour
{
    public static void ReloadScene()
    {
        SceneManager.LoadScene(0);
    }
}
