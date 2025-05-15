using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AmmoDetails_", menuName = "Scriptable Objects/Weapons/Ammo Details")]
public class AmmoDetailsSO : ScriptableObject
{
    #region Header BASIC AMMO DETAILS
    [Space(10)]
    [Header("ОСНОВНЫЕ ДЕТАЛИ ПАТРОНОВ")]
    #endregion
    #region Tooltip
    [Tooltip("Название для патронов")]
    #endregion
    public string ammoName;
    public bool isPlayerAmmo;

    #region Header AMMO SPRITE, PREFAB & MATERIALS
    [Space(10)]
    [Header("СПРАЙТ, ПРЕФАБ И МАТЕРИАЛЫ ПАТРОНОВ")]
    #endregion
    #region Tooltip
    [Tooltip("Спрайт, который будет использоваться для патронов")]
    #endregion
    public Sprite ammoSprite;
    #region Tooltip
    [Tooltip("Укажите префаб, который будет использоваться для патронов. Если указано несколько префабов, то будет выбран случайный из массива. Префаб может быть паттерном патронов - если он соответствует интерфейсу IFireable.")]
    #endregion
    public GameObject[] ammoPrefabArray;
    #region Tooltip
    [Tooltip("Материал, который будет использоваться для патронов")]
    #endregion
    public Material ammoMaterial;
    #region Tooltip
    [Tooltip("Если патроны должны 'заряжаться' перед движением, укажите время в секундах, в течение которого патроны удерживаются перед выстрелом.")]
    #endregion
    public float ammoChargeTime = 0.1f;
    #region Tooltip
    [Tooltip("Если у патронов есть время зарядки, укажите материал, который будет использоваться для отображения патронов во время зарядки.")]
    #endregion
    public Material ammoChargeMaterial;
    #region Header AMMO HIT EFFECT
    /**
    #region Header AMMO HIT EFFECT
    [Space(10)]
    [Header("ЭФФЕКТ ПОПАДАНИЯ ПАТРОНОВ")]
    #endregion
    #region Tooltip
    [Tooltip("Скриптовый объект, который определяет параметры для префаба эффекта попадания")]
    #endregion*/
    #endregion
    //public AmmoHitEffectSO ammoHitEffect;

    #region Header AMMO BASE PARAMETERS
    [Space(10)]
    [Header("БАЗОВЫЕ ПАРАМЕТРЫ ПАТРОНОВ")]
    #endregion
    #region Tooltip
    [Tooltip("Урон, наносимый каждым патроном")]
    #endregion
    public int ammoDamage = 1;
    #region Tooltip
    [Tooltip("Минимальная скорость патронов - скорость будет случайным значением между минимальным и максимальным.")]
    #endregion
    public float ammoSpeedMin = 20f;
    #region Tooltip
    [Tooltip("Максимальная скорость патронов - скорость будет случайным значением между минимальным и максимальным.")]
    #endregion
    public float ammoSpeedMax = 20f;
    #region Tooltip
    [Tooltip("Дальность патронов (или паттерна патронов) в единицах Unity.")]
    #endregion
    public float ammoRange = 20f;
    #region Tooltip
    [Tooltip("Скорость вращения паттерна патронов в градусах в секунду.")]
    #endregion
    public float ammoRotationSpeed = 1f;

    #region Header AMMO SPREAD DETAILS
    [Space(10)]
    [Header("ДЕТАЛИ РАССЕИВАНИЯ ПАТРОНОВ")]
    #endregion
    #region Tooltip
    [Tooltip("Минимальный угол рассеивания патронов. Больший угол рассеивания означает меньшую точность. Случайное значение рассеивания рассчитывается между минимальным и максимальным значениями.")]
    #endregion
    public float ammoSpreadMin = 0f;
    #region Tooltip
    [Tooltip("Максимальный угол рассеивания патронов. Больший угол рассеивания означает меньшую точность. Случайное значение рассеивания рассчитывается между минимальным и максимальным значениями.")]
    #endregion
    public float ammoSpreadMax = 0f;

    #region Header AMMO SPAWN DETAILS
    [Space(10)]
    [Header("ДЕТАЛИ СОЗДАНИЯ ПАТРОНОВ")]
    #endregion
    #region Tooltip
    [Tooltip("Минимальное количество патронов, создаваемых за выстрел. Случайное количество патронов создаётся между минимальным и максимальным значениями.")]
    #endregion
    public int ammoSpawnAmountMin = 1;
    #region Tooltip
    [Tooltip("Максимальное количество патронов, создаваемых за выстрел. Случайное количество патронов создаётся между минимальным и максимальным значениями.")]
    #endregion
    public int ammoSpawnAmountMax = 1;
    #region Tooltip
    [Tooltip("Минимальный интервал времени между созданием патронов. Интервал времени в секундах выбирается случайным значением между минимальным и максимальным.")]
    #endregion
    public float ammoSpawnIntervalMin = 0f;
    #region Tooltip
    [Tooltip("Максимальный интервал времени между созданием патронов. Интервал времени в секундах выбирается случайным значением между минимальным и максимальным.")]
    #endregion
    public float ammoSpawnIntervalMax = 0f;

    #region Header AMMO TRAIL DETAILS
    [Space(10)]
    [Header("ДЕТАЛИ СЛЕДА ПАТРОНОВ")]
    #endregion
    #region Tooltip
    [Tooltip("Выберите, если требуется след от патронов, иначе снимите выбор. Если выбрано, то остальные параметры следа патронов должны быть заполнены.")]
    #endregion
    public bool isAmmoTrail = false;
    #region Tooltip
    [Tooltip("Время жизни следа патронов в секундах.")]
    #endregion
    public float ammoTrailTime = 3f;
    #region Tooltip
    [Tooltip("Материал для следа патронов.")]
    #endregion
    public Material ammoTrailMaterial;
    #region Tooltip
    [Tooltip("Начальная ширина следа патронов.")]
    #endregion
    [Range(0f, 1f)] public float ammoTrailStartWidth;
    #region Tooltip
    [Tooltip("Конечная ширина следа патронов.")]
    #endregion
    [Range(0f, 1f)] public float ammoTrailEndWidth;

    #region Validation
#if UNITY_EDITOR
    // Проверка введённых данных в скриптовом объекте
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