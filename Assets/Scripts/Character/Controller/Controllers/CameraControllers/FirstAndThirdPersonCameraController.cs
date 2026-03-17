using System;
using System.Collections.Generic;
using System.Linq;
using TrouGame.Miscellaneous;
using Unity.Cinemachine;
using UnityEngine;

namespace TrouGame.Character.Controller.Controllers.CameraControllers
{
    public class FirstAndThirdPersonCameraController : MonoBehaviour
    {
        [SerializeField] private CinemachineCamera firstPerson;
        [SerializeField] private CinemachineCamera thirdPerson;
        [SerializeField] private List<GameObject> HideInFirstPerson;

        public Action<Transform> OnRotationChange;

        private int hideLayer;
        private int defaultLayer;

        private void Start()
        {
            defaultLayer = LayerMask.NameToLayer("Default");
            hideLayer = LayerMask.NameToLayer("HideToMainCamera");
            SwitchCamera();
        }

        public void Rotate(Vector2 value)
        {
            transform.rotation = Quaternion.Euler(
                Clamp(transform.rotation.eulerAngles.x - value.y, -50f, 50f, 360f - 50f),
                transform.rotation.eulerAngles.y + value.x,
                0);

            OnRotationChange.Invoke(transform);
        }

        private float Clamp(float value, float minNeg, float max, float min)
        {
            if (value > 180f && value <= min)
                return min;
            if (value <= minNeg)
                return minNeg;
            if (value < 180f && value >= max)
                return max;
            return value;
        }

        public void SwitchCamera()
        {
            firstPerson.enabled = thirdPerson.enabled;
            thirdPerson.enabled = !thirdPerson.enabled;

            if (thirdPerson.enabled)
            {
                foreach (GameObject obj in HideInFirstPerson)
                    obj.layer = defaultLayer; 
            }
        }

        public void OnBlendFinished()
        {
            if (firstPerson.enabled)
            {
                foreach (GameObject obj in HideInFirstPerson)
                    obj.layer = hideLayer;
            }
        }
    }
}