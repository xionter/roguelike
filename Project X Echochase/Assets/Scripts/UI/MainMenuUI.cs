using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] private GameObject playButton;
    [SerializeField] private GameObject quitButton;
    [SerializeField] private GameObject returnToMainMenuButton;

    private void Start()
    {
        MusicManager.Instance.PlayMusic(GameResources.Instance.mainMenuMusic, 0f, 2f);
        SceneManager.LoadScene("MainMenuScene", LoadSceneMode.Additive);
        returnToMainMenuButton.SetActive(false);
    }

    public void PlayGame()
    {
        SceneManager.LoadScene("MainGameScene");
    }

    public void LoadInitial()
    {
        returnToMainMenuButton.SetActive(false);
        playButton.SetActive(true);
        quitButton.SetActive(true);
        SceneManager.LoadScene("MainMenuScene", LoadSceneMode.Additive);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtilities.ValidateCheckNullValue(this, nameof(playButton), playButton);
        HelperUtilities.ValidateCheckNullValue(this, nameof(quitButton), quitButton);
        HelperUtilities.ValidateCheckNullValue(this, nameof(returnToMainMenuButton), returnToMainMenuButton);
    }
#endif
}