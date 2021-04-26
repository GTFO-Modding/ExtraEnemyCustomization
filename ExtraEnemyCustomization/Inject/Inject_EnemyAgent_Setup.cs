﻿using Enemies;
using HarmonyLib;

namespace ExtraEnemyCustomization.Inject
{
    [HarmonyPatch(typeof(EnemyAgent), nameof(EnemyAgent.Setup))]
    internal static class Inject_EnemyAgent_Setup
    {
        private static void Postfix(EnemyAgent __instance)
        {
            if (__instance.name.EndsWith(")")) //No Replicator Number = Fake call
            {
                return;
            }
            ConfigManager.Current.Customize_Postspawn(__instance);
        }
    }
}