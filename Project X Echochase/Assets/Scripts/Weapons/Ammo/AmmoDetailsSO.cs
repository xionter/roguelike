using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AmmoDetails_", menuName = "Scriptable Objects/Weapons/Ammo Details")]
public class AmmoDetailsSO : ScriptableObject
{
    #region Header Основные параметры снаряда
    [Space(10)]
    [Header("ОСНОВНЫЕ ПАРАМЕТРЫ СНАРЯДА")]
    #endregion
    #region Tooltip
    [Tooltip("Название снаряда")]
    #endregion
    public string ammoName;
    public bool isPlayerAmmo;

    #region Header Спрайт, префаб и материалы снаряда
    [Space(10)]
    [Header("СПРАЙТ, ПРЕФАБ И МАТЕРИАЛЫ СНАРЯДА")]
    #endregion
    #region Tooltip
    [Tooltip("Спрайт, используемый для снаряда")]
    #endregion
    public Sprite ammoSprite;
    #region Tooltip
    [Tooltip("Укажите префаб для снаряда. Если указано несколько префабов, будет выбран случайный. Префаб может быть шаблоном снаряда, если он соответствует интерфейсу IFireable.")]
    #endregion
    public GameObject[] ammoPrefabArray;
    #region Tooltip
    [Tooltip("Материал, используемый для снаряда")]
    #endregion
    public Material ammoMaterial;
    #region Tooltip
    [Tooltip("Если снаряд должен 'заряжаться' перед выстрелом, укажите время в секундах, которое он будет заряжаться перед выпуском")]
    #endregion
    public float ammoChargeTime = 0.1f;
    #region Tooltip
    [Tooltip("Если у снаряда есть время заряда, укажите материал для отображения снаряда во время заряда")]
    #endregion
    public Material ammoChargeMaterial;
    #region Эффект попадания снаряда
    #region Эффект попадания снаряда
    [Space(10)]
    [Header("ЭФФЕКТ ПОПАДАНИЯ СНАРЯДА")]
    #endregion
    #region Tooltip
    [Tooltip("Scriptable Object, определяющий параметры эффекта попадания")]
    #endregion
    #endregion
    public AmmoHitEffectSO ammoHitEffect;

    #region Header Базовые параметры снаряда
    [Space(10)]
    [Header("БАЗОВЫЕ ПАРАМЕТРЫ СНАРЯДА")]
    #endregion
    #region Tooltip
    [Tooltip("Урон, наносимый снарядом")]
    #endregion
    public int ammoDamage = 1;
    #region Tooltip
    [Tooltip("Минимальная скорость снаряда - скорость будет случайным значением между min и max")]
    #endregion
    public float ammoSpeedMin = 20f;
    #region Tooltip
    [Tooltip("Максимальная скорость снаряда - скорость будет случайным значением между min и max")]
    #endregion
    public float ammoSpeedMax = 20f;
    #region Tooltip
    [Tooltip("Дальность снаряда (или шаблона снаряда) в юнитах Unity")]
    #endregion
    public float ammoRange = 20f;
    #region Tooltip
    [Tooltip("Скорость вращения шаблона снаряда в градусах в секунду")]
    #endregion
    public float ammoRotationSpeed = 1f;

    #region Header Параметры разброса снаряда
    [Space(10)]
    [Header("ПАРАМЕТРЫ РАЗБРОСА СНАРЯДА")]
    #endregion
    #region Tooltip
    [Tooltip("Минимальный угол разброса снаряда. Больший разброс означает меньшую точность. Случайный разброс рассчитывается между min и max значениями.")]
    #endregion
    public float ammoSpreadMin = 0f;
    #region Tooltip
    [Tooltip("Максимальный угол разброса снаряда. Больший разброс означает меньшую точность. Случайный разброс рассчитывается между min и max значениями.")]
    #endregion
    public float ammoSpreadMax = 0f;

