﻿using EEC.Events;
using EEC.Managers;
using Enemies;
using HarmonyLib;

namespace EEC.Inject
{
    [HarmonyPatch(typeof(EnemyAgent), nameof(EnemyAgent.OnDeSpawn))]
    internal static class Inject_EnemyAgent_DeSpawn
    {
        [HarmonyWrapSafe]
        internal static void Prefix(EnemyAgent __instance)
        {
            EnemyEvents.OnDespawn(__instance);
        }

        [HarmonyWrapSafe]
        internal static void Postfix(EnemyAgent __instance)
        {
            EnemyEvents.OnDespawned(__instance);
            ConfigManager.FireDespawnedEvent(__instance);
        }
    }
}