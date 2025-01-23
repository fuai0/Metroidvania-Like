using UnityEngine;
using UnityEngine.SceneManagement;

public class UI_Options : MonoBehaviour
{

    public void BackToTitle()
    {
        SaveManager.instance.SaveGame();
        SceneManager.LoadScene("MainMenu");
        Time.timeScale = 1;
    }

    public void ExitGame()
    {
        SaveManager.instance.SaveGame();
        Application.Quit();
    }
}
