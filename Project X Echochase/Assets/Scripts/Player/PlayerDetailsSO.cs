using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "PlayerDetails_", menuName = "Scriptable Objects/Player/Player Details")]
public class PlayerDetailsSO : ScriptableObject
{
    #region Header PLAYER BASE DETAILS 
    [Space(10)] 
    [Header("ОСНОВНЫЕ ДЕТАЛИ ИГРОКА")] 
    #endregion 
    #region Tooltip 
    [Tooltip("Имя персонажа игрока.")] 
    #endregion 
    public string playerCharacterName;

    #region Tooltip 
    [Tooltip("Префаб игрового объекта для игрока")] 
    #endregion 
    public GameObject playerPrefab;

    #region Tooltip 
    [Tooltip("Runtime Animator Controller для игрока")] 
    #endregion 
    public RuntimeAnimatorController runtimeAnimatorController;


    #region Header HEALTH
    [Space(10)]
    [Header("ЗДОРОВЬЕ")]
    #endregion
    #region Tooltip
    [Tooltip("Начальное количество здоровья игрока")]
    #endregion
    public int playerHealthAmount;
    #region Tooltip
    [Tooltip("Выберите, есть ли период иммунитета сразу после получения урона. Если да, укажите время иммунитета в секундах в другом поле")]
    #endregion
    public bool isImmuneAfterHit = false;
    #region Tooltip
    [Tooltip("Время иммунитета в секундах после получения урона")]
    #endregion
    public float hitImmunityTime;

    #region Header WEAPON
    [Space(10)]
    [Header("ОРУЖИЕ")]
    #endregion
    #region Tooltip
    [Tooltip("Начальное оружие игрока")]
    #endregion
    public WeaponDetailsSO startingWeapon;
    #region Tooltip
    [Tooltip("Заполните список начального оружия")]
    #endregion
    public List<WeaponDetailsSO> startingWeaponList;


    #region Header OTHER 
    [Space(10)] 
    [Header("ПРОЧЕЕ")] 
    #endregion 
    #region Tooltip 
    [Tooltip("Иконка игрока для использования на миникарте")] 
    #endregion 
    public Sprite playerMiniMapIcon; 
    #region Tooltip 
    [Tooltip("Спрайт руки игрока")] 
    #endregion 
    public Sprite playerHandSprite;



    #region Validation 
#if UNITY_EDITOR 
    private void OnValidate() 
    { 
        HelperUtilities.ValidateCheckEmptyString(this, nameof(playerCharacterName), playerCharacterName);
        HelperUtilities.ValidateCheckNullValue(this, nameof(playerPrefab), playerPrefab);
        HelperUtilities.ValidateCheckPositiveValue(this, nameof(playerHealthAmount), playerHealthAmount, false);
        HelperUtilities.ValidateCheckNullValue(this, nameof(startingWeapon), startingWeapon);
        //HelperUtilities.ValidateCheckNullValue(this, nameof(playerMiniMapIcon), playerMiniMapIcon);
        HelperUtilities.ValidateCheckNullValue(this, nameof(playerHandSprite), playerHandSprite);
        HelperUtilities.ValidateCheckNullValue(this, nameof(runtimeAnimatorController), runtimeAnimatorController);
        HelperUtilities.ValidateCheckEnumerableValues(this, nameof(startingWeaponList), startingWeaponList);

        if (isImmuneAfterHit)
        {
            HelperUtilities.ValidateCheckPositiveValue(this, nameof(hitImmunityTime), hitImmunityTime, false);
        }
    } 
#endif 
    #endregion
}