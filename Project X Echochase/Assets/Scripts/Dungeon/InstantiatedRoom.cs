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

    public void Initialise(GameObject roomGameobject)
    {
        PopulateTilemapMemberVariables(roomGameobject);
        
        DisableCollisionTilemapRenderer();
        
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
            else if (tilemap.CompareTag("minimapTilemap "))
            {
                minimapTilemap = tilemap;
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
