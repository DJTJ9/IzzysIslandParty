using System;
using UnityEngine;
using Unity.Cinemachine;

public class CameraFocus : MonoBehaviour
{
    public CinemachineBrain Brain;
    public ICinemachineCamera CamA;
    public ICinemachineCamera CamB;

    private void Start()
    {
        CamA = GetComponent<CinemachineCamera>();
        CamB = GetComponent<CinemachineCamera>();

        int layer = 1;
        int priority = 1;
        float weight = 1f;
        float blendTime = 1f;
        Brain.SetCameraOverride(layer, priority, CamA, CamB, weight, blendTime);
    }
}
