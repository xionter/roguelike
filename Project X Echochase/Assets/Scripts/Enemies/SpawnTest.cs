using System.Collections.Generic;
using UnityEngine;
using System;
// НЕПОНЯТНО, ПОЧЕМУ НЕ РАБОТАЕТ. НЕ УДАЛЯТЬ КОМЕНТЫ
public class SpawnTest : MonoBehaviour
{
    public RoomTemplateSO roomTemplateSO;
    private List<SpawnableObjectsByLevel<EnemyDetailsSO>> testLevelSpawnList;
    private RandomSpawnableObject<EnemyDetailsSO> randomEnemyHelperClass;
    private List<GameObject> instantiatedEnemyList = new List<GameObject>();
    private GameObject instantiatedEnemy;
    private void Start()
    {
        testLevelSpawnList = roomTemplateSO.enemiesByLevelList;
        randomEnemyHelperClass = new RandomSpawnableObject<EnemyDetailsSO>(testLevelSpawnList);
    }
    private void OnEnable()
    {
        StaticEventHandler.OnRoomChanged += StaticEventHandler_OnRoomChanged;
    }

    private void OnDisable()
    { 
        StaticEventHandler.OnRoomChanged -= StaticEventHandler_OnRoomChanged;
    }


    private void StaticEventHandler_OnRoomChanged(RoomChangedEventArgs roomChangedEventArgs)
    {
        if (instantiatedEnemyList != null && instantiatedEnemyList.Count > 0)
        {
            foreach (GameObject enemy in instantiatedEnemyList)
            {
                Destroy(enemy);
            }
        }
        if (roomChangedEventArgs.room == null) {
            Debug.LogError("roomChangedEventArgs.room is null!");
            return;
        }
        RoomTemplateSO roomTemplate = DungeonBuilder.Instance.GetRoomTemplate(roomChangedEventArgs.room.templateID);

        if (roomTemplate != null)
        {
            testLevelSpawnList = roomTemplate.enemiesByLevelList;

            randomEnemyHelperClass = new RandomSpawnableObject<EnemyDetailsSO>(testLevelSpawnList);
        }
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            if(instantiatedEnemy != null)
            { 
                Destroy(instantiatedEnemy);
            }
            EnemyDetailsSO enemyDetails = randomEnemyHelperClass.GetItem();
            if (enemyDetails != null)
            {
                Debug.Log(HelperUtilities.GetSpawnPositionNearestToPlayer(HelperUtilities.GetMouseWorldPosition()));
                instantiatedEnemy = Instantiate(enemyDetails.enemyPrefab,
                    HelperUtilities.GetSpawnPositionNearestToPlayer(HelperUtilities.GetMouseWorldPosition()),
                    Quaternion.identity);
            }

            // EnemyDetailsSO enemyDetails = randomEnemyHelperClass.GetItem();
            // if (enemyDetails != null)
            //    instantiatedEnemyList.Add( Instantiate(enemyDetails.enemyPrefab, 
            //    HelperUtilities.GetSpawnPositionNearestToPlayer(HelperUtilities.GetMouseWorldPosition()), 
            //    Quaternion.identity));
            else Debug.Log("GAY");
            
        }
    }
}
