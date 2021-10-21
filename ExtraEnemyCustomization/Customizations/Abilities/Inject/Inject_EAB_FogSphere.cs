﻿using EECustom.Customizations.Abilities.Handlers;
using EECustom.Utils;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Text;

namespace EECustom.Customizations.Abilities.Inject
{
    [HarmonyPatch(typeof(EAB_FogSphere), nameof(EAB_FogSphere.DoTrigger))]
    internal class Inject_EAB_FogSphere
    {
        private static void Prefix(EAB_FogSphere __instance)
        {
            var effectSetting = EnemyProperty<SphereEffectSetting>.Get(__instance.m_owner);
            if (effectSetting == null)
                return;

            effectSetting.HandlerCount = __instance.m_activeFogSpheres.Count;
        }

        private static void Postfix(EAB_FogSphere __instance)
        {
            var effectSetting = EnemyProperty<SphereEffectSetting>.Get(__instance.m_owner);
            if (effectSetting == null)
                return;

            if (effectSetting.HandlerCount == __instance.m_activeFogSpheres.Count)
                return;

            var handler = __instance.m_activeFogSpheres[__instance.m_activeFogSpheres.Count - 1];
            if (handler.gameObject.GetComponent<EffectFogSphereHandler>() != null)
                return;

            var effectHandler = handler.gameObject.AddComponent<EffectFogSphereHandler>();
            effectHandler.Handler = handler;
            effectHandler.EVSphere = new EV_Sphere()
            {
                position = handler.transform.position,
                minRadius = 0.0f,
                maxRadius = 0.0f,
                modificationScale = effectSetting.EffectScale,
                invert = true,
                contents = effectSetting.EffectContents,
                modification = effectSetting.EffectModification
            };
            EffectVolumeManager.RegisterVolume(effectHandler.EVSphere);
        }
    }
}