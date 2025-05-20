using UnityEngine;

[CreateAssetMenu(fileName = "SoundEffect_", menuName = "Scriptable Objects/Sounds/SoundEffect")]
public class SoundEffectSO : ScriptableObject
{
    #region Header SOUND EFFECT DETAILS
    [Space(10)]
    [Header("������ ��������� �������")]
    #endregion
    #region Tooltip
    [Tooltip("�������� ��������� �������")]
    #endregion
    public string soundEffectName;
    #region Tooltip
    [Tooltip("��������� ��� ��������� �������")]
    #endregion
    public GameObject soundPrefab;
    #region Tooltip
    [Tooltip("��������� ��� ��������� �������")]
    #endregion
    public AudioClip soundEffectClip;
    #region Tooltip
    [Tooltip("����������� ��������� ������ ���� ��� ��������� �������. ��������� ��������� ������ ���� ����� ������������� ����� ����������� � ������������ ����������. ��������� ��������� ������ ���� ������ �������� ������� ����� �������������.")]
    #endregion
    [Range(0.1f, 1.5f)]
    public float soundEffectPitchRandomVariationMin = 0.8f;
    #region Tooltip
    [Tooltip("������������ ��������� ������ ���� ��� ��������� �������. ��������� ��������� ������ ���� ����� ������������� ����� ����������� � ������������ ����������. ��������� ��������� ������ ���� ������ �������� ������� ����� �������������.")]
    #endregion
    [Range(0.1f, 1.5f)]
    public float soundEffectPitchRandomVariationMax = 1.2f;
    #region Tooltip
    [Tooltip("��������� ��������� �������")]
    #endregion
    [Range(0f, 1f)]
    public float soundEffectVolume = 1f;

    #region Validation
#if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtilities.ValidateCheckEmptyString(this, nameof(soundEffectName), soundEffectName);
        HelperUtilities.ValidateCheckNullValue(this, nameof(soundPrefab), soundPrefab);
        HelperUtilities.ValidateCheckNullValue(this, nameof(soundEffectClip), soundEffectClip);
        HelperUtilities.ValidateCheckPositiveRange(this, nameof(soundEffectPitchRandomVariationMin), soundEffectPitchRandomVariationMin,
            nameof(soundEffectPitchRandomVariationMax), soundEffectPitchRandomVariationMax, false);
        HelperUtilities.ValidateCheckPositiveValue(this, nameof(soundEffectVolume), soundEffectVolume, true);
    }
#endif
    #endregion
}
