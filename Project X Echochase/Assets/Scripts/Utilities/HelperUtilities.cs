using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class HelperUtilities
{
    public static Camera mainCamera; 

    /// <summary> 
    /// Получить позицию курсора 
    /// </summary> 
    public static Vector3 GetMouseWorldPosition() { 
        if (mainCamera == null) mainCamera = Camera.main; 
        Vector3 mouseScreenPosition = Input.mousePosition; 
        // Ограничиваем позицию размером экрана
        mouseScreenPosition.x = Mathf.Clamp(mouseScreenPosition.x, 0f, Screen.width); 
        mouseScreenPosition.y = Mathf.Clamp(mouseScreenPosition.y, 0f, Screen.height); 
        Vector3 worldPosition = mainCamera.ScreenToWorldPoint(mouseScreenPosition); 

        worldPosition.z = 0f; 
        return worldPosition; 
    } 

    /// <summary> 
    /// Получить угол в градусах с направления вектора 
    /// </summary> 
    public static float GetAngleFromVector(Vector3 vector) 
    { 
        var radians = Mathf.Atan2(vector.y, vector.x); 

        var degrees = radians * Mathf.Rad2Deg; 

        return degrees;
    }


    public static AimDirection GetAimDirection(float angleDegrees) 
    { 
        AimDirection aimDirection; 
        //UpRight 
        if (angleDegrees >= 22f && angleDegrees <= 67f) 
        { 
            aimDirection = AimDirection.UpRight; 
        } 
        // Up 
        else if (angleDegrees > 67f && angleDegrees <= 112f) 
        { 
            aimDirection = AimDirection.Up; 
        }
        // Up Left 
        else if (angleDegrees > 112f && angleDegrees <= 158f) 
        { 
            aimDirection = AimDirection.UpLeft; 
        }
        // Left 
        else if ((angleDegrees <= 180f && angleDegrees > 158f) || 
                (angleDegrees > -180 && angleDegrees <= -135f)) 
        { 
            aimDirection = AimDirection.Left;
        }
        // Down 
        else if (angleDegrees > -135f && angleDegrees <= -45f) 
        { 
            aimDirection = AimDirection.Down; 
        }

        // Right 
        else if ((angleDegrees > -45f && angleDegrees <= 0f) || 
                (angleDegrees > 0f && angleDegrees < 22f)) 
        { 
            aimDirection = AimDirection.Right;
        }
        else
        {
            aimDirection = AimDirection.Right;
        }

        return aimDirection;
    }
    public static bool ValidateCheckNullValue(Object thisObject, string fieldName, UnityEngine.Object objectToCheck) 
    { 
        if (objectToCheck == null) 
        { 
            Debug.Log(fieldName + "is null and must contain a value in object" + thisObject.name.ToString()); 
            return true; 
        } 
        return false;
    }
    /// <summary>
    /// Проверяет если строка пустая
    /// </summary>
    public static bool ValidateCheckEmptyString(Object thisObject, string fieldName, string stringToCheck)
    {
        if (stringToCheck == "")
        {
            Debug.Log(fieldName + " is empty and must contain a value in object " + thisObject.name.ToString());
            return true;
        }

        return false;
    }
    /// <summary>
    /// Список пуст или содержит null значения - возвращает true если есть ошибка
    /// </summary>
    public static bool ValidateCheckEnumerableValues(Object thisObject, string fieldName, IEnumerable enumerableObjectToCheck)
    {
        var error = false;
        var count = 0;

        if (enumerableObjectToCheck == null)
        {
            Debug.Log(fieldName + " is null in object " + thisObject.name.ToString());
            return true;
        }
        
        foreach (var item in enumerableObjectToCheck)
        {
            if (item == null)
            {
                Debug.Log(fieldName + " has null values in object " + thisObject.name.ToString());
                error = true;
            }
            else count++;
        }

        if (count == 0)
        {
            Debug.Log(fieldName + " has no values in object " + thisObject.name.ToString());
            error = true;
        }
        return error;
    }


    public static bool ValidateCheckPositiveValue(Object thisObject, string fieldName, int valueToCheck, bool isZeroAllowed) 
    {
        bool error = false; 
        
        if (isZeroAllowed) 
        { 
            if (valueToCheck < 0) 
            { 
                Debug.Log(fieldName + "must contain a positive value or zero in object" + thisObject.name.ToString()); 
                error = true; 
            } 
        }
        else 
        { 
            if (valueToCheck <= 0) 
            { 
                Debug.Log(fieldName + "must contain a positive value in object" + thisObject.name.ToString()); 
                error = true; 
            } 
        }
        return error; 
    }
}
