using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class ActivateRooms : MonoBehaviour
{
    #region Header POPULATE WITH MINIMAP CAMERA
    [Header("��������� � ������� ������ ���������")]
    #endregion Header
    [SerializeField] private Camera miniMapCamera;

    private Camera cameraMain;

    //Start ���������� ����� ������ ����������� �����
    private void Start()
    {
        //��� main ������
        cameraMain = Camera.main;

        InvokeRepeating("EnableRooms", 0.5f, 0.75f);
    }

    private void EnableRooms()
    {
        HelperUtilities.CameraWorldPositionBounds(out var miniMapCameraWorldPositionLowerBounds,
                out var miniMapCameraWorldPositionUpperBounds, miniMapCamera);

        HelperUtilities.CameraWorldPositionBounds(out var mainCameraWorldPositionLowerBounds,
                out var mainCameraWorldPositionUpperBounds, cameraMain);

        //��������� �� �������� ����������
        foreach (var keyValuePair in DungeonBuilder.Instance.dungeonBuilderRoomDictionary)
        {
            var room = keyValuePair.Value;

            //���� ������� ��������� � ������� ��������� ������ miniMap, �� ����������� ������� ������ �������
            if ((room.lowerBounds.x <= miniMapCameraWorldPositionUpperBounds.x && room.lowerBounds.y <= miniMapCameraWorldPositionUpperBounds.y) &&
                (room.upperBounds.x >= miniMapCameraWorldPositionLowerBounds.x && room.upperBounds.y >= miniMapCameraWorldPositionLowerBounds.y))
            {
                room.instantiatedRoom.gameObject.SetActive(true);

                //���� ������� ��������� � �������� ������� ������ �������� ������, �� ����������� ������� ������ ���������
                if ((room.lowerBounds.x <= mainCameraWorldPositionUpperBounds.x && room.lowerBounds.y <= mainCameraWorldPositionUpperBounds.y) &&
                (room.upperBounds.x >= mainCameraWorldPositionLowerBounds.x && room.upperBounds.y >= mainCameraWorldPositionLowerBounds.y))
                {
                    room.instantiatedRoom.ActivateEnvironmentGameObjects();
                }
                else
                {
                    room.instantiatedRoom.DeactivateEnvironmentGameObjects();
                }
            }
            else
            {
                room.instantiatedRoom.gameObject.SetActive(false);
            }
        }
    }

    #region Validation
#if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtilities.ValidateCheckNullValue(this, nameof(miniMapCamera), miniMapCamera);
    }
#endif
    #endregion
}