    #region Header Параметры создания снарядов
    [Space(10)]
    [Header("ПАРАМЕТРЫ СОЗДАНИЯ СНАРЯДОВ")]
    #endregion
    #region Tooltip
    [Tooltip("Минимальное количество снарядов, создаваемых за выстрел. Количество будет случайным между min и max значениями.")]
    #endregion
    public int ammoSpawnAmountMin = 1;
    #region Tooltip
    [Tooltip("Максимальное количество снарядов, создаваемых за выстрел. Количество будет случайным между min и max значениями.")]
    #endregion
    public int ammoSpawnAmountMax = 1;
    #region Tooltip
    [Tooltip("Минимальный интервал между созданием снарядов. Интервал в секундах будет случайным между min и max значениями.")]
    #endregion
    public float ammoSpawnIntervalMin = 0f;
    #region Tooltip
    [Tooltip("Максимальный интервал между созданием снарядов. Интервал в секундах будет случайным между min и max значениями.")]
    #endregion
    public float ammoSpawnIntervalMax = 0f;

    #region Header Параметры следа снаряда
    [Space(10)]
    [Header("ПАРАМЕТРЫ СЛЕДА СНАРЯДА")]
    #endregion
    #region Tooltip
    [Tooltip("Отметьте, если нужен след от снаряда. Если отмечено, заполните остальные параметры следа.")]
    #endregion
    public bool isAmmoTrail = false;
    #region Tooltip
    [Tooltip("Время жизни следа снаряда в секундах.")]
    #endregion
    public float ammoTrailTime = 3f;
    #region Tooltip
    [Tooltip("Материал следа снаряда.")]
    #endregion
    public Material ammoTrailMaterial;
    #region Tooltip
    [Tooltip("Начальная ширина следа снаряда.")]
    #endregion
    [Range(0f, 1f)] public float ammoTrailStartWidth;
    #region Tooltip
    [Tooltip("Конечная ширина следа снаряда")]
    #endregion
    [Range(0f, 1f)] public float ammoTrailEndWidth;

    #region Validation
#if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtilities.ValidateCheckEmptyString(this, nameof(ammoName), ammoName);
        HelperUtilities.ValidateCheckNullValue(this, nameof(ammoSprite), ammoSprite);
        HelperUtilities.ValidateCheckEnumerableValues(this, nameof(ammoPrefabArray), ammoPrefabArray);
        HelperUtilities.ValidateCheckNullValue(this, nameof(ammoMaterial), ammoMaterial);
        if (ammoChargeTime > 0)
            HelperUtilities.ValidateCheckNullValue(this, nameof(ammoChargeMaterial), ammoChargeMaterial);
        HelperUtilities.ValidateCheckPositiveValue(this, nameof(ammoDamage), ammoDamage, false);
        HelperUtilities.ValidateCheckPositiveRange(this, nameof(ammoSpeedMin), ammoSpeedMin, nameof(ammoSpeedMax), ammoSpeedMax, false);
        HelperUtilities.ValidateCheckPositiveValue(this, nameof(ammoRange), ammoRange, false);
        HelperUtilities.ValidateCheckPositiveRange(this, nameof(ammoSpreadMin), ammoSpreadMin, nameof(ammoSpreadMax), ammoSpreadMax, true);
        HelperUtilities.ValidateCheckPositiveRange(this, nameof(ammoSpawnAmountMin), ammoSpawnAmountMin, nameof(ammoSpawnAmountMax), ammoSpawnAmountMax, false);
        HelperUtilities.ValidateCheckPositiveRange(this, nameof(ammoSpawnIntervalMin), ammoSpawnIntervalMin, nameof(ammoSpawnIntervalMax), ammoSpawnIntervalMax, true);
        if (isAmmoTrail)
        {
            HelperUtilities.ValidateCheckPositiveValue(this, nameof(ammoTrailTime), ammoTrailTime, false);
            HelperUtilities.ValidateCheckNullValue(this, nameof(ammoTrailMaterial), ammoTrailMaterial);
            HelperUtilities.ValidateCheckPositiveValue(this, nameof(ammoTrailStartWidth), ammoTrailStartWidth, false);
            HelperUtilities.ValidateCheckPositiveValue(this, nameof(ammoTrailEndWidth), ammoTrailEndWidth, false);
        }
    }

#endif
    #endregion
}
