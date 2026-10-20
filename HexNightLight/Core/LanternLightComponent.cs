using UnityEngine;

namespace HexNightLight.Core
{
    public class LanternLightComponent : MonoBehaviour
    {
        private Light _light;

        private void Awake()
        {
            _light = GetComponent<Light>();

            if (_light == null)
            {
                return;
            }

            ApplySettings();
        }

        private void OnEnable()
        {
            Plugin.LanternLightRange.SettingChanged += OnSettingChanged;
            Plugin.LanternLightIntensity.SettingChanged += OnSettingChanged;
            Plugin.LanternLightShadows.SettingChanged += OnSettingChanged;

            ApplySettings();
        }

        private void OnDisable()
        {
            Plugin.LanternLightRange.SettingChanged -= OnSettingChanged;
            Plugin.LanternLightIntensity.SettingChanged -= OnSettingChanged;
            Plugin.LanternLightShadows.SettingChanged -= OnSettingChanged;
        }

        private void OnSettingChanged(object sender, System.EventArgs e)
        {
            ApplySettings();
        }

        private void ApplySettings()
        {
            if (_light == null)
            {
                return;
            }

            _light.range = Plugin.LanternLightRange.Value;
            _light.intensity = Plugin.LanternLightIntensity.Value;
            _light.shadows = Plugin.LanternLightShadows.Value;
        }
    }
}