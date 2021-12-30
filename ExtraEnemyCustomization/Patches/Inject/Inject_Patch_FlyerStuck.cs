﻿using EECustom.Events;
using EECustom.Managers;
using EECustom.Patches.Handlers;
using Enemies;
using HarmonyLib;
using SNetwork;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace EECustom.Patches.Inject
{
    [HarmonyPatch(typeof(EnemyAgent), nameof(EnemyAgent.Setup))]
    internal static class Inject_Patch_FlyerStuck
    {
        public static void Postfix(EnemyAgent __instance)
        {
            if (!ConfigManager.Current.Global.UsingFlyerStuckCheck)
                return;

            if (!SNet.IsMaster)
                return;

            if (!__instance.EnemyBehaviorData.IsFlyer)
                return;

            var flyerHandler = __instance.gameObject.AddComponent<FlyerStuckHandler>();
            flyerHandler.Agent = __instance;
            flyerHandler.UpdateInterval = ConfigManager.Current.Global.FlyerStuck_Interval;
            flyerHandler.RetryCount = ConfigManager.Current.Global.FlyerStuck_Retry;
        }
    }
}