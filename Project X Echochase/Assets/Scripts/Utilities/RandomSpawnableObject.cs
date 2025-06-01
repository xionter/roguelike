using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomSpawnableObject<T>
{
    private struct chanceBoundaries
    {
        public T spawnableObject;
        public int lowBoundaryValue;
        public int highBoundaryValue;
    }

    private int ratioValueTotal = 0;
    private List<chanceBoundaries> chanceBoundariesList = new List<chanceBoundaries>();
    private List<SpawnableObjectsByLevel<T>> spawnableObjectsByLevelList;

    public RandomSpawnableObject(List<SpawnableObjectsByLevel<T>> spawnableObjectsByLevelList)
    {
        this.spawnableObjectsByLevelList = spawnableObjectsByLevelList;
    }

    public T GetItem()
    {
        var upperBoundary = -1;
        ratioValueTotal = 0;
        chanceBoundariesList.Clear();
        var spawnableObject = default(T);

        foreach (var spawnableObjectsByLevel in spawnableObjectsByLevelList)
        {
            // проверка для текущего уровня
            if (spawnableObjectsByLevel.dungeonLevel == GameManager.Instance.GetCurrentDungeonLevel())
            {
                foreach (var spawnableObjectRatio in spawnableObjectsByLevel.spawnableObjectRatioList)
                {
                    var lowerBoundary = upperBoundary + 1;

                    upperBoundary = lowerBoundary + spawnableObjectRatio.ratio - 1;

                    ratioValueTotal += spawnableObjectRatio.ratio;

                    chanceBoundariesList.Add(new chanceBoundaries() { spawnableObject = spawnableObjectRatio.dungeonObject, lowBoundaryValue = lowerBoundary, highBoundaryValue = upperBoundary });

                }
            }
        }

        if (chanceBoundariesList.Count == 0) return default(T);

        var lookUpValue = Random.Range(0, ratioValueTotal);

        // проход по списку, чтобы найти соотвествующие детали
        foreach (var spawnChance in chanceBoundariesList)
        {
            if (lookUpValue >= spawnChance.lowBoundaryValue && lookUpValue <= spawnChance.highBoundaryValue)
            {
                spawnableObject = spawnChance.spawnableObject;
                break;
            }
        }


        return spawnableObject;
    }

}
