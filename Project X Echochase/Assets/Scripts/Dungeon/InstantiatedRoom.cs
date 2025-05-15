using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;
using System.Collections;

[DisallowMultipleComponent]
[RequireComponent(typeof(BoxCollider2D))]
public class InstantiatedRoom : MonoBehaviour
{
    [HideInInspector] public Room room;
    [HideInInspector] public Grid grid;
    [HideInInspector] public Tilemap groundTilemap;
    [HideInInspector] public Tilemap decoration1Tilemap;
    [HideInInspector] public Tilemap decoration2Tilemap;
    [HideInInspector] public Tilemap frontTilemap;
    [HideInInspector] public Tilemap collisionTilemap;
    [HideInInspector] public Tilemap minimapTilemap;
    [HideInInspector] public Bounds roomColliderBounds;

    private BoxCollider2D boxCollider2D;

    private void Awake()
    {
        boxCollider2D = GetComponent<BoxCollider2D>();

        roomColliderBounds = boxCollider2D.bounds;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == Settings.playerTag && room != GameManager.Instance.GetCurrentRoom())
        {
            this.room.isPreviouslyVisited = true;

            StaticEventHandler.CallRoomChangedEvent(room);
        }
    }

    public void Initialise(GameObject roomGameobject)
    {
        PopulateTilemapMemberVariables(roomGameobject);

        BlockOffUnusedDoorWays();

        AddDoorsToRooms();

        DisableCollisionTilemapRenderer();
    }

    private void BlockOffUnusedDoorWays()
    {
        // Проходим по всем дверным проёмам
        foreach (DoorWay doorway in room.doorWayList)
        {
            if (doorway.isConnected)
                continue;

            // Закрываем неиспользуемые дверные проёмы с помощью тайлов на тайлмапах
            if (collisionTilemap != null)
            {
                BlockADoorwayOnTilemapLayer(collisionTilemap, doorway);
            }

            if (minimapTilemap != null)
            {
                BlockADoorwayOnTilemapLayer(minimapTilemap, doorway);
            }

            if (groundTilemap != null)
            {
                BlockADoorwayOnTilemapLayer(groundTilemap, doorway);
            }

            if (decoration1Tilemap != null)
            {
                BlockADoorwayOnTilemapLayer(decoration1Tilemap, doorway);
            }

            if (decoration2Tilemap != null)
            {
                BlockADoorwayOnTilemapLayer(decoration2Tilemap, doorway);
            }

            if (frontTilemap != null)
            {
                BlockADoorwayOnTilemapLayer(frontTilemap, doorway);
            }
        }
    }

    /// <summary>
    /// Закрыть дверной проём на слое тайлмапа
    /// </summary>
    private void BlockADoorwayOnTilemapLayer(Tilemap tilemap, DoorWay doorway)
    {
        switch (doorway.orientation)
        {
            case Orientation.north:
            case Orientation.south:
                BlockDoorwayHorizontally(tilemap, doorway);
                break;

            case Orientation.east:
            case Orientation.west:
                BlockDoorwayVertically(tilemap, doorway);
                break;

            case Orientation.none:
                break;
        }
    }

    /// <summary>
    /// Закрыть дверной проём горизонтально - для северных и южных дверей
    /// </summary>
    private void BlockDoorwayHorizontally(Tilemap tilemap, DoorWay doorway)
    {
        Vector2Int startPosition = doorway.doorwayStartCopyPosition;

        // Проходим по всем тайлам для копирования
        for (int xPos = 0; xPos < doorway.doorwayCopyTileWidth; xPos++)
        {
            for (int yPos = 0; yPos < doorway.doorwayCopyTileHeight; yPos++)
            {
                // Получаем поворот тайла, который копируем
                Matrix4x4 transformMatrix = tilemap.GetTransformMatrix(new Vector3Int(startPosition.x + xPos, startPosition.y - yPos, 0));

                // Копируем тайл
                tilemap.SetTile(new Vector3Int(startPosition.x + 1 + xPos, startPosition.y - yPos, 0), tilemap.GetTile(new Vector3Int(startPosition.x + xPos, startPosition.y - yPos, 0)));

                // Устанавливаем поворот скопированного тайла
                tilemap.SetTransformMatrix(new Vector3Int(startPosition.x + 1 + xPos, startPosition.y - yPos, 0), transformMatrix);
            }
        }
    }

    /// <summary>
    /// Закрыть дверной проём вертикально - для восточных и западных дверей
    /// </summary>
    private void BlockDoorwayVertically(Tilemap tilemap, DoorWay doorway)
    {
        Vector2Int startPosition = doorway.doorwayStartCopyPosition;

        // Проходим по всем тайлам для копирования
        for (int yPos = 0; yPos < doorway.doorwayCopyTileHeight; yPos++)
        {
            for (int xPos = 0; xPos < doorway.doorwayCopyTileWidth; xPos++)
            {
                // Получаем поворот тайла, который копируем
                Matrix4x4 transformMatrix = tilemap.GetTransformMatrix(new Vector3Int(startPosition.x + xPos, startPosition.y - yPos, 0));

                // Копируем тайл
                tilemap.SetTile(new Vector3Int(startPosition.x + xPos, startPosition.y - 1 - yPos, 0), tilemap.GetTile(new Vector3Int(startPosition.x + xPos, startPosition.y - yPos, 0)));

                // Устанавливаем поворот скопированного тайла
                tilemap.SetTransformMatrix(new Vector3Int(startPosition.x + xPos, startPosition.y - 1 - yPos, 0), transformMatrix);
            }
        }
    }

    private void PopulateTilemapMemberVariables(GameObject roomGameobject)
    {
        grid = roomGameobject.GetComponentInChildren<Grid>();

        Tilemap[] tilemaps = grid.GetComponentsInChildren<Tilemap>();
        foreach (Tilemap tilemap in tilemaps)
        {
            if (tilemap.CompareTag("groundTilemap"))
            {
                groundTilemap = tilemap;
            }
            else if (tilemap.CompareTag("decoration1Tilemap"))
            {
                decoration1Tilemap = tilemap;
            }
            else if (tilemap.CompareTag("decoration2Tilemap"))
            {
                decoration2Tilemap = tilemap;
            }
            else if (tilemap.CompareTag("frontTilemap"))
            {
                frontTilemap = tilemap;
            }
            else if (tilemap.CompareTag("collisionTilemap"))
            {
                collisionTilemap = tilemap;
            }
            else if (tilemap.CompareTag("minimapTilemap"))
            {
                minimapTilemap = tilemap;
            }
        }
    }

    /// <summary>
    /// Добавить открывающиеся двери, если это не коридор
    /// </summary>
    private void AddDoorsToRooms()
    {
        if (room.roomNodeType.isCorridorEW || room.roomNodeType.isCorridorNS) return;

        foreach (DoorWay doorway in room.doorWayList)
        {
            if (doorway.doorPrefab != null && doorway.isConnected)
            {
                float tileDistance = Settings.tileSizePixels / Settings.pixelsPerUnit;

                GameObject door = null;

                if (doorway.orientation == Orientation.north)
                {
                    door = Instantiate(doorway.doorPrefab, gameObject.transform);
                    door.transform.localPosition = new Vector3(doorway.position.x + tileDistance / 2f + 0.5f, doorway.position.y + tileDistance, 0f);
                }
                else if (doorway.orientation == Orientation.south)
                {
                    door = Instantiate(doorway.doorPrefab, gameObject.transform);
                    door.transform.localPosition = new Vector3(doorway.position.x + tileDistance / 2f + 0.5f, doorway.position.y, 0f);
                }
                else if (doorway.orientation == Orientation.east)
                {
                    door = Instantiate(doorway.doorPrefab, gameObject.transform);
                    door.transform.localPosition = new Vector3(doorway.position.x + tileDistance, doorway.position.y + tileDistance * 1.25f, 0f);
                }
                else if (doorway.orientation == Orientation.west)
                {
                    door = Instantiate(doorway.doorPrefab, gameObject.transform);
                    door.transform.localPosition = new Vector3(doorway.position.x, doorway.position.y + tileDistance * 1.25f, 0f);
                }

                // Получаем компонент двери
                Door doorComponent = door.GetComponent<Door>();

                // Устанавливаем, является ли дверь частью комнаты босса
                if (room.roomNodeType.isBossRoom)
                {
                    doorComponent.isBossRoomDoor = true;

                    doorComponent.LockDoor();
                }
            }
        }
    }

    private void DisableCollisionTilemapRenderer()
    {
        collisionTilemap.gameObject.GetComponent<TilemapRenderer>().enabled = false;
    }

    public void DisableRoomCollider()
    {
        boxCollider2D.enabled = false;
    }

    public void EnableRoomCollider()
    {
        boxCollider2D.enabled = true;
    }

    public void LockDoors()
    {
        Door[] doorArray = GetComponentsInChildren<Door>();

        foreach (Door door in doorArray)
        {
            door.LockDoor();
        }

        DisableRoomCollider();
    }

    public void UnlockDoors(float doorUnlockDelay)
    {
        StartCoroutine(UnlockDoorsRoutine(doorUnlockDelay));
    }

    private IEnumerator UnlockDoorsRoutine(float doorUnlockDelay)
    {
        if (doorUnlockDelay > 0f)
            yield return new WaitForSeconds(doorUnlockDelay);

        Door[] doorArray = GetComponentsInChildren<Door>();

        foreach (Door door in doorArray)
        {
            door.UnlockDoor();
        }

        EnableRoomCollider();
    }
}