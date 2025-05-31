using UnityEngine;

[CreateAssetMenu(fileName = "WeaponDetails_", menuName = "Scriptable Objects/Weapons/Weapon Details")]
public class WeaponDetailsSO : ScriptableObject
{
    #region Header WEAPON BASE DETAILS
    [Space(10)]
    [Header("ОСНОВНЫЕ ДЕТАЛИ ОРУЖИЯ")]
    #endregion Header WEAPON BASE DETAILS
    #region Tooltip
    [Tooltip("Название оружия")]
    #endregion Tooltip
    public string weaponName;
    #region Tooltip
    [Tooltip("Спрайт оружия - у спрайта должна быть включена опция 'generate physics shape'")]
    #endregion Tooltip
    public Sprite weaponSprite;

    #region Header WEAPON CONFIGURATION
    [Space(10)]
    [Header("КОНФИГУРАЦИЯ ОРУЖИЯ")]
    #endregion Header WEAPON CONFIGURATION
    #region Tooltip
    [Tooltip("Позиция стрельбы оружия - смещение от точки привязки спрайта")]
    #endregion Tooltip
    public Vector3 weaponShootPosition;
    #region Tooltip
    [Tooltip("Текущие боеприпасы оружия")]
    #endregion Tooltip
    public AmmoDetailsSO weaponCurrentAmmo;
    #region Tooltip
    [Tooltip("Weapon shoot effect SO - contains particle effecct parameters to be used in conjunction with the weaponShootEffectPrefab ")]
    #endregion Tooltip
    public WeaponShootEffectSO weaponShootEffect;
    #region Tooltip
    [Tooltip("Звуковой эффект стрельбы оружия (Scriptable Object)")]
    #endregion Tooltip
    public SoundEffectSO weaponFiringSoundEffect;
    #region Tooltip
    [Tooltip("Звуковой эффект перезарядки оружия (Scriptable Object)")]
    #endregion Tooltip
    public SoundEffectSO weaponReloadingSoundEffect;

    #region Header WEAPON OPERATING VALUES
    [Space(10)]
    [Header("РАБОЧИЕ ПАРАМЕТРЫ ОРУЖИЯ")]
    #endregion Header WEAPON OPERATING VALUES
    #region Tooltip
    [Tooltip("Выберите, если у оружия бесконечные боеприпасы")]
    #endregion Tooltip
    public bool hasInfiniteAmmo = false;
    #region Tooltip
    [Tooltip("Выберите, если у оружия бесконечная ёмкость магазина")]
    #endregion Tooltip
    public bool hasInfiniteClipCapacity = false;
    #region Tooltip
    [Tooltip("Ёмкость магазина оружия - количество выстрелов до перезарядки")]
    #endregion Tooltip
    public int weaponClipAmmoCapacity = 6;
    #region Tooltip
    [Tooltip("Ёмкость боеприпасов оружия - максимальное количество патронов, которые можно носить для этого оружия")]
    #endregion Tooltip
    public int weaponAmmoCapacity = 100;
    #region Tooltip
    [Tooltip("Скорострельность оружия - 0.2 означает 5 выстрелов в секунду")]
    #endregion Tooltip
    public float weaponFireRate = 0.2f;
    #region Tooltip
    [Tooltip("Время предварительной зарядки оружия - время в секундах, в течение которого нужно удерживать кнопку стрельбы перед выстрелом")]
    #endregion Tooltip
    public float weaponPrechargeTime = 0f;
    #region Tooltip
    [Tooltip("Время перезарядки оружия в секундах")]
    #endregion Tooltip
    public float weaponReloadTime = 0f;


    #region Validation
#if UNITY_EDITOR

    private void OnValidate()
    {
        HelperUtilities.ValidateCheckEmptyString(this, nameof(weaponName), weaponName);
        HelperUtilities.ValidateCheckNullValue(this, nameof(weaponCurrentAmmo), weaponCurrentAmmo);
        HelperUtilities.ValidateCheckPositiveValue(this, nameof(weaponFireRate), weaponFireRate, false);
        HelperUtilities.ValidateCheckPositiveValue(this, nameof(weaponPrechargeTime), weaponPrechargeTime, true);

        if (!hasInfiniteAmmo)
        {
            HelperUtilities.ValidateCheckPositiveValue(this, nameof(weaponAmmoCapacity), weaponAmmoCapacity, false);
        }

        if (!hasInfiniteClipCapacity)
        {
            HelperUtilities.ValidateCheckPositiveValue(this, nameof(weaponClipAmmoCapacity), weaponClipAmmoCapacity, false);
        }
    }

#endif
    #endregion Validation
}