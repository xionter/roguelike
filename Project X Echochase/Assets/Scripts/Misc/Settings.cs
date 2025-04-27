using UnityEngine;

public static class Settings 
{
    #region Room settings

    public const int maxChildCorridors = 3;

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
    #endregion
}
