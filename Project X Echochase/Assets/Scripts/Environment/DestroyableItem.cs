using System.Collections;
using UnityEngine;

//Не добавляйте директивы require, так как мы уничтожаем компоненты при уничтожении элемента
[DisallowMultipleComponent]
public class DestroyableItem : MonoBehaviour
{
    #region Header HEALTH
    [Header("ЗДОРОВЬЕ")]
    #endregion Header HEALTH
    #region Tooltip
    [Tooltip("Каким должно быть начальное здоровье этого разрушаемого предмета")]
    #endregion Tooltip
    [SerializeField] private int startingHealthAmount = 1;
    #region SOUND EFFECT
    [Header("ЗВУКОВОЙ ЭФФЕКТ")]
    #endregion SOUND EFFECT
    #region Tooltip
    [Tooltip("Звуковой эффект при разрушении предмета")]
    #endregion Tooltip
    [SerializeField] private SoundEffectSO destroySoundEffect;
    private Animator animator;
    private BoxCollider2D boxCollider2D;
    private HealthEvent healthEvent;
    private Health health;
    private ReceiveContactDamage receiveContactDamage;
    private LightFlicker lightFlicker;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        boxCollider2D = GetComponent<BoxCollider2D>();
        healthEvent = GetComponent<HealthEvent>();
        health = GetComponent<Health>();
        health.SetStartingHealth(startingHealthAmount);
        receiveContactDamage = GetComponent<ReceiveContactDamage>();
        lightFlicker = GetComponent<LightFlicker>();
    }

    private void OnEnable()
    {
        healthEvent.OnHealthChanged += HealthEvent_OnHealthLost;
    }

    private void OnDisable()
    {
        healthEvent.OnHealthChanged -= HealthEvent_OnHealthLost;
    }

    private void HealthEvent_OnHealthLost(HealthEvent healthEvent, HealthEventArgs healthEventArgs)
    {
        if (healthEventArgs.healthAmount <= 0f)
        {
            StartCoroutine(PlayAnimation());
        }
    }

    private IEnumerator PlayAnimation()
    {
        // Уничтожить триггерный коллайдер
        Destroy(boxCollider2D);

        // Воспроизвести звуковой эффект
        if (destroySoundEffect != null)
        {
            SoundEffectManager.Instance.PlaySoundEffect(destroySoundEffect);
        }

        // Запустить анимацию уничтожения
        animator.SetBool(Settings.destroy, true);

        // Позволить анимации воспроизводиться
        while (!animator.GetCurrentAnimatorStateInfo(0).IsName(Settings.stateDestroyed))
        {
            yield return null;
        }

        //Затем уничтожить все компоненты, кроме Sprite Renderer, чтобы просто отобразить финальный спрайт в анимации
        Destroy(animator);
        Destroy(receiveContactDamage);
        Destroy(health);
        Destroy(healthEvent);
        Destroy(lightFlicker.light2D);
        Destroy(lightFlicker);
        Destroy(this);
    }
}
