using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RoomNodeTypeListSO", menuName = "ScriptableObjects/Dungeon/Room Node Type List")]
public class RoomNodeTypeListSO : ScriptableObject
{

    #region Header ROOM NODE TYPE LIST
    [Space(10)]
    [Header("СПИСОК ТИПОВ УЗЛОВ КОМНАТ")]
    #endregion
    #region Tooltip
    [Tooltip("Этот список должен быть заполнен всеми RoomNodeTypeSO для игры - он используется вместо перечисления")]
    #endregion
    public List<RoomNodeTypeSO> list;
    #region Validation
#if UNITY_EDITOR // вызывается только в редакторе
    private void OnValidate()
    {
        HelperUtilities.ValidateCheckEnumerableValues(this, nameof(list), list);
    }
#endif
    #endregion
}