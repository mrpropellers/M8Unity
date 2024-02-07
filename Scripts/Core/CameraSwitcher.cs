using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Dirtywave.M8
{
    [RequireComponent(typeof(PlayerInput))]
    public class CameraSwitcher : MonoBehaviour
    {
        [SerializeField]
        private CinemachineVirtualCamera[] cameras;

        public void OnCloseCam(InputValue val)
        {
            if (!val.isPressed)
                return;
            cameras[0].Priority = int.MaxValue;
            cameras[1].Priority = int.MinValue;
        }
        public void OnHalfCam(InputValue val)
        {
            if (!val.isPressed)
                return;
            cameras[1].Priority = int.MaxValue;
            cameras[0].Priority = int.MinValue;
        }
        public void OnNoCam(InputValue val)
        {
            Debug.Log($"NoCam: {val}");
            if (!val.isPressed)
                return;
            cameras[0].Priority = int.MinValue;
            cameras[1].Priority = int.MinValue;
        }
    }
}
