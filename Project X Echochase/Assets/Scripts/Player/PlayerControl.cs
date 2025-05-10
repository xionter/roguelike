using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Player))]
[DisallowMultipleComponent]

public class PlayerControl : MonoBehaviour
{
    #region Tooltip
    [Tooltip("MovementDetailsSO scriptable object containing movement details such as speed")]
    #endregion Tooltip

    [SerializeField] private MovementDetailsSO movementDetails;

    private Player player; 
    private bool leftMouseDownPreviousFrame = false;
    private float moveSpeed;
    private int currentWeaponIndex = 1;
    private Coroutine playerRollCoroutine;
    private WaitForFixedUpdate waitForFixedUpdate;
    private bool isPlayerRolling = false;
    private float playerRollCooldownTimer = 0f;
    private bool isPlayerMovementDisabled = false;


    private void Awake() 
    { 
        player = GetComponent<Player>(); 

        moveSpeed = movementDetails.GetMoveSpeed();
    } 

    private void Start()
    {
        // создаём waitforfixed update для использования coroutine
        waitForFixedUpdate = new WaitForFixedUpdate();

        
        SetStartingWeapon();

        SetPlayerAnimationSpeed();
    }

    private void SetPlayerAnimationSpeed()
    {
        // чтобы скорость анимации соответствовала скорости игрока
        player.animator.speed = moveSpeed / Settings.baseSpeedForPlayerAnimations;
    }

    private void SetStartingWeapon()
    {
        int index = 1;

        foreach (Weapon weapon in player.weaponList)
        {
            if (weapon.weaponDetails == player.playerDetails.startingWeapon)
            {
                SetWeaponByIndex(index);
                break;
            }
            index++;
        }
    }


    private void Update() 
    { 
        if (isPlayerMovementDisabled)
            return;
        // если перекат, то нельзя двигаться
        if (isPlayerRolling) return;

        // обработка movement input игрока
        MovementInput(); 
        // обработка weapon input игрока
        WeaponInput(); 

        PlayerRollCooldownTimer();
    }

    /// <summary> 
    /// movement input игрока
    /// </summary> 
    private void MovementInput() 
    { 
        float horizontalMovement = Input.GetAxisRaw("Horizontal");
        float verticalMovement = Input.GetAxisRaw("Vertical");
        bool rightMouseButtonDown = Input.GetMouseButtonDown(1);

        // Вектор напрвления исходя из инпута
        Vector2 direction = new Vector2(horizontalMovement, verticalMovement);

        // Для диагонального напрвления (пифагорейское приближение)
        if (horizontalMovement != 0f && verticalMovement != 0f)
        {
            direction *= 0.7f;
        }

        // Если есть движение либо движение, либо перекат
        if (direction != Vector2.zero)
        {
            player.movementByVelocityEvent.CallMovementByVelocityEvent(direction, moveSpeed);

            if (!rightMouseButtonDown)
            {
                // тригерим мувмент ивент
                player.movementByVelocityEvent.CallMovementByVelocityEvent(direction, moveSpeed);
            }
            // иначе игрок делает перекат, если не в перезарядке
            else if (playerRollCooldownTimer <= 0f)
            {
                PlayerRoll((Vector3)direction);
            }

        }
        // иначе тригерим idle
        else
        {
            player.idleEvent.CallIdleEvent();
        }
    } 

    private void PlayerRoll(Vector3 direction)
    {
        playerRollCoroutine = StartCoroutine(PlayerRollRoutine(direction));
    }

    /// <summary>
    /// Player roll coroutine
    /// </summary>
    private IEnumerator PlayerRollRoutine(Vector3 direction)
    {
        //minDistance, чтобы решить, когда выходить из цикла
        float minDistance = 0.2f;

        isPlayerRolling = true;

        Vector3 targetPosition = player.transform.position + (Vector3)direction * movementDetails.rollDistance;

        while (Vector3.Distance(player.transform.position, targetPosition) > minDistance)
        {
            player.movementToPositionEvent.CallMovementToPositionEvent(targetPosition, player.transform.position, movementDetails.rollSpeed, direction, isPlayerRolling);

            // yield and ждём fixed update
            yield return waitForFixedUpdate;

        }

        isPlayerRolling = false;

        // устанавливаем перезарядку для переката
        playerRollCooldownTimer = movementDetails.rollCooldownTime;

        player.transform.position = targetPosition;

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        StopPlayerRollRoutine();
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        StopPlayerRollRoutine();
    }

