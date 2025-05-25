using System.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;

[DisallowMultipleComponent]
[RequireComponent(typeof(InstantiatedRoom))]
public class RoomLightingControl : MonoBehaviour
{
    private InstantiatedRoom instantiatedRoom;
    
    private void Awake()
    {
        // Загрузка компонентов
        instantiatedRoom = GetComponent<InstantiatedRoom>();
    }

    private void OnEnable()
    {
        // Подписка на событие изменения комнаты
        StaticEventHandler.OnRoomChanged += StaticEventHandler_OnRoomChanged;
    }

    private void OnDisable()
    {
        // Отписка от события изменения комнаты
        StaticEventHandler.OnRoomChanged -= StaticEventHandler_OnRoomChanged;
    }

    /// <summary>
    /// Обработчик события изменения комнаты
    /// </summary>
    private void StaticEventHandler_OnRoomChanged(RoomChangedEventArgs roomChangedEventArgs)
    {
        if (roomChangedEventArgs.room == instantiatedRoom.room && !instantiatedRoom.room.isLit)
        {
            // Плавное появление освещения комнаты
            FadeInRoomLighting();

            //Убедитесь, что игровые объекты декорирования помещения активированы
            instantiatedRoom.ActivateEnvironmentGameObjects();

            //Исчезают в окружающей среде декорации игровые объекты освещение
            FadeInEnvironmentLighting();

            // Плавное появление освещения дверей комнаты
            FadeInDoors();

            instantiatedRoom.room.isLit = true;
        }
    }

    /// <summary>
    /// Плавное появление освещения комнаты
    /// </summary>
    private void FadeInRoomLighting()
    {
        StartCoroutine(FadeInRoomLightingRoutine(instantiatedRoom));
    }

    /// <summary>
    /// Корутина для плавного появления освещения комнаты
    /// </summary>
    private IEnumerator FadeInRoomLightingRoutine(InstantiatedRoom instantiatedRoom)
    {
        // Создание нового материала для плавного появления
        Material material = new Material(GameResources.Instance.variableLitShader);

        instantiatedRoom.groundTilemap.GetComponent<TilemapRenderer>().material = material;
        instantiatedRoom.decoration1Tilemap.GetComponent<TilemapRenderer>().material = material;
        instantiatedRoom.decoration2Tilemap.GetComponent<TilemapRenderer>().material = material;
        instantiatedRoom.frontTilemap.GetComponent<TilemapRenderer>().material = material;
        instantiatedRoom.minimapTilemap.GetComponent<TilemapRenderer>().material = material;

        for (float i = 0.05f; i <= 1f; i += Time.deltaTime / Settings.fadeInTime)
        {
            material.SetFloat("Alpha_Slider", i);
            yield return null;
        }

        // Возврат материала к освещённому материалу
        instantiatedRoom.groundTilemap.GetComponent<TilemapRenderer>().material = GameResources.Instance.litMaterial;
        instantiatedRoom.decoration1Tilemap.GetComponent<TilemapRenderer>().material = GameResources.Instance.litMaterial;
        instantiatedRoom.decoration2Tilemap.GetComponent<TilemapRenderer>().material = GameResources.Instance.litMaterial;
        instantiatedRoom.frontTilemap.GetComponent<TilemapRenderer>().material = GameResources.Instance.litMaterial;
        instantiatedRoom.minimapTilemap.GetComponent<TilemapRenderer>().material = GameResources.Instance.litMaterial;
    }

    /// <summary>
    /// Исчезают в окружающей среде декорации игровые объекты
    /// </summary>
    private void FadeInEnvironmentLighting()
    {
        Material material = new Material(GameResources.Instance.variableLitShader);

        Environment[] environmentComponents = GetComponentsInChildren<Environment>();

        foreach (Environment environmentComponent in environmentComponents)
        {
            if (environmentComponent.spriteRenderer != null)
                environmentComponent.spriteRenderer.material = material;
        }

        StartCoroutine(FadeInEnvironmentLightingRoutine(material, environmentComponents));
    }

    private IEnumerator FadeInEnvironmentLightingRoutine(Material material, Environment[] environmentComponents)
    {
        for (float i = 0.05f; i <= 1f; i += Time.deltaTime / Settings.fadeInTime)
        {
            material.SetFloat("Alpha_Slider", i);
            yield return null;
        }

        foreach (Environment environmentComponent in environmentComponents)
        {
            if (environmentComponent.spriteRenderer != null)
                environmentComponent.spriteRenderer.material = GameResources.Instance.litMaterial;
        }
    }

    /// <summary>
    /// Плавное появление дверей
    /// </summary>
    private void FadeInDoors()
    {
        Door[] doorArray = GetComponentsInChildren<Door>();

        foreach (Door door in doorArray)
        {
            DoorLightingControl doorLightingControl = door.GetComponentInChildren<DoorLightingControl>();

            doorLightingControl.FadeInDoor(door);
        }
    }
}