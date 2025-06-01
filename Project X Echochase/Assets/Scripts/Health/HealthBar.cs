using UnityEngine;

public class HealthBar : MonoBehaviour
{
    #region Header GameObject References

    [Space(10)]
    [Header("GameObject References")]

    #endregion Header GameObject References

    #region Tooltip

    [Tooltip("Populate with the child Bar gameobject ")]

    #endregion Tooltip

    [SerializeField] private GameObject healthBar;

    /// <summary>
    /// Включить полоску здоровья
    /// </summary>
    public void EnableHealthBar()
    {
        gameObject.SetActive(true);
    }

    /// <summary>
    /// Выключить полоску здоровья
    /// </summary>
    public void DisableHealthBar()
    {
        gameObject.SetActive(false);
    }

    /// <summary>
    /// Поставить значения полоски в процентах
    /// </summary>
    public void SetHealthBarValue(float healthPercent)
    {
        healthBar.transform.localScale = new Vector3(healthPercent, 1f, 1f);
    }
}