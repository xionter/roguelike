using System;
using UnityEngine;
using Unity.Cinemachine; 
using System.Collections.Generic;

[RequireComponent(typeof(CinemachineTargetGroup))]
public class CinemachineTarget : MonoBehaviour
{
    private CinemachineTargetGroup cinemachineTargetGroup;
    [SerializeField] private Transform cursorTarget;

    private void Awake()
    {
        cinemachineTargetGroup = GetComponent<CinemachineTargetGroup>();
    }

    private void Start()
    {
        SetCinemachineTargetGroup();
    }

    private void SetCinemachineTargetGroup()
    {
        cinemachineTargetGroup.Targets = new List<CinemachineTargetGroup.Target>();        
        // игрок таргет
        cinemachineTargetGroup.AddMember(
            GameManager.Instance.GetPlayer().transform,
            1f,    // weight
            2.5f   // radius
        );

        // курсор таргет
        cinemachineTargetGroup.AddMember(
            cursorTarget,
            1f,    // weight
            1f     // radius
        );
    }

    private void Update()
    {
        cursorTarget.position = HelperUtilities.GetMouseWorldPosition();
    }
}
