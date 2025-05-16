using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.Audio;

public class GameResources : MonoBehaviour
{
    private static GameResources instance;

    public static GameResources Instance
    {
        get
        {
            if (instance == null)
            {
                instance = Resources.Load<GameResources>("GameResources");
            }
            return instance;
        }
    }

    #region Header DUNGEON
    [Space(10)]
    [Header("ПОДЗЕМЕЛЬЕ")]
    #endregion
    #region Tooltip
    [Tooltip("Заполните списком типов узлов комнат подземелья (RoomNodeTypeListSO)")]
    #endregion

    public RoomNodeTypeListSO roomNodeTypeList;

    #region PLAYER SELECTION
    [Space(10)]
    [Header("ВЫБОР ИГРОКА")]
    #endregion PLAYER SELECTION
    #region Tooltip
    [Tooltip("Префаб выбора игрока")]
    #endregion Tooltip
    public GameObject playerSelectionPrefab;

    #region Header PLAYER
    [Space(10)]
    [Header("ИГРОК")]
    #endregion Header PLAYER
    #region Tooltip
    [Tooltip("Список деталей игрока - заполните список ScriptableObject с деталями игрока")]
    #endregion Tooltip
    public List<PlayerDetailsSO> playerDetailsList;
    #region Tooltip
    [Tooltip("Текущий ScriptableObject игрока - используется для передачи данных между сценами")]
    #endregion Tooltip
    public CurrentPlayerSO currentPlayer;

    #region Header SOUNDS
    [Space(10)]
    [Header("ЗВУКИ")]
    #endregion
    #region Tooltip
    [Tooltip("Заполните группу главного микшера звуков")]
    #endregion
    public AudioMixerGroup soundsMasterMixerGroup;
    #region Tooltip
    [Tooltip("Звуковой эффект открытия и закрытия двери")]
    #endregion Tooltip
    public SoundEffectSO doorOpenCloseSoundEffect;

    #region Header MATERIALS
    [Space(10)]
    [Header("МАТЕРИАЛЫ")]
    #endregion
    #region Tooltip
    [Tooltip("Материал с затемнением")]
    #endregion
    public Material dimmedMaterial;

    #region Tooltip
    [Tooltip("Материал Sprite-Lit-Default")]
    #endregion
    public Material litMaterial;

    #region Tooltip
    [Tooltip("Заполните шейдером Variable Lit")]
    #endregion
    public Shader variableLitShader;
    #region Tooltip
    [Tooltip("Заполните шейдером Materialize")]
    #endregion
    public Shader materializeShader;

    #region Header SPECIAL TILEMAP TILES
    [Space(10)]
    [Header("СПЕЦИАЛЬНЫЕ ТАЙЛЫ TILEMAP")]
    #endregion Header SPECIAL TILEMAP TILES
    #region Tooltip
    [Tooltip("Тайлы столкновений, по которым враги не могут ходить")]
    #endregion Tooltip
    public TileBase[] enemyUnwalkableCollisionTilesArray;
    #region Tooltip
    [Tooltip("Предпочтительный тайл пути для навигации врагов")]
    #endregion Tooltip
    public TileBase preferredEnemyPathTile;

    #region Header UI
    [Space(10)]
    [Header("ИНТЕРФЕЙС")]
    #endregion
    #region Tooltip
    [Tooltip("Заполните префабом изображения сердца")]
    #endregion
    public GameObject heartPrefab;
    #region Tooltip
    [Tooltip("Заполните префабом иконки патронов")]
    #endregion Tooltip
    public GameObject ammoIconPrefab;
    #region Tooltip
    [Tooltip("Префаб очков")]
    #endregion
    public GameObject scorePrefab;

    #region Header CHESTS
    [Space(10)]
    [Header("СУНДУКИ")]
    #endregion
    #region Tooltip
    [Tooltip("Префаб предмета из сундука")]
    #endregion
    public GameObject chestItemPrefab;
    #region Tooltip
    [Tooltip("Заполните иконкой сердца")]
    #endregion
    public Sprite heartIcon;
    #region Tooltip
    [Tooltip("Заполните иконкой пули")]
    #endregion
    public Sprite bulletIcon;

    #region Header MINIMAP
    [Space(10)]
    [Header("МИНИКАРТА")]
    #endregion
    #region Tooltip
    [Tooltip("Префаб черепа для миникарты")]
    #endregion
    public GameObject minimapSkullPrefab;

    #region Validation
#if UNITY_EDITOR
    // Проверка введённых данных ScriptableObject
    private void OnValidate()
    {
        HelperUtilities.ValidateCheckNullValue(this, nameof(roomNodeTypeList), roomNodeTypeList);
        HelperUtilities.ValidateCheckNullValue(this, nameof(playerSelectionPrefab), playerSelectionPrefab);
        HelperUtilities.ValidateCheckEnumerableValues(this, nameof(playerDetailsList), playerDetailsList);
        HelperUtilities.ValidateCheckNullValue(this, nameof(currentPlayer), currentPlayer);
        HelperUtilities.ValidateCheckNullValue(this, nameof(soundsMasterMixerGroup), soundsMasterMixerGroup);
        HelperUtilities.ValidateCheckNullValue(this, nameof(doorOpenCloseSoundEffect), doorOpenCloseSoundEffect);
        HelperUtilities.ValidateCheckNullValue(this, nameof(litMaterial), litMaterial);
        HelperUtilities.ValidateCheckNullValue(this, nameof(dimmedMaterial), dimmedMaterial);
        HelperUtilities.ValidateCheckNullValue(this, nameof(variableLitShader), variableLitShader);
        HelperUtilities.ValidateCheckNullValue(this, nameof(ammoIconPrefab), ammoIconPrefab);
    }

#endif
    #endregion
}