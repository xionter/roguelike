using UnityEngine;
[System.Serializable]

public class DoorWay
{
    public Vector2Int position;
    public Orientation orientation;
    public GameObject doorPrefab;

    #region Header
    [Header("Верхняя левая позиция для копирования")]
    #endregion
    public Vector2Int doorwayStartCopyPosition;
    #region Header
    [Header("Ширина плит в дверном проходе")]
    #endregion
    public int doorwayCopyTileWidth;
    #region Header
    [Header("Высота плит в дверном проходе")]
    #endregion
    public int doorwayCopyTileHeight;
    [HideInInspector]
    public bool isConnected = false;
    [HideInInspector]
    public bool isUnavailable = false;
}
