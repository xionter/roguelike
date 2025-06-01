using UnityEngine;
using UnityEngine.SceneManagement;

public class IntroCutscene : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private string animationName = "IntroAnimation";
    [SerializeField] private float delayAfterAnimation = 1f;
    [SerializeField] private bool skipCutscene = false;
    
    private bool isCutscenePlaying = false;
    private float cutsceneDuration = 35f; 

    private void Start()
    {
        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }

        if (skipCutscene)
        {
            LoadGameScene();
            return;
        }

        PlayCutscene();
    }

    private void Update()
    {
        if (isCutscenePlaying && Input.GetKeyDown(KeyCode.Return))
        {
            SkipCutscene();
        }
    }

    private void PlayCutscene()
    {
        isCutscenePlaying = true;
        if (animator != null && !string.IsNullOrEmpty(animationName))
        {
            animator.Play(animationName, -1, 0f);
            Invoke("LoadGameScene", cutsceneDuration + delayAfterAnimation);
        }
        else
        {
            LoadGameScene(); 
        }
    }

    private void LoadGameScene()
    {
        if (!isCutscenePlaying) return;
        
        isCutscenePlaying = false;
        SceneManager.LoadScene("MainGameScene");
    }

    private void SkipCutscene()
    {
        if (!isCutscenePlaying) return;
        
        CancelInvoke("LoadGameScene");
        LoadGameScene();
        
        if (animator != null)
        {
            animator.SetTrigger("Skip");
        }
    }
}