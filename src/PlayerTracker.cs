using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SnowsCameraMod
{
    public sealed class TrackedPlayer
    {
        public GameObject Root;
        public Transform Head;
        public Transform Body;
        public string Name;
        public bool IsLocal;
        public string DisplayName
        {
            get { return string.IsNullOrWhiteSpace(Name) ? (Root != null ? Root.name : "Unknown Player") : Name; }
        }
    }

    public sealed class PlayerTracker : MonoBehaviour
    {
        private readonly List<TrackedPlayer> players = new List<TrackedPlayer>();
        private float nextScan;
        public IReadOnlyList<TrackedPlayer> Players => players;

        private void Update()
        {
            if (Time.unscaledTime < nextScan) return;
            nextScan = Time.unscaledTime + 1.0f;
            Scan();
        }

        public void Scan()
        {
            Dictionary<int, TrackedPlayer> found = new Dictionary<int, TrackedPlayer>();

            foreach (GameObject go in Resources.FindObjectsOfTypeAll<GameObject>())
            {
                if (go == null || !go.activeInHierarchy || !RuntimeReflection.LooksLikePlayerObject(go))
                    continue;

                Transform root = go.transform;
                TrackedPlayer player = new TrackedPlayer
                {
                    Root = go,
                    Head = RuntimeReflection.FindBestHead(root),
                    Body = RuntimeReflection.FindBestBody(root),
                    Name = RuntimeReflection.GetString(go, "Name", "name", "NickName", "Nickname", "PlayerName", "playerName", "Username", "DisplayName", "displayName")
                };

                object local = RuntimeReflection.GetMember(go, "isLocal");
                if (local is bool b) player.IsLocal = b;
                object localPlayer = RuntimeReflection.GetMember(go, "isLocalPlayer");
                if (localPlayer is bool b2) player.IsLocal = b2;

                found[go.GetInstanceID()] = player;
            }

            players.Clear();
            players.AddRange(found.Values.OrderByDescending(p => p.IsLocal).ThenBy(p => p.DisplayName, StringComparer.OrdinalIgnoreCase));

            if (!players.Any(p => p.IsLocal))
            {
                Type type = RuntimeReflection.FindType("GorillaLocomotion.Player");
                if (type != null)
                {
                    object instance = RuntimeReflection.GetStaticMember(type, "Instance");
                    if (instance is Component component)
                    {
                        players.Insert(0, new TrackedPlayer
                        {
                            Root = component.gameObject,
                            Body = component.transform,
                            Head = RuntimeReflection.FindBestHead(component.transform),
                            Name = "You",
                            IsLocal = true
                        });
                    }
                }
            }
        }

        public TrackedPlayer GetLocal()
        {
            return players.FirstOrDefault(p => p.IsLocal) ?? players.FirstOrDefault();
        }
    }
}