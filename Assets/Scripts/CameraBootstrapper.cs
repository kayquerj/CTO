using Cinemachine;
using UnityEngine;

namespace HollowKnightLike.CameraSystem
{
    [DefaultExecutionOrder(-10)]
    public class CameraBootstrapper : MonoBehaviour
    {
        [SerializeField] private Transform followTarget;
        [SerializeField] private Collider2D levelBounds;
        [SerializeField] private float deadZoneWidth = 0.2f;
        [SerializeField] private float deadZoneHeight = 0.15f;
        [SerializeField] private float damping = 0.2f;

        private CinemachineVirtualCamera _virtualCamera;

        private void Awake()
        {
            var brain = GetComponent<CinemachineBrain>();
            if (brain == null)
            {
                brain = gameObject.AddComponent<CinemachineBrain>();
            }

            EnsureVirtualCamera();
        }

        public void EnsureVirtualCamera()
        {
            if (_virtualCamera == null)
            {
                var existing = transform.Find("VirtualCamera");
                GameObject vcamGameObject;
                if (existing != null)
                {
                    vcamGameObject = existing.gameObject;
                }
                else
                {
                    vcamGameObject = new GameObject("VirtualCamera");
                    vcamGameObject.transform.SetParent(transform, false);
                }

                _virtualCamera = vcamGameObject.GetComponent<CinemachineVirtualCamera>();
                if (_virtualCamera == null)
                {
                    _virtualCamera = vcamGameObject.AddComponent<CinemachineVirtualCamera>();
                }
            }

            _virtualCamera.Follow = followTarget;

            var framing = _virtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
            if (framing == null)
            {
                framing = _virtualCamera.AddCinemachineComponent<CinemachineFramingTransposer>();
            }

            framing.m_XDamping = damping;
            framing.m_YDamping = damping;
            framing.m_ZDamping = damping;
            framing.m_DeadZoneWidth = deadZoneWidth;
            framing.m_DeadZoneHeight = deadZoneHeight;
            framing.m_SoftZoneWidth = 0.8f;
            framing.m_SoftZoneHeight = 0.8f;

            var confiner = _virtualCamera.GetComponent<CinemachineConfiner2D>();
            if (confiner == null)
            {
                confiner = _virtualCamera.gameObject.AddComponent<CinemachineConfiner2D>();
            }

            confiner.m_BoundingShape2D = levelBounds as Collider2D;
            confiner.m_ConfineMode = CinemachineConfiner2D.Mode.Confine3D;
            confiner.m_Damping = damping;
        }

        public void SetFollowTarget(Transform target)
        {
            followTarget = target;
            if (_virtualCamera == null)
            {
                EnsureVirtualCamera();
            }
            else
            {
                _virtualCamera.Follow = target;
            }
        }

        public void SetBounds(Collider2D bounds)
        {
            levelBounds = bounds;
            if (_virtualCamera == null)
            {
                return;
            }

            var confiner = _virtualCamera.GetComponent<CinemachineConfiner2D>();
            if (confiner != null)
            {
                confiner.m_BoundingShape2D = bounds;
            }
        }
    }
}
