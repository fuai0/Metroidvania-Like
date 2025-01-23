using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UI_MainMenu : MonoBehaviour
{
    [SerializeField] private string scenceName = "MainScence";
    [SerializeField] private GameObject continueButton;
    [SerializeField] private UI_FadeScreen fadeScreen;

    private void Start()
    {
        if (!SaveManager.instance.HasSavedData())
            continueButton.SetActive(false);
    }

    public void ContinueGame()
    {
        StartCoroutine(LoadScenceWithFadeEffect(1.5f));
    }

    public void NewGame()
    {
        SaveManager.instance.DeleteSaveDate();
        StartCoroutine(LoadScenceWithFadeEffect(1.5f));
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    IEnumerator LoadScenceWithFadeEffect(float _delay)
    {
        fadeScreen.FadeOut();

        yield return new WaitForSeconds(_delay);

        SceneManager.LoadScene(scenceName);
    }
}
