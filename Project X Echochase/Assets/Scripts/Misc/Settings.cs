using UnityEngine;

public static class Settings 
{
    #region UNITS
    public const float pixelsPerUnit = 16f;
    public const float tileSizePixels = 16f;
    #endregion

    #region Dungeon BUILD SETTINGS

    public const int maxDungeonRebuildAttemptsForRoomGraph = 1000;
    public const int maxDungeonBuildAttempts = 10;

    #endregion
    
    #region Room settings

    public const int maxChildCorridors = 3;
    public const float fadeInTime = 0.5f;
    public const float doorUnlockDelay = 1f;

    #endregion

    #region ANIMATOR PARAMETERS 

    public static int aimUp = Animator.StringToHash("aimUp"); 
    public static int aimDown = Animator.StringToHash("aimDown"); 
    public static int aimUpRight = Animator.StringToHash("aimUpRight"); 
    public static int aimUpLeft = Animator.StringToHash("aimUpLeft"); 
    public static int aimRight = Animator.StringToHash("aimRight"); 
    public static int aimLeft = Animator.StringToHash("aimLeft"); 
    public static int isIdle = Animator.StringToHash("isIdle"); 
    public static int isMoving = Animator.StringToHash("isMoving"); 
    public static int rollUp = Animator.StringToHash("rollUp");
    public static int rollRight = Animator.StringToHash("rollRight");
    public static int rollLeft = Animator.StringToHash("rollLeft");
    public static int rollDown = Animator.StringToHash("rollDown");
    public static float baseSpeedForPlayerAnimations = 8f;

    //door
    public static int open = Animator.StringToHash("open");
    #endregion

    #region FIRING CONTROL
    public const float useAimAngleDistance = 3.5f; // если расстояние до цели меньше этого параметра, то
    // считаем угол от игрока. Если больше, то угол от оружия
    #endregion

    #region GAMEOBJECT TAGS
    public const string playerTag = "Player";
    public const string playerWeapon = "playerWeapon";
    #endregion

    #region UI PARAMETERS
    public const float uiHeartSpacing = 16f;
    public const float uiAmmoIconSpacing = 4f;
    #endregion

    #region ENEMY PARAMETERS
    public const int defaultEnemyHealth = 20;
    #endregion
    
    #region CONTACT DAMAGE PARAMETERS
    public const float contactDamageCollisionResetDelay = 0.5f;
    #endregion


    // анимации для врагов
    public static float baseSpeedForEnemyAnimations = 3f;

}
