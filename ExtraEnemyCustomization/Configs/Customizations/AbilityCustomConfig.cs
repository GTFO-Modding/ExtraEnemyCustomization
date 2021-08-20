﻿using EECustom.Customizations;
using EECustom.Customizations.Abilities;
using System.Collections.Generic;

namespace EECustom.Configs.Customizations
{
    public class AbilityCustomConfig : CustomizationConfig
    {
        public LightCustom[] LightCustom { get; set; } = new LightCustom[0];
        public BirthingCustom[] BirthingCustom { get; set; } = new BirthingCustom[0];
        public FogSphereCustom[] FogSphereCustom { get; set; } = new FogSphereCustom[0];
        public HealthRegenCustom[] HealthRegenCustom { get; set; } = new HealthRegenCustom[0];
        public InfectionAttackCustom[] InfectionAttackCustom { get; set; } = new InfectionAttackCustom[0];

        public override EnemyCustomBase[] GetAllSettings()
        {
            var list = new List<EnemyCustomBase>();
            list.AddRange(LightCustom);
            list.AddRange(BirthingCustom);
            list.AddRange(FogSphereCustom);
            list.AddRange(HealthRegenCustom);
            list.AddRange(InfectionAttackCustom);
            return list.ToArray();
        }
    }
}