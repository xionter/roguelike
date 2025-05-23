using System.Collections;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(MaterializeEffect))]
public class Chest : MonoBehaviour, IUseable
{
    #region Tooltip
    [Tooltip("Установите цвет, который будет использоваться для эффекта материализации")]
    #endregion Tooltip
    [ColorUsage(false, true)]
    [SerializeField] private Color materializeColor;
    #region Tooltip
    [Tooltip("Установите время, за которое сундук материализуется")]
    #endregion Tooltip
    [SerializeField] private float materializeTime = 3f;
    #region Tooltip
    [Tooltip("Укажите трансформ точки спавна предмета (ItemSpawnPoint)")]
    #endregion Tooltip
    [SerializeField] private Transform itemSpawnPoint;
    private int healthPercent;
    private WeaponDetailsSO weaponDetails;
    private int ammoPercent;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private MaterializeEffect materializeEffect;
    private bool isEnabled = false;
    private ChestState chestState = ChestState.closed;
    private GameObject chestItemGameObject;
    private ChestItem chestItem;
    private TextMeshPro messageTextTMP;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        materializeEffect = GetComponent<MaterializeEffect>();
        messageTextTMP = GetComponentInChildren<TextMeshPro>();
    }
    

    /// <summary>
    /// инициализация сундука. Его можно сделать видимым сразу или материализовать
    /// </summary>
    public void Initialize(bool shouldMaterialize, int healthPercent, WeaponDetailsSO weaponDetails, int ammoPercent)
    {
        this.healthPercent = healthPercent;
        this.weaponDetails = weaponDetails;
        this.ammoPercent = ammoPercent;

        if (shouldMaterialize)
        {
            StartCoroutine(MaterializeChest());
        }
        else
        {
            EnableChest();
        }
    }


    private IEnumerator MaterializeChest()
    {
        SpriteRenderer[] spriteRendererArray = new SpriteRenderer[] { spriteRenderer };

        yield return StartCoroutine(materializeEffect.MaterializeRoutine(GameResources.Instance.materializeShader, materializeColor, materializeTime, spriteRendererArray, GameResources.Instance.litMaterial));

        EnableChest();
    }

    private void EnableChest()
    {
        isEnabled = true;
    }


    public void UseItem()
    {
        if (!isEnabled) return;

        switch (chestState)
        {
            case ChestState.closed:
                OpenChest();
                break;

            case ChestState.healthItem:
                CollectHealthItem();
                break;

            case ChestState.ammoItem:
                CollectAmmoItem();
                break;

            case ChestState.weaponItem:
                CollectWeaponItem();
                break;

            case ChestState.empty:
                return;

            default:
                return;
        }
    }

    /// <summary>
    /// открыть сундук при первом использовании
    /// </summary>
    private void OpenChest()
    {
        animator.SetBool(Settings.use, true);

        // саунд эффект
        //SoundEffectManager.Instance.PlaySoundEffect(GameResources.Instance.chestOpen);

        // если у игрока уже есть оружие, то ставим wepon в null
        if (weaponDetails != null)
        {
            if (GameManager.Instance.GetPlayer().IsWeaponHeldByPlayer(weaponDetails))
                weaponDetails = null;
        }

        UpdateChestState();
    }


    private void UpdateChestState()
    {
        if (healthPercent != 0)
        {
            chestState = ChestState.healthItem;
            InstantiateHealthItem();
        }
        else if (ammoPercent != 0)
        {
            chestState = ChestState.ammoItem;
            InstantiateAmmoItem();
        }
        else if (weaponDetails != null)
        {
            chestState = ChestState.weaponItem;
            InstantiateWeaponItem();
        }
        else
        {
            chestState = ChestState.empty;
        }
    }

    private void InstantiateItem()
    {
        chestItemGameObject = Instantiate(GameResources.Instance.chestItemPrefab, this.transform);

        chestItem = chestItemGameObject.GetComponent<ChestItem>();
    }


    private void InstantiateHealthItem()
    {
        InstantiateItem();

        chestItem.Initialize(GameResources.Instance.heartIcon, healthPercent.ToString() + "%", itemSpawnPoint.position, materializeColor);
    }


    private void CollectHealthItem()
    {
        // проверка, что предмет есть и материализован
        if (chestItem == null || !chestItem.isItemMaterialized) return;

        // добавить хп игроку
        GameManager.Instance.GetPlayer().health.AddHealth(healthPercent);

        // саунд эффект поднятия
        //SoundEffectManager.Instance.PlaySoundEffect(GameResources.Instance.healthPickup);

        healthPercent = 0;

        Destroy(chestItemGameObject);

        UpdateChestState();
    }


    private void InstantiateAmmoItem()
    {
        InstantiateItem();

        chestItem.Initialize(GameResources.Instance.bulletIcon, ammoPercent.ToString() + "%", itemSpawnPoint.position, materializeColor);
    }


    private void CollectAmmoItem()
    {
        // проверка, что предмет есть и материализован
        if (chestItem == null || !chestItem.isItemMaterialized) return;

        Player player = GameManager.Instance.GetPlayer();

        // добавить патронов текущему оружию
        player.reloadWeaponEvent.CallReloadWeaponEvent(player.activeWeapon.GetCurrentWeapon(), ammoPercent);

        // саунд эффект поднятия
        //SoundEffectManager.Instance.PlaySoundEffect(GameResources.Instance.ammoPickup);

        ammoPercent = 0;

        Destroy(chestItemGameObject);

        UpdateChestState();
    }

    private void InstantiateWeaponItem()
    {
        InstantiateItem();

        chestItemGameObject.GetComponent<ChestItem>().Initialize(weaponDetails.weaponSprite, weaponDetails.weaponName, itemSpawnPoint.position, materializeColor);
    }


    private void CollectWeaponItem()
    {
        // проверка, что предмет есть и материализован
        if (chestItem == null || !chestItem.isItemMaterialized) return;

        // если у игрока ещё нет оружия
        if (!GameManager.Instance.GetPlayer().IsWeaponHeldByPlayer(weaponDetails))
        {
            GameManager.Instance.GetPlayer().AddWeaponToPlayer(weaponDetails);

            // саунд эффект поднятия
            //SoundEffectManager.Instance.PlaySoundEffect(GameResources.Instance.weaponPickup);
        }
        else
        {
            StartCoroutine(DisplayMessage("WEAPON\nALREADY\nEQUIPPED", 5f));
        }
        weaponDetails = null;

        Destroy(chestItemGameObject);

        UpdateChestState();
    }


    private IEnumerator DisplayMessage(string messageText, float messageDisplayTime)
    {
        messageTextTMP.text = messageText;

        yield return new WaitForSeconds(messageDisplayTime);

        messageTextTMP.text = "";
    }
}
