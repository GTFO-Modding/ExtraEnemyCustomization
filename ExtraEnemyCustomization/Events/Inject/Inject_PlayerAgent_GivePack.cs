﻿using Gear;
using HarmonyLib;
using Player;

namespace EEC.Events.Inject
{
    [HarmonyPatch(typeof(PlayerAgent))]
    internal static class Inject_PlayerAgent_GivePack
    {
        [HarmonyPostfix]
        [HarmonyWrapSafe]
        [HarmonyPatch(nameof(PlayerAgent.GiveHealth))]
        internal static void Post_Health(float amountRel, PlayerAgent __instance)
        {
            ResourcePackEvents.OnReceiveMedi(__instance.Cast<iResourcePackReceiver>(), amountRel);
        }

        [HarmonyPostfix]
        [HarmonyWrapSafe]
        [HarmonyPatch(nameof(PlayerAgent.GiveAmmoRel))]
        internal static void Post_Ammo(float ammoStandardRel, float ammoSpecialRel, float ammoClassRel, PlayerAgent __instance)
        {
            if (ammoStandardRel > 0.0f || ammoSpecialRel > 0.0f)
                ResourcePackEvents.OnReceiveAmmo(__instance.Cast<iResourcePackReceiver>(), ammoStandardRel, ammoSpecialRel);
            if (ammoClassRel > 0.0f)
                ResourcePackEvents.OnReceiveTool(__instance.Cast<iResourcePackReceiver>(), ammoClassRel);
        }

        [HarmonyPostfix]
        [HarmonyWrapSafe]
        [HarmonyPatch(nameof(PlayerAgent.GiveDisinfection))]
        internal static void Post_Disinect(float amountRel, PlayerAgent __instance)
        {
            ResourcePackEvents.OnReceiveDisinfect(__instance.Cast<iResourcePackReceiver>(), amountRel);
        }
    }
}