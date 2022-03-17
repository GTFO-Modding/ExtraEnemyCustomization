﻿using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using UnityEngine;

namespace EECustom.Events.Inject
{
    [HarmonyPatch(typeof(ProjectileBase), nameof(ProjectileBase.Collision))]
    internal static class Inject_ProjectileBase
    {
        [HarmonyWrapSafe]
        internal static void Prefix(ProjectileBase __instance, RaycastHit hit)
        {
            if (hit.collider == null)
                return;

            if (!hit.collider.gameObject.TryGetComp<IDamageable>(out var damagable))
            {
                ProjectileEvents.OnCollisionWorld(__instance, hit.collider.gameObject);
                return;
            }

            var baseAgent = damagable.GetBaseAgent();
            if (baseAgent == null)
                return;

            if (!baseAgent.TryCastToPlayerAgent(out var playerAgent))
                return;

            ProjectileEvents.OnCollisionPlayer(__instance, playerAgent);
            if (__instance.TryGetOwner(out var owner))
            {
                if (owner == null)
                    return;

                if (owner.WasCollected)
                    return;

                LocalPlayerDamageEvents.OnProjectileDamage(playerAgent, owner, __instance, __instance.m_maxDamage);
            }
        }
    }
}
