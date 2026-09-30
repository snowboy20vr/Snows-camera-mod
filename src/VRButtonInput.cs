using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

namespace SnowsCameraMod
{
    public sealed class VRButtonInput : MonoBehaviour
    {
        private readonly List<InputDevice> devices = new List<InputDevice>();
        private bool lastPressed;

        public bool YPressedThisFrame()
        {
            InputDevices.GetDevicesWithCharacteristics(
                InputDeviceCharacteristics.Left | InputDeviceCharacteristics.Controller | InputDeviceCharacteristics.HeldInHand,
                devices);

            bool pressed = false;
            for (int i = 0; i < devices.Count; i++)
            {
                bool value;
                if (devices[i].TryGetFeatureValue(CommonUsages.secondaryButton, out value) && value)
                    pressed = true;
            }

            InputDevices.GetDevicesWithCharacteristics(
                InputDeviceCharacteristics.Right | InputDeviceCharacteristics.Controller | InputDeviceCharacteristics.HeldInHand,
                devices);

            for (int i = 0; i < devices.Count; i++)
            {
                bool value;
                if (devices[i].TryGetFeatureValue(CommonUsages.primaryButton, out value) && value)
                    pressed = true;
            }

            bool rising = pressed && !lastPressed;
            lastPressed = pressed;
            return rising;
        }
    }
}