using UnityEngine;

[CreateAssetMenu(fileName = "WeaponShootEffect_", menuName = "Scriptable Objects/Weapons/Weapon Shoot Effect")]
public class WeaponShootEffectSO : ScriptableObject
{
    #region Header ДЕТАЛИ ЭФФЕКТА ВЫСТРЕЛА ОРУЖИЯ
    [Space(10)]
    [Header("WEAPON SHOOT EFFECT DETAILS")]
    #endregion Header ДЕТАЛИ ЭФФЕКТА ВЫСТРЕЛА ОРУЖИЯ

    #region Tooltip
    [Tooltip("Градиент цвета для эффекта выстрела. Показывает изменение цвета частиц в течение их времени жизни (слева направо)")]
    #endregion Tooltip
    public Gradient colorGradient;

    #region Tooltip
    [Tooltip("Длительность эмиссии частиц системы частиц")]
    #endregion Tooltip
    public float duration = 0.50f;

    #region Tooltip
    [Tooltip("Начальный размер частиц эффекта")]
    #endregion Tooltip
    public float startParticleSize = 0.25f;

    #region Tooltip
    [Tooltip("Начальная скорость частиц эффекта")]
    #endregion Tooltip
    public float startParticleSpeed = 3f;

    #region Tooltip
    [Tooltip("Время жизни частиц эффекта")]
    #endregion Tooltip
    public float startLifetime = 0.5f;

    #region Tooltip
    [Tooltip("Максимальное количество эмитируемых частиц")]
    #endregion Tooltip
    public int maxParticleNumber = 100;

    #region Tooltip
    [Tooltip("Количество частиц, испускаемых в секунду. Если 0 - будет использовано только количество из burst")]
    #endregion Tooltip
    public int emissionRate = 100;

    #region Tooltip
    [Tooltip("Количество частиц, выброшенных за один раз (burst)")]
    #endregion Tooltip
    public int burstParticleNumber = 20;

    #region Tooltip
    [Tooltip("Гравитация, воздействующая на частицы. Небольшое отрицательное значение заставит их подниматься вверх")]
    #endregion Tooltip
    public float effectGravity = -0.01f;

    #region Tooltip
    [Tooltip("Спрайт для эффекта частиц. Если не указан - будет использован стандартный спрайт частиц")]
    #endregion Tooltip
    public Sprite sprite;

    #region Tooltip
    [Tooltip("Минимальная скорость частицы за время её жизни. Будет выбрано случайное значение между min и max")]
    #endregion Tooltip
    public Vector3 velocityOverLifetimeMin;

    #region Tooltip
    [Tooltip("Максимальная скорость частицы за время её жизни. Будет выбрано случайное значение между min и max")]
    #endregion Tooltip
    public Vector3 velocityOverLifetimeMax;

    #region Tooltip
    [Tooltip("Префаб weaponShootEffectPrefab содержит систему частиц для эффекта выстрела и настраивается через weaponShootEffect SO")]
    #endregion Tooltip
    public GameObject weaponShootEffectPrefab;

    #region Validation

#if UNITY_EDITOR

    private void OnValidate()
    {
        HelperUtilities.ValidateCheckPositiveValue(this, nameof(duration), duration, false);
        HelperUtilities.ValidateCheckPositiveValue(this, nameof(startParticleSize), startParticleSize, false);
        HelperUtilities.ValidateCheckPositiveValue(this, nameof(startParticleSpeed), startParticleSpeed, false);
        HelperUtilities.ValidateCheckPositiveValue(this, nameof(startLifetime), startLifetime, false);
        HelperUtilities.ValidateCheckPositiveValue(this, nameof(maxParticleNumber), maxParticleNumber, false);
        HelperUtilities.ValidateCheckPositiveValue(this, nameof(emissionRate), emissionRate, true);
        HelperUtilities.ValidateCheckPositiveValue(this, nameof(burstParticleNumber), burstParticleNumber, true);
        HelperUtilities.ValidateCheckNullValue(this, nameof(weaponShootEffectPrefab), weaponShootEffectPrefab);
    }

#endif

    #endregion Validation
}
