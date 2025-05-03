using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    #region Tooltip 
        [Tooltip("The player WeaponShootPosition gameobject in the hieracrchy")] 
    #endregion Tooltip 

    [SerializeField] private Transform weaponShootPosition; 

    private Player player; 

    private void Awake() 
    { 
        player = GetComponent<Player>(); 
    } 

    private void Update() 
    { 
        // обработка movement input игрока
        MovementInput(); 
        // обработка weapon input игрока
        WeaponInput(); 
    }

    /// <summary> 
    /// movement input игрока
    /// </summary> 
    private void MovementInput() 
    { 
        player. idleEvent.CallIdleEvent();
    } 
    /// <summary> 
    /// weapon input игрока
    /// </summary> 
    private void WeaponInput() 
    { 
        Vector3 weaponDirection; 
        float weaponAngleDegrees, playerAngleDegrees; 
        AimDirection playerAimDirection; 
        // направить оружие 
        AimWeaponInput(out weaponDirection, out weaponAngleDegrees, out playerAngleDegrees, out playerAimDirection); 
    } 

    private void AimWeaponInput(out Vector3 weaponDirection, out float weaponAngleDegrees, out float playerAngleDegrees, out AimDirection 
            playerAimDirection) 
    {
        Vector3 mouseWorldPosition = HelperUtilities.GetMouseWorldPosition(); 
        // посчитать вектора напрвления курсора от позиции выстрела
        weaponDirection = (mouseWorldPosition - weaponShootPosition.position); 

        // Вычислить вектор направления курсора мыши от позиции трансформации игрока
        Vector3 playerDirection = (mouseWorldPosition - transform.position); 

        // угол от оружия до курсора 
        weaponAngleDegrees = HelperUtilities.GetAngleFromVector(weaponDirection); 

        // угол от игрока до курсора 
        playerAngleDegrees = HelperUtilities.GetAngleFromVector(playerDirection); 

        // установить угол направления 
        playerAimDirection = HelperUtilities.GetAimDirection(playerAngleDegrees); 

        // затригерить ивент направления оружия 
        player.aimWeaponEvent.CallAimWeaponEvent(playerAimDirection, playerAngleDegrees, weaponAngleDegrees, weaponDirection);
    }
}
