using UnityEngine;

[CreateAssetMenu(fileName = "SoundEffect_", menuName = "Scriptable Objects/Sounds/SoundEffect")]
public class SoundEffectSO : ScriptableObject
{
    #region Header SOUND EFFECT DETAILS
    [Space(10)]
    [Header("ДЕТАЛИ ЗВУКОВОГО ЭФФЕКТА")]
    #endregion
    #region Tooltip
    [Tooltip("Название звукового эффекта")]
    #endregion
    public string soundEffectName;
    #region Tooltip
    [Tooltip("Заготовка для звукового эффекта")]
    #endregion
    public GameObject soundPrefab;
    #region Tooltip
    [Tooltip("Аудиоклип для звукового эффекта")]
    #endregion
    public AudioClip soundEffectClip;
    #region Tooltip
    [Tooltip("Минимальное изменение высоты тона для звукового эффекта. Случайное изменение высоты тона будет сгенерировано между минимальным и максимальным значениями. Случайное изменение высоты тона делает звуковые эффекты более естественными.")]
    #endregion
    [Range(0.1f, 1.5f)]
    public float soundEffectPitchRandomVariationMin = 0.8f;
    #region Tooltip
    [Tooltip("Максимальное изменение высоты тона для звукового эффекта. Случайное изменение высоты тона будет сгенерировано между минимальным и максимальным значениями. Случайное изменение высоты тона делает звуковые эффекты более естественными.")]
    #endregion
    [Range(0.1f, 1.5f)]
    public float soundEffectPitchRandomVariationMax = 1.2f;
    #region Tooltip
    [Tooltip("Громкость звукового эффекта")]
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
