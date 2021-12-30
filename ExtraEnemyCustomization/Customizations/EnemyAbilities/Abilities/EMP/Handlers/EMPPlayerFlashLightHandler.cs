﻿using EECustom.Events;
using GameData;
using Player;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace EECustom.Customizations.EnemyAbilities.Abilities.EMP.Handlers
{
    public class EMPPlayerFlashLightHandler : EMPHandlerBase
    {
        protected override bool IsDeviceOnPlayer => true;
        private PlayerInventoryBase _inventory;
        private bool FlashlightEnabled => _inventory.FlashlightEnabled;
        private float _originalIntensity;
        private bool _originalFlashlightState;
        public override void Setup(GameObject gameObject, EMPController controller)
        {
            _inventory = gameObject.GetComponent<PlayerAgent>().Inventory;
            if (_inventory == null)
            {
                Logger.Warning("Player inventory was null!");
                return;
            }
            _state = EMPState.On;
            InventoryEvents.ItemWielded += InventoryEvents_ItemWielded;
        }

        private void InventoryEvents_ItemWielded(Gear.GearPartFlashlight flashlight)
        {
            FlashlightSettingsDataBlock block = GameDataBlockBase<FlashlightSettingsDataBlock>.GetBlock(flashlight.m_settingsID);
            _originalIntensity = block.intensity;
        }

        protected override void DeviceOff()
        {
            _originalFlashlightState = FlashlightEnabled;

            if (_originalFlashlightState != false)
                _inventory.Owner.Sync.WantsToSetFlashlightEnabled(false);
        }

        protected override void DeviceOn()
        {
            if (_originalFlashlightState != FlashlightEnabled)
                _inventory.Owner.Sync.WantsToSetFlashlightEnabled(_originalFlashlightState);

            _inventory.m_flashlight.intensity = _originalIntensity;
        }

        protected override void FlickerDevice()
        {
            if (!FlashlightEnabled) return;
            _inventory.m_flashlight.intensity = UnityEngine.Random.value * _originalIntensity;
        }
    }
}