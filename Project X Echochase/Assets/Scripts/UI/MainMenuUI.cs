using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuUI : MonoBehaviour
{
    #region Header OBJECT REFERENCES
    [Space(10)]
    [Header("РЕФЕРЕНСЫ ОБЪЕКТОВ")]
    #endregion Header OBJECT REFERENCES
    #region Tooltip
    [Tooltip("Кнопка старта")]
    #endregion Tooltip
    [SerializeField] private GameObject playButton;
    #region Tooltip
    [Tooltip("Кнопка выхода")]
    #endregion
    [SerializeField] private GameObject quitButton;
    #region Tooltip
    [Tooltip("Кнопка возвращения в главное меню")]
    #endregion
    [SerializeField] private GameObject returnToMainMenuButton;

    private void Start()
    {
        MusicManager.Instance.PlayMusic(GameResources.Instance.mainMenuMusic, 0f, 2f);

        SceneManager.LoadScene("InitialScene", LoadSceneMode.Additive);

        returnToMainMenuButton.SetActive(false);
    }


    /// <summary>
    /// вызывается из Play Game / Enter The Dungeon
    /// </summary>
    public void PlayGame()
    {
        SceneManager.LoadScene("MainGameScene");
    }

    /// <summary>
    /// вызывается из Return To Main Menu
    /// </summary>
    public void LoadInitial()
    {
        returnToMainMenuButton.SetActive(false);
        playButton.SetActive(true);
        quitButton.SetActive(true);
        SceneManager.LoadScene("InitialScene", LoadSceneMode.Additive);
    }


    public void QuitGame()
    {
        Application.Quit();
    }


    #region Validation
#if UNITY_EDITOR
    // Validate the scriptable object details entered
    private void OnValidate()
    {
        HelperUtilities.ValidateCheckNullValue(this, nameof(playButton), playButton);
        HelperUtilities.ValidateCheckNullValue(this, nameof(quitButton), quitButton);
        HelperUtilities.ValidateCheckNullValue(this, nameof(returnToMainMenuButton), returnToMainMenuButton);
    }
#endif
    #endregion
}
