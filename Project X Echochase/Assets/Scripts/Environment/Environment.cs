using UnityEngine;

[DisallowMultipleComponent]
public class Environment : MonoBehaviour
{
    //Действие этого класса для игрового объекта окружения, освещение которого постепенно исчезает

    #region Header References
    [Space(10)]
    [Header("Ссылки")]
    #endregion
    #region Tooltip
    [Tooltip("Заполните компонент SpriteRenderer на префабе")]
    #endregion

    public SpriteRenderer spriteRenderer;

    #region Validation

#if UNITY_EDITOR

    private void OnValidate()
    {
        HelperUtilities.ValidateCheckNullValue(this, nameof(spriteRenderer), spriteRenderer);
    }

#endif

    #endregion Validation
}
