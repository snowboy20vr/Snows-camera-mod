using System.Collections.Generic;
using UnityEngine;

namespace SnowsCameraMod
{
    public sealed class CameraTabletWorld : MonoBehaviour
    {
        public bool IsVisible { get; private set; }
        public Transform TabletTransform { get; private set; }

        private GameObject tablet;
        private GameObject screen;
        private ParticleSystem trail;
        private Transform dock;
        private Transform localHead;
        private float nextTrailBurst;
        private float nextDockScan;

        private void Start()
        {
            BuildTablet();
            FindDock();
            IsVisible = false;
            tablet.SetActive(false);
        }

        private void Update()
        {
            if (Plugin.Instance == null) return;

            if (dock != null && !dock.gameObject.activeSelf && Time.unscaledTime >= nextDockScan)
            {
                nextDockScan = Time.unscaledTime + 2f;
                FindDock();
            }

            if (Plugin.Instance.TabletButtonPressed())
                Toggle();

            if (!IsVisible) return;

            UpdateFollowPosition();
            EmitSnowTrail();
        }

        public void Toggle()
        {
            if (tablet == null) return;

            IsVisible = !IsVisible;
            tablet.SetActive(IsVisible);
            if (Plugin.Instance.UI != null && Plugin.Instance.UI.enabled)
                Plugin.Instance.UI.Toggle();

            if (IsVisible)
            {
                FindLocalHead();
                UpdateFollowPosition(true);
            }
        }

        private void BuildTablet()
        {
            tablet = new GameObject("Snow's Camera Tablet");
            DontDestroyOnLoad(tablet);
            TabletTransform = tablet.transform;

            GameObject body = GameObject.CreatePrimitive(PrimitiveType.Cube);
            body.name = "TabletBody";
            body.transform.SetParent(tablet.transform, false);
            body.transform.localScale = new Vector3(0.34f, 0.025f, 0.23f);
            body.transform.localPosition = Vector3.zero;

            Renderer bodyRenderer = body.GetComponent<Renderer>();
            bodyRenderer.material = MakeMaterial(new Color(0.025f, 0.03f, 0.04f, 1f));

            screen = GameObject.CreatePrimitive(PrimitiveType.Cube);
            screen.name = "TabletScreen";
            screen.transform.SetParent(tablet.transform, false);
            screen.transform.localScale = new Vector3(0.27f, 0.006f, 0.16f);
            screen.transform.localPosition = new Vector3(0f, 0.016f, 0f);
            screen.GetComponent<Renderer>().material = MakeMaterial(new Color(0.04f, 0.07f, 0.10f, 1f));

            GameObject accent = GameObject.CreatePrimitive(PrimitiveType.Cube);
            accent.name = "OrangeAccent";
            accent.transform.SetParent(tablet.transform, false);
            accent.transform.localScale = new Vector3(0.025f, 0.008f, 0.16f);
            accent.transform.localPosition = new Vector3(-0.14f, 0.021f, 0f);
            accent.GetComponent<Renderer>().material = MakeMaterial(new Color(1f, 0.48f, 0.06f, 1f));

            CreateSnowflakeLogo();

            trail = tablet.AddComponent<ParticleSystem>();
            var main = trail.main;
            main.loop = true;
            main.playOnAwake = true;
            main.startLifetime = 0.8f;
            main.startSpeed = 0.15f;
            main.startSize = 0.018f;
            main.startColor = new Color(1f, 1f, 1f, 0.85f);
            main.maxParticles = 100;

            var emission = trail.emission;
            emission.rateOverTime = 18f;

            var shape = trail.shape;
            shape.shapeType = ParticleSystemShapeType.Circle;
            shape.radius = 0.12f;

            var velocity = trail.velocityOverLifetime;
            velocity.enabled = true;
            velocity.space = ParticleSystemSimulationSpace.Local;
            velocity.y = -0.12f;

            var renderer = trail.GetComponent<ParticleSystemRenderer>();
            renderer.material = MakeMaterial(new Color(1f, 1f, 1f, 0.9f));

            CreateDockMarker();
        }

        private void CreateSnowflakeLogo()
        {
            GameObject logo = new GameObject("SnowflakeLogo");
            logo.transform.SetParent(tablet.transform, false);
            logo.transform.localPosition = new Vector3(0f, 0.022f, 0.075f);

            TextMesh text = logo.AddComponent<TextMesh>();
            text.text = "❄";
            text.fontSize = 64;
            text.characterSize = 0.012f;
            text.anchor = TextAnchor.MiddleCenter;
            text.alignment = TextAlignment.Center;
            text.color = new Color(1f, 1f, 1f, 0.95f);
        }

        private void CreateDockMarker()
        {
            GameObject marker = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            marker.name = "SnowCameraTabletDock";
            marker.transform.localScale = new Vector3(0.14f, 0.006f, 0.14f);
            marker.GetComponent<Renderer>().material = MakeMaterial(new Color(1f, 0.48f, 0.06f, 0.7f));
            dock = marker.transform;
            marker.SetActive(false);
        }

        private void FindDock()
        {
            GameObject[] objects = Resources.FindObjectsOfTypeAll<GameObject>();
            foreach (GameObject go in objects)
            {
                if (go == null || !go.scene.IsValid()) continue;
                string n = go.name.ToLowerInvariant();
                if (n.Contains("stump") || n.Contains("treehouse"))
                {
                    dock.position = go.transform.position + go.transform.up * 0.55f;
                    dock.rotation = go.transform.rotation;
                    dock.gameObject.SetActive(true);
                    return;
                }
            }

            dock.position = Vector3.zero;
        }

        private void FindLocalHead()
        {
            TrackedPlayer local = Plugin.Instance.Players.GetLocal();
            localHead = local != null ? local.Head : null;
        }

        private void UpdateFollowPosition(bool snap = false)
        {
            if (localHead == null) FindLocalHead();
            if (localHead == null) return;

            Vector3 target = localHead.position + localHead.forward * 0.65f - localHead.up * 0.25f;
            Quaternion rotation = Quaternion.LookRotation(localHead.forward, Vector3.up) * Quaternion.Euler(72f, 0f, 0f);

            if (snap)
            {
                TabletTransform.position = target;
                TabletTransform.rotation = rotation;
            }
            else
            {
                TabletTransform.position = Vector3.Lerp(TabletTransform.position, target, 1f - Mathf.Exp(-14f * Time.unscaledDeltaTime));
                TabletTransform.rotation = Quaternion.Slerp(TabletTransform.rotation, rotation, 1f - Mathf.Exp(-14f * Time.unscaledDeltaTime));
            }
        }

        private void EmitSnowTrail()
        {
            if (trail == null || Time.unscaledTime < nextTrailBurst) return;
            nextTrailBurst = Time.unscaledTime + 0.08f;
            trail.Emit(2);
        }

        private Material MakeMaterial(Color color)
        {
            Shader shader = Shader.Find("Sprites/Default");
            if (shader == null) shader = Shader.Find("Unlit/Color");
            Material material = new Material(shader);
            material.color = color;
            return material;
        }
    }
}