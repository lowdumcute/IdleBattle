using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    // Chuyển sang scene theo tên
    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    // Chuyển sang scene theo index
    public void LoadScene(int sceneIndex)
    {
        SceneManager.LoadScene(sceneIndex);
    }

    // Thoát game
    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Game closed");
    }
}
