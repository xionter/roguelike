using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Enemy))]
[DisallowMultipleComponent]

public class EnemyMovementAI : MonoBehaviour
{
    #region Tooltip

    [Tooltip("MovementDetailsSO должен содержать скорость движения врага")]

    #endregion Tooltip

    [SerializeField] private MovementDetailsSO movementDetails;
    private Enemy enemy;
    private Stack<Vector3> movementSteps = new Stack<Vector3>();
    private Vector3 playerReferencePosition;
    private Coroutine moveEnemyRoutine;
    private float currentEnemyPathRebuildCooldown;
    private WaitForFixedUpdate waitForFixedUpdate;
    [HideInInspector] public float moveSpeed;
    private bool chasePlayer = false;
    [HideInInspector] public int updateFrameNumber = 1;


    private void Awake()
    {
        enemy = GetComponent<Enemy>();

        moveSpeed = movementDetails.GetMoveSpeed();
    }

    private void Start()
    {
        waitForFixedUpdate = new WaitForFixedUpdate();
        playerReferencePosition = GameManager.Instance.GetPlayer().GetPlayerPosition();
    }

    private void Update()
    {
        MoveEnemy();
    }

    private void MoveEnemy()
    {
        currentEnemyPathRebuildCooldown -= Time.deltaTime;

        if (!chasePlayer && Vector3.Distance(transform.position, GameManager.Instance.GetPlayer().GetPlayerPosition()) <
            enemy.enemyDetails.chaseDistance)
        {
            chasePlayer = true;
        }

        if (!chasePlayer)
            return;

        if (Time.frameCount % Settings.targetFrameRateToSpreadPathFindingOver != updateFrameNumber) return;
        
        if (currentEnemyPathRebuildCooldown <= 0f ||
            (Vector3.Distance(playerReferencePosition, GameManager.Instance.GetPlayer().GetPlayerPosition()) >
             Settings.playerMoveDistanceToRebuildPath))
        {
            currentEnemyPathRebuildCooldown = Settings.playerMoveDistanceToRebuildPath;

            playerReferencePosition = GameManager.Instance.GetPlayer().GetPlayerPosition();

            CreatePath();

            if (movementSteps != null)
            {
                if (moveEnemyRoutine != null)
                {
                    enemy.idleEvent.CallIdleEvent();
                    StopCoroutine(moveEnemyRoutine);
                }

                moveEnemyRoutine = StartCoroutine(MoveEnemyRoutine(movementSteps));
            }
        }
    }
    
    private IEnumerator MoveEnemyRoutine(Stack<Vector3> movementSteps)
    {
        while (movementSteps.Count > 0)
        {
            var nextPosition = movementSteps.Pop();

            while (Vector3.Distance(nextPosition, transform.position) > 0.2f)
            {
                enemy.movementToPositionEvent.CallMovementToPositionEvent(
                    nextPosition, transform.position, moveSpeed, (nextPosition - transform.position).normalized);

                yield return waitForFixedUpdate;
            }

            yield return waitForFixedUpdate;
        } 

        enemy.idleEvent.CallIdleEvent();
    }

    public void SetUpdateFrameNumber(int updateFrameNumber)
    {
        this.updateFrameNumber = updateFrameNumber;
    }


    private void CreatePath()
    {
        var currentRoom = GameManager.Instance.GetCurrentRoom();

        var grid = currentRoom.instantiatedRoom.grid;

        var enemyGridPosition = grid.WorldToCell(transform.position);
        var playerGridPosition = GetNearestNonObstaclePlayerPosition(currentRoom);

        movementSteps = AStar.BuildPath(currentRoom, enemyGridPosition, playerGridPosition);

        if (movementSteps != null)
        {
            movementSteps.Pop();
        }
        else
        {
            enemy.idleEvent.CallIdleEvent();
        }
    }

    private Vector3Int GetNearestNonObstaclePlayerPosition(Room currentRoom)
    {
        var playerPosition = GameManager.Instance.GetPlayer().GetPlayerPosition();
        
        var playerCellPosition = currentRoom.instantiatedRoom.grid.WorldToCell(playerPosition);
        
        var adjustedPlayerCellPosition = new Vector2Int(playerCellPosition.x - currentRoom.templateLowerBounds.x,
            playerCellPosition.y - currentRoom.templateLowerBounds.y);
        
        var obstacle = currentRoom.instantiatedRoom.aStarMovementPenalty[adjustedPlayerCellPosition.x,
            adjustedPlayerCellPosition.y];

        if (obstacle != 0)
        {
            return playerCellPosition;
        }

        for (var i = -1; i <= 1; i++)
        {
            for (var j = -1; j <= 1; j++)
            {
                if (j == 0 && i == 0)
                    continue;
                try
                {
                    obstacle = currentRoom.instantiatedRoom.aStarMovementPenalty[
                        adjustedPlayerCellPosition.x + i,
                        adjustedPlayerCellPosition.y + j];
                    if (obstacle != 0) return new Vector3Int(playerCellPosition.x + i,
                        playerCellPosition.y + j, 0 );
                }
                catch
                {
                    continue;
                }
            }
        }
        return playerCellPosition;
    }

    
    #region Validation

#if UNITY_EDITOR

    private void OnValidate()
    {
        HelperUtilities.ValidateCheckNullValue(this, nameof(movementDetails), movementDetails);
    }
    
#endif
    
    #endregion
    
}
