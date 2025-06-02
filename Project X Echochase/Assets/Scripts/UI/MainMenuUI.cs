using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] private GameObject playButton;
    [SerializeField] private GameObject quitButton;
    [SerializeField] private GameObject returnToMainMenuButton;
    [SerializeField] private GameObject instructionsButton;

    private void Start()
    {
        MusicManager.Instance.PlayMusic(GameResources.Instance.mainMenuMusic, 0f, 2f);
        returnToMainMenuButton.SetActive(false);
        playButton.SetActive(true);
    }

    public void PlayGame()
    {
        SceneManager.LoadScene("IntroCutscene");
    }

    public void LoadInitial()
    {
        SceneManager.UnloadSceneAsync("InstructionScene");
        returnToMainMenuButton.SetActive(false);
        instructionsButton.SetActive(true);
        playButton.SetActive(true);
        quitButton.SetActive(true);
    }

    public void LoadInstructions()
    {
        playButton.SetActive(false);
        quitButton.SetActive(false);
        instructionsButton.SetActive(false);

        returnToMainMenuButton.SetActive(true);

        SceneManager.LoadScene("InstructionScene", LoadSceneMode.Additive);
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
        HelperUtilities.ValidateCheckNullValue(this, nameof(instructionsButton), instructionsButton);
    }
#endif
}