    private void StopPlayerRollRoutine()
    {
        if (playerRollCoroutine != null)
        {
            StopCoroutine(playerRollCoroutine);

            isPlayerRolling = false;
        }
    }
    private void PlayerRollCooldownTimer()
    {
        if (playerRollCooldownTimer >= 0f)
        {
            playerRollCooldownTimer -= Time.deltaTime;
        }
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

        // выстрелить из оружия
        FireWeaponInput(weaponDirection, weaponAngleDegrees, playerAngleDegrees, playerAimDirection);

        // поменять оружие
        SwitchWeaponInput();

        // перезарядить оружие
        ReloadWeaponInput();

    } 

    private void AimWeaponInput(out Vector3 weaponDirection, out float weaponAngleDegrees, out float playerAngleDegrees, out AimDirection 
            playerAimDirection) 
    {
        Vector3 mouseWorldPosition = HelperUtilities.GetMouseWorldPosition(); 
        // посчитать вектора напрвления курсора от позиции выстрела
        weaponDirection = (mouseWorldPosition - player.activeWeapon.GetShootPosition()); 

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

    private void FireWeaponInput(Vector3 weaponDirection, float weaponAngleDegrees, float playerAngleDegrees, AimDirection playerAimDirection)
    {
        // выстрелить из оружия на лкм
        if (Input.GetMouseButton(0))
        {
            // тригер fireWeaponEvent
            player.fireWeaponEvent.CallFireWeaponEvent(true, leftMouseDownPreviousFrame, playerAimDirection, playerAngleDegrees, weaponAngleDegrees, weaponDirection);
            leftMouseDownPreviousFrame = true;
        }
        else
        {
            leftMouseDownPreviousFrame = false;
        }
    }

    private void SwitchWeaponInput()
{
    // переключение по колёсику мыши
    float scrollDelta = Input.mouseScrollDelta.y;
    if (scrollDelta < 0f) PreviousWeapon();
    if (scrollDelta > 0f) NextWeapon();

    // переключение по цифрам (1-9)
    for (var i = 0; i < 9; ++i)
    {
        if (Input.GetKeyDown(KeyCode.Alpha1 + i))
        {
            SetWeaponByIndex(i + 1);
        }
    }

    // особые случаи
    if (Input.GetKeyDown(KeyCode.Alpha0)) SetWeaponByIndex(10);
    if (Input.GetKeyDown(KeyCode.Minus)) SetCurrentWeaponToFirstInTheList();
}


    private void SetWeaponByIndex(int weaponIndex)
    {
        if (weaponIndex - 1 < player.weaponList.Count)
        {
            currentWeaponIndex = weaponIndex;
            player.setActiveWeaponEvent.CallSetActiveWeaponEvent(player.weaponList[weaponIndex - 1]);
        }
    }

    private void NextWeapon()
    {
        currentWeaponIndex++;
        if (currentWeaponIndex > player.weaponList.Count)
            currentWeaponIndex = 1;

        SetWeaponByIndex(currentWeaponIndex);
    }

    private void PreviousWeapon()
    {
        currentWeaponIndex--;
        if (currentWeaponIndex < 1)
            currentWeaponIndex = player.weaponList.Count;
    
        SetWeaponByIndex(currentWeaponIndex);
    }


    private void ReloadWeaponInput()
    {
        Weapon currentWeapon = player.activeWeapon.GetCurrentWeapon();

        if (currentWeapon.isWeaponReloading) return;

        if (currentWeapon.weaponRemainingAmmo < currentWeapon.weaponDetails.weaponClipAmmoCapacity && !currentWeapon.weaponDetails.hasInfiniteAmmo) return;

        if (currentWeapon.weaponClipRemainingAmmo == currentWeapon.weaponDetails.weaponClipAmmoCapacity) return;

        if (Input.GetKeyDown(KeyCode.R))
        {
            player.reloadWeaponEvent.CallReloadWeaponEvent(player.activeWeapon.GetCurrentWeapon(), 0);
        }

    }

    private void SetCurrentWeaponToFirstInTheList()
    {
        List<Weapon> tempWeaponList = new List<Weapon>();

        Weapon currentWeapon = player.weaponList[currentWeaponIndex - 1];
        currentWeapon.weaponListPosition = 1;
        tempWeaponList.Add(currentWeapon);

        int index = 2;

        foreach (Weapon weapon in player.weaponList)
        {
            if (weapon == currentWeapon) continue;

            tempWeaponList.Add(weapon);
            weapon.weaponListPosition = index;
            index++;
        }

        player.weaponList = tempWeaponList;

        currentWeaponIndex = 1;

        SetWeaponByIndex(currentWeaponIndex);
    }


    #region Validation

#if UNITY_EDITOR

    private void OnValidate()
    {
        HelperUtilities.ValidateCheckNullValue(this, nameof(movementDetails), movementDetails);
    }

#endif

    #endregion Validation
}
