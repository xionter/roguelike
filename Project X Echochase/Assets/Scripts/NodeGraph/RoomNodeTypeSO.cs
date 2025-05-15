using Mono.Cecil;
using UnityEngine;

[CreateAssetMenu(fileName = "RoomNodeType_", menuName = "ScriptableObjects/Dungeon/Room Node Type")]
public class RoomNodeTypeSO : ScriptableObject
{
    public string roomNodeTypeName;

    #region Header
    [Header("Отметьте только те типы узлов комнат, которые должны быть видны в редакторе")]
    #endregion Header
    public bool displayInNodeGraphEditor = true;
    #region Header
    [Header("Один тип должен быть коридором")]
    #endregion Header
    public bool isCorridor;
    #region Header
    [Header("Один тип должен быть коридором NS")]
    #endregion Header
    public bool isCorridorNS;
    #region Header
    [Header("Один тип должен быть коридором EW")]
    #endregion Header
    public bool isCorridorEW;
    #region Header
    [Header("Один тип должен быть входом")]
    #endregion Header
    public bool isEntrance;
    #region Header
    [Header("Один тип должен быть комнатой босса")]
    #endregion Header
    public bool isBossRoom;
    #region Header
    [Header("Один тип должен быть None (не назначен)")]
    #endregion Header
    public bool isNone;

    #region Validation
#if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtilities.ValidateCheckEmptyString(this, nameof(roomNodeTypeName), roomNodeTypeName);
    }
#endif
    #endregion
}