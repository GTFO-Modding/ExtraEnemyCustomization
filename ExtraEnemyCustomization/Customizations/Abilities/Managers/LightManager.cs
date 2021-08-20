using Enemies;
using System;
using UnityEngine;

namespace EECustom.Customizations.Abilities.Managers
{
    public class LightManager : MonoBehaviour
    {
        public LightManager(IntPtr ptr) : base(ptr) { }
        public void Setup(LightCustom lightCustom, EnemyAgent agent)
        {
            m_targetAgent = agent;
            m_lightObject = new GameObject();
            m_lightObject.transform.SetParent(m_targetAgent.m_headLimb.transform);
            var light = m_lightObject.AddComponent<Light>();
            light.intensity = lightCustom.Intensity;
            light.color = lightCustom.Color;
            light.range = lightCustom.Range;
            light.shadowResolution = lightCustom.ShadowResolution;
            light.shadows = lightCustom.Shadows;
            light.shape = lightCustom.Shape;
            light.type = lightCustom.Type;
            light.spotAngle = lightCustom.SpotAngle;
            light.innerSpotAngle = lightCustom.InnerSpotAngle;
            m_followRotation = lightCustom.FollowEnemyRotation;
            light.transform.SetParent(m_lightObject.transform);
            light.transform.localPosition = Vector3.zero;
            light.transform.localRotation = Quaternion.identity;
        }

        private void Update()
        {
            if (m_followRotation && m_targetAgent != null && m_targetAgent.Alive)
                m_lightObject.transform.rotation = m_targetAgent.gameObject.transform.rotation;
        }

        private void OnDestroy()
        {
            if (m_lightObject != null)
                Destroy(m_lightObject);
        }
        private bool m_followRotation = false;
        private EnemyAgent m_targetAgent;
        private GameObject m_lightObject;
    }
}
