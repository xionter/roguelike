using System.Collections;
using UnityEngine;

[DisallowMultipleComponent]
public class DoorLightingControl : MonoBehaviour
{
    private bool isLit = false;
    private Door door;

    private void Awake()
    {
        door = GetComponentInParent<Door>();
    }

    /// <summary>
    /// Затемнить двери
    /// </summary>
    public void FadeInDoor(Door door)
    {
        var material = new Material(GameResources.Instance.variableLitShader);

        if (!isLit)
        {
            var spriteRendererArray = GetComponentsInParent<SpriteRenderer>();

            foreach (var spriteRenderer in spriteRendererArray)
            {
                StartCoroutine(FadeInDoorRoutine(spriteRenderer, material));
            }

            isLit = true;
        }
    }
    
    private IEnumerator FadeInDoorRoutine(SpriteRenderer spriteRenderer, Material material)
    {
        spriteRenderer.material = material;

        for (var i = 0.05f; i <= 1f; i += Time.deltaTime / Settings.fadeInTime)
        {
            material.SetFloat("Alpha_Slider", i);
            yield return null;
        }

        spriteRenderer.material = GameResources.Instance.litMaterial;
    }

    // Затемнить если сработал триггер
    private void OnTriggerEnter2D(Collider2D collision)
    {
        FadeInDoor(door);
    }
}
