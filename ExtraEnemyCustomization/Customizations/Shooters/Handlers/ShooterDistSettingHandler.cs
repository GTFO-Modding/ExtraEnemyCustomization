﻿using EECustom.Attributes;
using System.Linq;
using UnityEngine;

namespace EECustom.Customizations.Shooters.Handlers
{
    [InjectToIl2Cpp]
    public sealed class ShooterDistSettingHandler : MonoBehaviour
    {
        public EAB_ProjectileShooter DefaultValue;
        public EAB_ProjectileShooter EAB_Shooter;
        public FireSetting[] FireSettings;

        private FireSetting _currentSetting = null;
        private float _timerToUpdate = 0.0f;

        internal void Update()
        {
            if (Clock.Time < _timerToUpdate)
                return;

            if (EAB_Shooter.m_owner.Locomotion.CurrentStateEnum == Enemies.ES_StateEnum.ShooterAttack)
                return;

            _timerToUpdate = Clock.Time + 0.125f;

            if (!EAB_Shooter.m_owner.AI.IsTargetValid)
                return;

            var distance = EAB_Shooter.m_owner.AI.Target.m_distance;
            var newSetting = FireSettings.FirstOrDefault(x => x.FromDistance <= distance);
            if (newSetting != _currentSetting)
            {
                newSetting.ApplyToEAB(EAB_Shooter, DefaultValue);
                _currentSetting = newSetting;
            }
        }

        internal void OnDestroy()
        {
            Destroy(DefaultValue);
            DefaultValue = null;

            EAB_Shooter = null;
            FireSettings = null;
            _currentSetting = null;
        }
    }
}