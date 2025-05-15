using UnityEngine;

[System.Serializable]
public class RoomEnemySpawnParameters
{
    #region Tooltip
    [Tooltip("Определяет уровень подземелья для этой комнаты в отношении того, сколько врагов в общей сложности должно быть заспавнено")]
    #endregion Tooltip
    public DungeonLevelSO dungeonLevel;
    #region Tooltip
    [Tooltip("Минимальное количество врагов, которые должны быть заспавнены в этой комнате для этого уровня подземелья. Фактическое количество будет случайным значением между минимальным и максимальным значениями.")]
    #endregion Tooltip
    public int minTotalEnemiesToSpawn;
    #region Tooltip
    [Tooltip("Максимальное количество врагов, которые должны быть заспавнены в этой комнате для этого уровня подземелья. Фактическое количество будет случайным значением между минимальным и максимальным значениями.")]
    #endregion Tooltip
    public int maxTotalEnemiesToSpawn;
    #region Tooltip
    [Tooltip("Минимальное количество одновременно заспавненных врагов в этой комнате для этого уровня подземелья. Фактическое количество будет случайным значением между минимальным и максимальным значениями.")]
    #endregion Tooltip
    public int minConcurrentEnemies;
    #region Tooltip
    [Tooltip("Максимальное количество одновременно заспавненных врагов в этой комнате для этого уровня подземелья. Фактическое количество будет случайным значением между минимальным и максимальным значениями.")]
    #endregion Tooltip
    public int maxConcurrentEnemies;
    #region Tooltip
    [Tooltip("Минимальный интервал спавна врагов в секундах для этой комнаты для этого уровня подземелья. Фактическое значение будет случайным значением между минимальным и максимальным значениями.")]
    #endregion Tooltip
    public int minSpawnInterval;
    #region Tooltip
    [Tooltip("Максимальный интервал спавна врагов в секундах для этой комнаты для этого уровня подземелья. Фактическое значение будет случайным значением между минимальным и максимальным значениями.")]
    #endregion Tooltip
    public int maxSpawnInterval;
}