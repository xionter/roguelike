using UnityEngine;

[CreateAssetMenu(fileName = "EnemyDetails_", menuName = "Scriptable Objects/Enemy/EnemyDetails")]
public class EnemyDetailsSO : ScriptableObject
{
    #region Header ОСНОВНЫЕ ПАРАМЕТРЫ ВРАГА
    [Space(10)]
    [Header("BASE ENEMY DETAILS")]
    #endregion

    #region Tooltip
    [Tooltip("Название врага")]
    #endregion
    public string enemyName;

    #region Tooltip
    [Tooltip("Префаб врага")]
    #endregion
    public GameObject enemyPrefab;

    #region Tooltip

    [Tooltip("Расстояние до игрока для начала атаки")]

    #endregion

    public float chaseDistance = 50f;
    
    #region Header НАСТРОЙКИ ОРУЖИЯ ВРАГА
    [Space(10)]
    [Header("ENEMY WEAPON SETTINGS")]
    #endregion
    #region Tooltip
    [Tooltip("Оружие врага (если отсутствует - оставить None)")]
    #endregion
    public WeaponDetailsSO enemyWeapon;
    #region Tooltip
    [Tooltip("Минимальная задержка (в секундах) между очередями выстрелов. Должна быть > 0. Выбирается случайное значение между min и max")]
    #endregion
    public float firingIntervalMin = 0.1f;
    #region Tooltip
    [Tooltip("Максимальная задержка (в секундах) между очередями выстрелов. Выбирается случайное значение между min и max")]
    #endregion
    public float firingIntervalMax = 1f;
    #region Tooltip
    [Tooltip("Минимальная длительность очереди выстрелов (в секундах). Должна быть > 0. Выбирается случайное значение между min и max")]
    #endregion
    public float firingDurationMin = 1f;
    #region Tooltip
    [Tooltip("Максимальная длительность очереди выстрелов (в секундах). Выбирается случайное значение между min и max")]
    #endregion
    public float firingDurationMax = 2f;
    #region Tooltip
    [Tooltip("Если отмечено, враг будет стрелять только при прямой видимости игрока")]
    #endregion
    public bool firingLineOfSightRequired;

    #region Header ЗДОРОВЬЕ ВРАГА
    [Space(10)]
    [Header("ENEMY HEALTH")]
    #endregion
    #region Tooltip
    [Tooltip("Здоровье врага для каждого уровня")]
    #endregion
    public EnemyHealthDetails[] enemyHealthDetailsArray;
    #region Tooltip
    [Tooltip("Если отмечено, после получения урона враг получает временную неуязвимость")]
    #endregion
    public bool isImmuneAfterHit = false;
    #region Tooltip
    [Tooltip("Длительность неуязвимости (в секундах) после получения урона")]
    #endregion
    public float hitImmunityTime;
    #region Tooltip
    [Tooltip("Отображать ли полосу здоровья у врага")]
    #endregion
    public bool isHealthBarDisplayed = true;

    #region Validation
#if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtilities.ValidateCheckEmptyString(this, nameof(enemyName), enemyName);
        HelperUtilities.ValidateCheckNullValue(this, nameof(enemyPrefab), enemyPrefab);
        HelperUtilities.ValidateCheckPositiveValue(this, nameof(chaseDistance), chaseDistance, false);
        HelperUtilities.ValidateCheckPositiveRange(this, nameof(firingIntervalMin), firingIntervalMin, nameof(firingIntervalMax), firingIntervalMax, false);
        HelperUtilities.ValidateCheckPositiveRange(this, nameof(firingDurationMin), firingDurationMin, nameof(firingDurationMax), firingDurationMax, false);
        HelperUtilities.ValidateCheckEnumerableValues(this, nameof(enemyHealthDetailsArray), enemyHealthDetailsArray);
        if (isImmuneAfterHit)
        {
            HelperUtilities.ValidateCheckPositiveValue(this, nameof(hitImmunityTime), hitImmunityTime, false);
        }
    }

#endif
    #endregion

}
