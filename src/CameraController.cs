using UnityEngine;

namespace SnowsCameraMod
{
    public enum CameraMode
    {
        FirstPerson,
        ThirdPerson,
        Freecam,
        Spectator
    }

    public sealed class CameraController : MonoBehaviour
    {
        public CameraMode Mode { get; private set; } = CameraMode.ThirdPerson;
        public float FieldOfView = 80f;
        public float Smoothness = 0.16f;
        public float NearClip = 0.03f;
        public float ThirdPersonDistance = 3.25f;
        public float ThirdPersonHeight = 0.25f;
        public float OrbitSensitivity = 2.2f;
        public bool Collision = true;
        public bool AutoOrbit;
        public float AutoOrbitSpeed = 16f;

        public TrackedPlayer SelectedPlayer { get; private set; }
        public Camera ActiveCamera => camera;

        private Camera camera;
        private Transform cameraTransform;
        private Camera originalCamera;
        private bool originalCameraEnabled;
        private Vector3 freeVelocity;
        private float yaw;
        private float pitch = 8f;

        private void Update()
        {
            EnsureCamera();
            if (Plugin.Instance == null || Plugin.Instance.Players == null) return;

            if (SelectedPlayer == null || SelectedPlayer.Root == null)
                SelectedPlayer = Plugin.Instance.Players.GetLocal();

            if (Mode == CameraMode.Freecam) UpdateFreecam();
            else UpdateTrackedCamera();

            if (camera != null)
            {
                camera.fieldOfView = Mathf.Clamp(FieldOfView, 30f, 170f);
                camera.nearClipPlane = Mathf.Clamp(NearClip, 0.005f, 1f);
                camera.farClipPlane = 1000f;
                camera.depth = 100f;
            }
        }

        public void CycleMode()
        {
            Mode = (CameraMode)(((int)Mode + 1) % 4);
            ResetSmoothing();
        }

        public void SetMode(CameraMode mode)
        {
            Mode = mode;
            ResetSmoothing();
        }

        public void Select(TrackedPlayer player)
        {
            if (player == null) return;
            SelectedPlayer = player;
            ResetSmoothing();
        }

        public void SelectRelative(int amount)
        {
            var list = Plugin.Instance.Players.Players;
            if (list == null || list.Count == 0) return;

            int index = -1;
            for (int i = 0; i < list.Count; i++)
                if (list[i] == SelectedPlayer) index = i;

            if (index < 0) index = 0;
            index = (index + amount) % list.Count;
            if (index < 0) index += list.Count;
            Select(list[index]);
        }

        public void ResetToLocal()
        {
            SelectedPlayer = Plugin.Instance.Players.GetLocal();
            Mode = CameraMode.ThirdPerson;
            yaw = 0f;
            pitch = 8f;
            ResetSmoothing();
        }

        private void EnsureCamera()
        {
            if (camera != null) return;

            originalCamera = Camera.main;
            GameObject go = new GameObject("SnowCamera");
            DontDestroyOnLoad(go);

            camera = go.AddComponent<Camera>();
            cameraTransform = go.transform;
            camera.enabled = true;
            camera.clearFlags = CameraClearFlags.Skybox;
            camera.depth = 100f;

            if (originalCamera != null && originalCamera != camera)
            {
                originalCameraEnabled = originalCamera.enabled;
                originalCamera.enabled = false;
            }
        }

        private void UpdateTrackedCamera()
        {
            if (SelectedPlayer == null || SelectedPlayer.Root == null)
            {
                SelectedPlayer = Plugin.Instance.Players.GetLocal();
                if (SelectedPlayer == null) return;
            }

            Transform head = SelectedPlayer.Head != null ? SelectedPlayer.Head : SelectedPlayer.Root.transform;
            Transform body = SelectedPlayer.Body != null ? SelectedPlayer.Body : SelectedPlayer.Root.transform;

            if (Mode == CameraMode.FirstPerson)
            {
                SmoothTransform(head.position, head.rotation);
                return;
            }

            Vector3 center = body.position + Vector3.up * ThirdPersonHeight;
            if (AutoOrbit) yaw += AutoOrbitSpeed * Time.unscaledDeltaTime;

            Quaternion orbit = Quaternion.Euler(pitch, yaw, 0f);
            Vector3 desired = center + orbit * new Vector3(0f, 0f, -ThirdPersonDistance);

            if (Collision)
            {
                Vector3 direction = desired - center;
                float distance = direction.magnitude;
                if (distance > 0.01f && Physics.SphereCast(center, 0.16f, direction.normalized, out RaycastHit hit, distance, ~0, QueryTriggerInteraction.Ignore))
                    desired = center + direction.normalized * Mathf.Max(0.1f, hit.distance - 0.08f);
            }

            Quaternion look = Quaternion.LookRotation((center - desired).normalized, Vector3.up);
            SmoothTransform(desired, look);
        }

        private void UpdateFreecam()
        {
            float dt = Time.unscaledDeltaTime;

            if (Input.GetMouseButton(1))
            {
                yaw += Input.GetAxis("Mouse X") * OrbitSensitivity * 4f;
                pitch -= Input.GetAxis("Mouse Y") * OrbitSensitivity * 4f;
                pitch = Mathf.Clamp(pitch, -89f, 89f);
            }

            Quaternion rotation = Quaternion.Euler(pitch, yaw, 0f);
            cameraTransform.rotation = rotation;

            Vector3 input = Vector3.zero;
            if (Input.GetKey(KeyCode.W)) input += Vector3.forward;
            if (Input.GetKey(KeyCode.S)) input += Vector3.back;
            if (Input.GetKey(KeyCode.A)) input += Vector3.left;
            if (Input.GetKey(KeyCode.D)) input += Vector3.right;
            if (Input.GetKey(KeyCode.E)) input += Vector3.up;
            if (Input.GetKey(KeyCode.Q)) input += Vector3.down;

            float speed = Input.GetKey(KeyCode.LeftShift) ? 14f : 5f;
            Vector3 desired = rotation * input.normalized * speed;
            freeVelocity = Vector3.Lerp(freeVelocity, desired, 1f - Mathf.Exp(-12f * dt));
            cameraTransform.position += freeVelocity * dt;
        }

        private void SmoothTransform(Vector3 targetPosition, Quaternion targetRotation)
        {
            float blend = 1f - Mathf.Exp(-Mathf.Max(0.001f, Smoothness) * 60f * Time.unscaledDeltaTime);
            cameraTransform.position = Vector3.Lerp(cameraTransform.position, targetPosition, blend);
            cameraTransform.rotation = Quaternion.Slerp(cameraTransform.rotation, targetRotation, blend);
        }

        public void Orbit(float horizontal, float vertical)
        {
            yaw += horizontal * OrbitSensitivity;
            pitch = Mathf.Clamp(pitch - vertical * OrbitSensitivity, -75f, 75f);
        }

        public void ResetSmoothing()
        {
            if (SelectedPlayer == null || cameraTransform == null) return;
            Transform target = SelectedPlayer.Head != null ? SelectedPlayer.Head : SelectedPlayer.Root != null ? SelectedPlayer.Root.transform : null;
            if (target != null)
            {
                cameraTransform.position = target.position;
                cameraTransform.rotation = target.rotation;
            }
            freeVelocity = Vector3.zero;
        }

        private void OnDestroy()
        {
            if (originalCamera != null) originalCamera.enabled = originalCameraEnabled;
            if (camera != null) Destroy(camera.gameObject);
        }
    }
}