using EECustom.Customizations.Abilities.Managers;
using Enemies;
using UnityEngine;
using UnityEngine.Rendering;

namespace EECustom.Customizations.Abilities
{
    public class LightCustom : EnemyCustomBase, IEnemySpawnedEvent
    {
        public float Intensity { get; set; } = 4f;
        public Color Color { get; set; } = new Color(1.0f, 1.0f, 1.0f, 1.0f);
        public float Range { get; set; } = 7f;
        public LightShadowResolution ShadowResolution { get; set; } = LightShadowResolution.VeryHigh;
        public LightShadows Shadows { get; set; } = LightShadows.Soft;
        public LightShape Shape { get; set; } = LightShape.Cone;
        public LightType Type { get; set; } = LightType.Spot;
        public float SpotAngle { get; set; } = 90.0f;
        public float InnerSpotAngle { get; set; } = 90.0f;
        public bool FollowEnemyRotation { get; set; } = true;

        public override string GetProcessName()
        {
            return "Light";
        }

        public void OnSpawned(EnemyAgent agent)
        {
            LightManager lightManager = agent.gameObject.AddComponent<LightManager>();
            lightManager.Setup(this, agent);
        }
    }
}
