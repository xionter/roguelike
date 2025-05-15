using System;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class PoolManager : SingletonMonobehaviour<PoolManager>
{
    #region Tooltip
    [Tooltip("Заполните этот массив префабами, которые вы хотите добавить в пул, и укажите количество объектов, которые нужно создать для каждого.")]
    #endregion
    [SerializeField] private Pool[] poolArray = null;
    private Transform objectPoolTransform;
    private Dictionary<int, Queue<Component>> poolDictionary = new Dictionary<int, Queue<Component>>();

    [System.Serializable]
    public struct Pool
    {
        public int poolSize;
        public GameObject prefab;
        public string componentType;
    }

    private void Start()
    {
        // Этот объект Singleton будет родительским объектом для пула
        objectPoolTransform = this.gameObject.transform;

        // Создание пулов объектов при старте
        for (int i = 0; i < poolArray.Length; i++)
        {
            CreatePool(poolArray[i].prefab, poolArray[i].poolSize, poolArray[i].componentType);
        }

    }

    /// <summary>
    /// Создать пул объектов с указанными префабами и заданным размером пула для каждого
    /// </summary>
    private void CreatePool(GameObject prefab, int poolSize, string componentType)
    {
        int poolKey = prefab.GetInstanceID();

        string prefabName = prefab.name; // получить имя префаба

        GameObject parentGameObject = new GameObject(prefabName + "Anchor"); // создать родительский объект для дочерних объектов

        parentGameObject.transform.SetParent(objectPoolTransform);

        if (!poolDictionary.ContainsKey(poolKey))
        {
            poolDictionary.Add(poolKey, new Queue<Component>());

            for (int i = 0; i < poolSize; i++)
            {
                GameObject newObject = Instantiate(prefab, parentGameObject.transform) as GameObject;

                newObject.SetActive(false);

                poolDictionary[poolKey].Enqueue(newObject.GetComponent(Type.GetType(componentType)));

            }
        }

    }

    /// <summary>
    /// Повторно использовать компонент объекта из пула. 'prefab' — это префаб игрового объекта, содержащий компонент. 'position' — это мировая позиция для игрового объекта, где он должен появиться при активации. 'rotation' задаёт поворот объекта.
    /// </summary>
    public Component ReuseComponent(GameObject prefab, Vector3 position, Quaternion rotation)
    {
        int poolKey = prefab.GetInstanceID();

        if (poolDictionary.ContainsKey(poolKey))
        {
            // Получить объект из очереди пула
            Component componentToReuse = GetComponentFromPool(poolKey);

            ResetObject(position, rotation, componentToReuse, prefab);

            return componentToReuse;
        }
        else
        {
            Debug.Log("Нет пула для " + prefab);
            return null;
        }
    }

    /// <summary>
    /// Получить компонент игрового объекта из пула, используя 'poolKey'
    /// </summary>
    private Component GetComponentFromPool(int poolKey)
    {
        Component componentToReuse = poolDictionary[poolKey].Dequeue();
        poolDictionary[poolKey].Enqueue(componentToReuse);

        if (componentToReuse.gameObject.activeSelf == true)
        {
            componentToReuse.gameObject.SetActive(false);
        }

        return componentToReuse;
    }

    /// <summary>
    /// Сбросить состояние игрового объекта.
    /// </summary>
    private void ResetObject(Vector3 position, Quaternion rotation, Component componentToReuse, GameObject prefab)
    {
        componentToReuse.transform.position = position;
        componentToReuse.transform.rotation = rotation;
        componentToReuse.gameObject.transform.localScale = prefab.transform.localScale;
    }

    #region Validation
#if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtilities.ValidateCheckEnumerableValues(this, nameof(poolArray), poolArray);
    }
#endif
    #endregion
}