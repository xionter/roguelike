using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MovementDetails_", menuName = "ScriptableObjects/Movement/MovementDetails")]
public class MovementDetailsSO : ScriptableObject
{
    #region Header MOVEMENT DETAILS
    [Space(10)]
    [Header("ДЕТАЛИ ДВИЖЕНИЯ")]
    #endregion Header
    #region Tooltip
    [Tooltip("Минимальная скорость движения. Метод GetMoveSpeed вычисляет случайное значение между минимальной и максимальной")]
    #endregion Tooltip
    public float minMoveSpeed = 8f;
    #region Tooltip
    [Tooltip("Максимальная скорость движения. Метод GetMoveSpeed вычисляет случайное значение между минимальной и максимальной")]
    #endregion Tooltip
    public float maxMoveSpeed = 8f;
    #region Tooltip
    [Tooltip("Если есть движение перекатом - это скорость переката")]
    #endregion
    public float rollSpeed; // для игрока
    #region Tooltip
    [Tooltip("Если есть движение перекатом - это расстояние переката")]
    #endregion
    public float rollDistance; // для игрока
    #region Tooltip
    [Tooltip("Если есть движение перекатом - это время перезарядки в секундах между действиями переката")]
    #endregion
    public float rollCooldownTime; // для игрока

    /// <summary>
    /// Случайная скорость на отрезке от минимальной до максимальной
    /// </summary>
    public float GetMoveSpeed()
    {
        if (minMoveSpeed == maxMoveSpeed)
        {
            return minMoveSpeed;
        }
        else
        {
            return Random.Range(minMoveSpeed, maxMoveSpeed);
        }
    }

    #region Validation
#if UNITY_EDITOR

    private void OnValidate()
    {
        HelperUtilities.ValidateCheckPositiveRange(this, nameof(minMoveSpeed), minMoveSpeed, nameof(maxMoveSpeed), maxMoveSpeed, false);

        if (rollDistance != 0f || rollSpeed != 0 || rollCooldownTime != 0)
        {
            HelperUtilities.ValidateCheckPositiveValue(this, nameof(rollDistance), rollDistance, false);
            HelperUtilities.ValidateCheckPositiveValue(this, nameof(rollSpeed), rollSpeed, false);
            HelperUtilities.ValidateCheckPositiveValue(this, nameof(rollCooldownTime), rollCooldownTime, false);
        }

    }

#endif
    #endregion Validation
}