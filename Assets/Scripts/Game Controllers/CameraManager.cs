using UnityEngine;
using Unity.Cinemachine;

public class CameraManager : MonoBehaviour
{
    public Transform TargetFollow;
    public CinemachineCamera cinemachineCamera;
    public bool IsSetTargetDone { get; private set; }

    void LateUpdate()
    {
        if (TargetFollow != null)
        {
            if (!IsSetTargetDone)
            {
                IsSetTargetDone = true;
                cinemachineCamera.Target.TrackingTarget = TargetFollow;
            }
        }
    }
}
