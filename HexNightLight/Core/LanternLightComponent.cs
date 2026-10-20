using UnityEngine;

namespace HexNightLight.Core
{
    public class LanternLightComponent : MonoBehaviour
    {
        private Light _light;
        private ZNetView _nview;
        private bool? _lastEnabled;
        private GameObject _flare;
        private Material _lanternMaterial;

        internal void Initialize(ZNetView nview)
        {
            _nview = nview;
            RefreshLightState();
        }

        internal void SetLightEnabled(bool enabled)
        {
            if (_light == null)
            {
                return;
            }

            _light.enabled = enabled;
        }

        internal void RefreshLightState()
        {
            if (_light == null || _nview == null)
            {
                return;
            }

            var zdo = _nview.GetZDO();

            if (zdo == null)
            {
                return;
            }

            var enabled = zdo.GetBool("HexNightLight_LightEnabled", true);

            if (_lastEnabled == enabled)
            {
                return;
            }

            _lastEnabled = enabled;
            _light.enabled = enabled;

            if (_lanternMaterial != null)
            {
                if (enabled)
                {
                    _lanternMaterial.EnableKeyword("_EMISSION");
                }
                else
                {
                    _lanternMaterial.DisableKeyword("_EMISSION");
                }
            }

            if (_flare != null)
            {
                _flare.SetActive(enabled);
            }
        }

        private void Awake()
        {
            _light = GetComponent<Light>();
            _flare = transform.parent.Find("flare")?.gameObject;

            if (_light == null)
            {
                return;
            }

            var renderer = transform.parent.GetComponent<MeshRenderer>();

            if (renderer != null)
            {
                _lanternMaterial = renderer.material;
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