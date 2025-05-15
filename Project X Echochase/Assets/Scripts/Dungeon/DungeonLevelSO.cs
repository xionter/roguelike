using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "DungeonLevel_", menuName = "Scriptable Objects/Dungeon/Dungeon Level")]
public class DungeonLevelSO : ScriptableObject
{
    #region Header ОСНОВНЫЕ ДЕТАЛИ УРОВНЯ

    [Space(10)]
    [Header("ОСНОВНЫЕ ДЕТАЛИ УРОВНЯ")]

    #endregion Header ОСНОВНЫЕ ДЕТАЛИ УРОВНЯ

    #region Tooltip

    [Tooltip("Имя уровня")]

    #endregion Tooltip

    public string levelName;

    #region Header ШАБЛОНЫ КОМНАТ ДЛЯ УРОВНЯ

    [Space(10)]
    [Header("ШАБЛОНЫ КОМНАТ ДЛЯ УРОВНЯ")]

    #endregion Header ШАБЛОНЫ КОМНАТ ДЛЯ УРОВНЯ

    #region Tooltip

    [Tooltip("Заполните список шаблонами комнат, которые вы хотите включить в уровень. Убедитесь, что шаблоны комнат включены для всех типов узлов комнат, указанных в графах узлов комнат для уровня.")]

    #endregion Tooltip

    public List<RoomTemplateSO> roomTemplateList;
    
    
    #region Header ГРАФЫ УЗЛОВ КОМНАТ ДЛЯ УРОВНЯ

    [Space(10)]
    [Header("ГРАФЫ УЗЛОВ КОМНАТ ДЛЯ УРОВНЯ")]

    #endregion Header ГРАФЫ УЗЛОВ КОМНАТ ДЛЯ УРОВНЯ

    #region Tooltip

    [Tooltip("Заполните этот список графами узлов комнат, которые должны быть случайным образом выбраны для уровня.")]

    #endregion Tooltip

    public RoomNodeGraphSO roomNodeGraph;
    
    #region Validation

#if UNITY_EDITOR

    // Проверка введённых данных ScriptableObject
    private void OnValidate()
    {
        HelperUtilities.ValidateCheckEmptyString(this, nameof(levelName), levelName);
        if (HelperUtilities.ValidateCheckEnumerableValues(this, nameof(roomTemplateList), roomTemplateList))
            return;
        

        
        
        bool isEWCorridor = false;
        bool isNSCorridor = false;
        bool isEntrance = false;
        

        foreach (RoomTemplateSO roomTemplateSO in roomTemplateList)
        {
            if (roomTemplateSO == null)
                return;
        
            if (roomTemplateSO.roomNodeType.isCorridorEW)
                isEWCorridor = true;
        
            if (roomTemplateSO.roomNodeType.isCorridorNS)
                isNSCorridor = true;
        
            if (roomTemplateSO.roomNodeType.isEntrance)
                isEntrance = true;
        }

        if (isEWCorridor == false)
        {
            Debug.Log("В " + this.name.ToString() + " : Не указан тип комнаты для коридора В/З");
        }

        if (isNSCorridor == false)
        {
            Debug.Log("В " + this.name.ToString() + " : Не указан тип комнаты для коридора С/Ю");
        }

        if (isEntrance == false)
        {
            Debug.Log("В " + this.name.ToString() + " : Не указан тип комнаты для входа");
        }

        
        
        
        foreach (RoomNodeSO roomNodeSO in roomNodeGraph.roomNodeList)
        {
            if (roomNodeSO == null)
                continue;


            if (roomNodeSO.roomNodeType.isEntrance || roomNodeSO.roomNodeType.isCorridorEW || roomNodeSO.roomNodeType.isCorridorNS || roomNodeSO.roomNodeType.isCorridor || roomNodeSO.roomNodeType.isNone)
                continue;

            bool isRoomNodeTypeFound = false;

            foreach (RoomTemplateSO roomTemplateSO in roomTemplateList)
            {
                 if (roomTemplateSO == null)
                     continue;

                 if (roomTemplateSO.roomNodeType == roomNodeSO.roomNodeType)
                 {
                     isRoomNodeTypeFound = true;
                     break;
                 }

            }

            if (!isRoomNodeTypeFound)
                Debug.Log("В " + this.name.ToString() + " : Не найден шаблон комнаты " + roomNodeSO.roomNodeType.name.ToString() + " для графа узлов " + roomNodeGraph.name.ToString());


        }
    }

#endif

    #endregion Validation
}