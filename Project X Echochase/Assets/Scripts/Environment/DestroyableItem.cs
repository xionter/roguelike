using System.Collections;
using UnityEngine;

//�� ���������� ��������� require, ��� ��� �� ���������� ���������� ��� ����������� ��������
[DisallowMultipleComponent]
public class DestroyableItem : MonoBehaviour
{
    #region Header HEALTH
    [Header("��������")]
    #endregion Header HEALTH
    #region Tooltip
    [Tooltip("����� ������ ���� ��������� �������� ����� ������������ ��������")]
    #endregion Tooltip
    [SerializeField] private int startingHealthAmount = 1;
    #region SOUND EFFECT
    [Header("�������� ������")]
    #endregion SOUND EFFECT
    #region Tooltip
    [Tooltip("�������� ������ ��� ���������� ��������")]
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
        // ���������� ���������� ���������
        Destroy(boxCollider2D);

        // ������������� �������� ������
        if (destroySoundEffect != null)
        {
            SoundEffectManager.Instance.PlaySoundEffect(destroySoundEffect);
        }

        // ��������� �������� �����������
        animator.SetBool(Settings.destroy, true);

        // ��������� �������� ����������������
        while (!animator.GetCurrentAnimatorStateInfo(0).IsName(Settings.stateDestroyed))
        {
            yield return null;
        }

        //����� ���������� ��� ����������, ����� Sprite Renderer, ����� ������ ���������� ��������� ������ � ��������
        Destroy(animator);
        Destroy(receiveContactDamage);
        Destroy(health);
        Destroy(healthEvent);
        Destroy(lightFlicker.light2D);
        Destroy(lightFlicker);
        Destroy(this);
    }
}
