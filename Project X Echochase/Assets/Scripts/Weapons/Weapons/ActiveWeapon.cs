using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SetActiveWeaponEvent))]
[DisallowMultipleComponent]
public class ActiveWeapon : MonoBehaviour
{
    #region Tooltip
    [Tooltip("Заполните SpriteRenderer дочернего объекта Weapon")]
    #endregion
    [SerializeField] private SpriteRenderer weaponSpriteRenderer;
    #region Tooltip
    [Tooltip("Заполните PolygonCollider2D дочернего объекта Weapon")]
    #endregion
    [SerializeField] private PolygonCollider2D weaponPolygonCollider2D;
    #region Tooltip
    [Tooltip("Заполните Transform объекта WeaponShootPosition")]
    #endregion
    [SerializeField] private Transform weaponShootPositionTransform;
    #region Tooltip
    [Tooltip("Заполните Transform объекта WeaponEffectPosition")]
    #endregion
    [SerializeField] private Transform weaponEffectPositionTransform;

    private SetActiveWeaponEvent setWeaponEvent;
    private Weapon currentWeapon;


    private void Awake()
    {
        // Загрузка компонентов
        setWeaponEvent = GetComponent<SetActiveWeaponEvent>();
    }

    private void OnEnable()
    {
        setWeaponEvent.OnSetActiveWeapon += SetWeaponEvent_OnSetActiveWeapon;
    }

    private void OnDisable()
    {
        setWeaponEvent.OnSetActiveWeapon -= SetWeaponEvent_OnSetActiveWeapon;
    }

    private void SetWeaponEvent_OnSetActiveWeapon(SetActiveWeaponEvent setActiveWeaponEvent, SetActiveWeaponEventArgs setActiveWeaponEventArgs)
    {
        SetWeapon(setActiveWeaponEventArgs.weapon);
    }

    private void SetWeapon(Weapon weapon)
    {
        currentWeapon = weapon;

        // Установить текущий спрайт оружия
        weaponSpriteRenderer.sprite = currentWeapon.weaponDetails.weaponSprite;

        // Если у оружия есть полигональный коллайдер и спрайт, то установить физическую форму спрайта
        if (weaponPolygonCollider2D != null && weaponSpriteRenderer.sprite != null)
        {
            // Получить физическую форму спрайта - возвращает точки физической формы спрайта в виде списка Vector2
            List<Vector2> spritePhysicsShapePointsList = new List<Vector2>();
            weaponSpriteRenderer.sprite.GetPhysicsShape(0, spritePhysicsShapePointsList);

            // Установить полигональный коллайдер на оружие, чтобы он соответствовал физической форме спрайта - установить точки коллайдера в точки физической формы спрайта
            weaponPolygonCollider2D.points = spritePhysicsShapePointsList.ToArray();

        }

        // Установить позицию стрельбы оружия
        weaponShootPositionTransform.localPosition = currentWeapon.weaponDetails.weaponShootPosition;
    }

    public AmmoDetailsSO GetCurrentAmmo()
    {
        return currentWeapon.weaponDetails.weaponCurrentAmmo;
    }

    public Weapon GetCurrentWeapon()
    {
        return currentWeapon;
    }

    public Vector3 GetShootPosition()
    {
        return weaponShootPositionTransform.position;
    }

    public Vector3 GetShootEffectPosition()
    {
        return weaponEffectPositionTransform.position;
    }

    public void RemoveCurrentWeapon()
    {
        currentWeapon = null;
    }

    #region Validation
#if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtilities.ValidateCheckNullValue(this, nameof(weaponSpriteRenderer), weaponSpriteRenderer);
        HelperUtilities.ValidateCheckNullValue(this, nameof(weaponPolygonCollider2D), weaponPolygonCollider2D);
        HelperUtilities.ValidateCheckNullValue(this, nameof(weaponShootPositionTransform), weaponShootPositionTransform);
        HelperUtilities.ValidateCheckNullValue(this, nameof(weaponEffectPositionTransform), weaponEffectPositionTransform);
    }
#endif
    #endregion

}