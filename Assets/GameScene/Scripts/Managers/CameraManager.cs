using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lore.Game.Managers
{
    public class CameraManager : BaseManager
    {
        #region Singleton
        private static CameraManager _instance;
        public static CameraManager Instance { get { return _instance; } }

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(this.gameObject);
            }
            else
            {
                _instance = this;
            }
        }
        #endregion

        [SerializeField] private List<Camera> cameras;
        [SerializeField] private Camera mainCamera = null;
        [SerializeField] private Camera fixedCamera = null;
        [SerializeField] private CinemachineFreeLook freeLookCamera;
        private int currentCameraIndex = 0;
        public Camera activeCamera { get; private set; }

        public void Start()
        {
            if (mainCamera == null)
            {
                currentCameraIndex = 0;
            }
            else
            {
                currentCameraIndex = cameras.IndexOf(mainCamera);
            }
            SelectMainCamera();
        }

        private int NextIndex()
        {
            return (currentCameraIndex + 1) % cameras.Count;
        }
        public void NextCamera()
        {
            cameras[NextIndex()].gameObject.SetActive(true);
            cameras[currentCameraIndex].gameObject.SetActive(false);
            currentCameraIndex++;
            activeCamera = cameras[currentCameraIndex];
        }
        public void CloseAllCameras()
        {
            for (int i=0; i<cameras.Count; i++)
            {
                cameras[i].gameObject.SetActive(false);
            }
        }


        public void SelectMainCamera()
        {
            if (mainCamera == null)
            {
                return;
            }
            CloseAllCameras();
            mainCamera.gameObject.SetActive(true);
            activeCamera = mainCamera;
        }
        public void SelectFixedCamera()
        {
            if (fixedCamera == null)
            {
                return;
            }
            CloseAllCameras();
            fixedCamera.gameObject.SetActive(true);
            activeCamera = fixedCamera;
        }
    }
}
