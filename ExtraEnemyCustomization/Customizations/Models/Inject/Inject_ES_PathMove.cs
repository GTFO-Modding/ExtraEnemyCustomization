﻿using EECustom.Customizations.Models.Handlers;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Text;

namespace EECustom.Customizations.Models.Inject
{
    [HarmonyPatch(typeof(ES_PathMove), nameof(ES_PathMove.CommonEnter))]
    internal static class Inject_ES_PathMove
    {
        [HarmonyWrapSafe]
        internal static void Prefix(ES_PathMove __instance)
        {
            var scannerManager = __instance.m_enemyAgent.gameObject.GetComponent<ScannerHandler>();
            if (scannerManager != null && scannerManager.OwnerAgent != null)
            {
                __instance.m_enemyAgent.ScannerData.m_soundIndex = 0;
            }
        }
    }

    [HarmonyPatch(typeof(ES_PathMoveFlyer), nameof(ES_PathMoveFlyer.CommonEnter))]
    internal static class Inject_ES_PathMove_Flyer
    {
        [HarmonyWrapSafe]
        internal static void Prefix(ES_PathMoveFlyer __instance)
        {
            var scannerManager = __instance.m_enemyAgent.gameObject.GetComponent<ScannerHandler>();
            if (scannerManager != null && scannerManager.OwnerAgent != null)
            {
                __instance.m_enemyAgent.ScannerData.m_soundIndex = 0;
            }
        }
    }
}