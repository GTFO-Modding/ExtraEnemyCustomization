﻿using EECustom.Customizations;
using EECustom.Customizations.Abilities;
using System.Collections.Generic;

namespace EECustom.Configs.Customizations
{
    public class AbilityCustomConfig : CustomizationConfig
    {
        public BirthingCustom[] BirthingCustom = new BirthingCustom[0];
        public FogSphereCustom[] FogSphereCustom = new FogSphereCustom[0];
        public HealthRegenCustom[] HealthRegenCustom = new HealthRegenCustom[0];
        public InfectionAttackCustom[] InfectionAttackCustom = new InfectionAttackCustom[0];

        public override EnemyCustomBase[] GetAllSettings()
        {
            var list = new List<EnemyCustomBase>();
            list.AddRange(BirthingCustom);
            list.AddRange(FogSphereCustom);
            list.AddRange(HealthRegenCustom);
            list.AddRange(InfectionAttackCustom);
            return list.ToArray();
        }
    }
}