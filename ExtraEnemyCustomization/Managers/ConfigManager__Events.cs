﻿using EECustom.Customizations;
using Enemies;
using System;
using System.Collections.Generic;
using System.Text;

namespace EECustom.Managers
{
    public partial class ConfigManager
    {
        private EnemyCustomBase[] _EnemyPrefabBuiltEvents = new EnemyCustomBase[0];
        private EnemyCustomBase[] _EnemySpawnedEvents = new EnemyCustomBase[0];
        private EnemyCustomBase[] _EnemyDespawnedEvents = new EnemyCustomBase[0];
        private EnemyCustomBase[] _EnemyGlowEvents = new EnemyCustomBase[0];

        private void GenerateEventBuffer()
        {
            List<EnemyCustomBase>
                prefabBuilt = new List<EnemyCustomBase>(),
                spawned = new List<EnemyCustomBase>(),
                despawned = new List<EnemyCustomBase>(),
                glow = new List<EnemyCustomBase>();

            foreach (var custom in _CustomizationBuffer)
            {
                if (custom is IEnemyPrefabBuiltEvent)
                    prefabBuilt.Add(custom);

                if (custom is IEnemySpawnedEvent)
                    spawned.Add(custom);

                if (custom is IEnemyDespawnedEvent)
                    despawned.Add(custom);

                if (custom is IEnemyGlowEvent)
                    glow.Add(custom);
            }

            _EnemyPrefabBuiltEvents = prefabBuilt.ToArray();
            _EnemySpawnedEvents = spawned.ToArray();
            _EnemyDespawnedEvents = despawned.ToArray();
            _EnemyGlowEvents = glow.ToArray();
        }

        internal void FirePrefabBuiltEvent(EnemyAgent agent)
        {
            for (int i = 0; i < _EnemyPrefabBuiltEvents.Length; i++)
            {
                var custom = _EnemyPrefabBuiltEvents[i];

                if (!custom.Enabled)
                    continue;

                if (custom.Target.IsMatch(agent))
                {
                    custom.LogDev($"Apply PrefabBuilt Event: {agent.name}");
                    ((IEnemyPrefabBuiltEvent)custom).OnPrefabBuilt(agent);
                    custom.LogVerbose($"Finished!");
                }
            }
        }

        internal void FireSpawnedEvent(EnemyAgent agent)
        {
            for (int i = 0; i < _EnemySpawnedEvents.Length; i++)
            {
                var custom = _EnemySpawnedEvents[i];

                if (!custom.Enabled)
                    continue;

                if (custom.Target.IsMatch(agent))
                {
                    custom.LogDev($"Apply Spawned Event: {agent.name}");
                    ((IEnemySpawnedEvent)custom).OnSpawned(agent);
                    custom.LogVerbose($"Finished!");
                }
            }
        }

        internal void FireDespawnedEvent(EnemyAgent agent)
        {
            for (int i = 0; i < _EnemyDespawnedEvents.Length; i++)
            {
                var custom = _EnemyDespawnedEvents[i];

                if (!custom.Enabled)
                    continue;

                if (custom.Target.IsMatch(agent))
                {
                    custom.LogDev($"Apply Despawned Event: {agent.name}");
                    ((IEnemyDespawnedEvent)custom).OnDespawned(agent);
                    custom.LogVerbose($"Finished!");
                }
            }
        }

        internal bool FireGlowEvent(EnemyAgent agent, ref GlowInfo glowInfo)
        {
            bool altered = false;

            for (int i = 0; i < _EnemyGlowEvents.Length; i++)
            {
                var custom = _EnemyGlowEvents[i];

                if (!custom.Enabled)
                    continue;

                if (custom.Target.IsMatch(agent))
                {
                    var newGlowInfo = new GlowInfo(glowInfo.Color, glowInfo.Position);
                    if (((IEnemyGlowEvent)custom).OnGlow(agent, ref newGlowInfo))
                    {
                        glowInfo = newGlowInfo;
                        altered = true;
                    }
                }
            }

            return altered;
        }
    }
}