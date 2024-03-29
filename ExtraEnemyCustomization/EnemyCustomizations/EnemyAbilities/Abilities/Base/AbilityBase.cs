﻿using BepInEx.Logging;
using EEC.Events;
using Enemies;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using UnityEngine;

namespace EEC.EnemyCustomizations.EnemyAbilities.Abilities
{
    public abstract class AbilityBase<T> : IAbility where T : AbilityBehaviour, new()
    {
        public string Name { get; set; } = string.Empty;

        private bool _isBehavioursDirty = true;
        private readonly Dictionary<ushort, AbilityBehaviour> _behaviourLookup = new();
        private readonly List<AbilityBehaviour> _behaviours = new();
        private AbilityBehaviour[] _behavioursCache = null;

        [JsonIgnore]
        public AbilityBehaviour[] Behaviours
        {
            get
            {
                if (_isBehavioursDirty)
                {
                    _behavioursCache = _behaviours.ToArray();
                    _isBehavioursDirty = false;
                }

                return _behavioursCache;
            }
        }

        public ushort SyncID { get; private set; }

        public void Setup(ushort syncID)
        {
            SyncID = syncID;
            OnAbilityLoaded();
            EnemyEvents.Despawn += EnemyDespawn;
        }

        private void EnemyDespawn(EnemyAgent agent)
        {
            if (_behaviourLookup.TryGetValue(agent.GlobalID, out var behaviour))
            {
                behaviour.Unload();
                _behaviourLookup.Remove(agent.GlobalID);
            }
        }

        public void Unload()
        {
            _behaviours.Clear();
            OnAbilityUnloaded();
            EnemyEvents.Despawn -= EnemyDespawn;
        }

        #region ABILITY CALLER

        public void TriggerSync(ushort enemyID)
        {
            EnemyAbilityManager.SendEvent(SyncID, enemyID, AbilityPacketType.DoTrigger);
        }

        public void Trigger(ushort enemyID)
        {
            if (TryGetBehaviour(enemyID, out var behaviour))
            {
                behaviour.DoTrigger();
            }
        }

        public void TriggerAllSync()
        {
            EnemyAbilityManager.SendEvent(SyncID, 0, AbilityPacketType.DoTriggerAll);
        }

        public void TriggerAll()
        {
            foreach (var behaviour in _behaviours)
            {
                behaviour.DoTrigger();
            }
        }

        public void ExitSync(ushort enemyID)
        {
            EnemyAbilityManager.SendEvent(SyncID, enemyID, AbilityPacketType.DoExit);
        }

        public void Exit(ushort enemyID)
        {
            if (TryGetBehaviour(enemyID, out var behaviour))
            {
                behaviour.DoExit();
            }
        }

        public void ExitAllSync()
        {
            EnemyAbilityManager.SendEvent(SyncID, 0, AbilityPacketType.DoExitAll);
        }

        public void ExitAll()
        {
            foreach (var behaviour in _behaviours)
            {
                behaviour.DoExit();
            }
        }

        #endregion ABILITY CALLER

        public AbilityBehaviour RegisterBehaviour(EnemyAgent agent)
        {
            var id = agent.GlobalID;

            if (_behaviourLookup.TryGetValue(id, out var cachedBehaviour))
            {
                return cachedBehaviour;
            }

            var behaviour = new T();
            behaviour.Setup(this, agent);
            _behaviours.Add(behaviour);
            _isBehavioursDirty = true;
            _behaviourLookup[id] = behaviour;

            OnBehaviourAssigned(agent, behaviour);

            return behaviour;
        }

        public virtual void OnAbilityLoaded()
        {
        }

        public virtual void OnAbilityUnloaded()
        {
        }

        public virtual void OnBehaviourAssigned(EnemyAgent agent, T behaviour)
        {
        }

        public bool TryGetBehaviour(ushort enemyID, out AbilityBehaviour behaviour)
        {
            return _behaviourLookup.TryGetValue(enemyID, out behaviour);
        }

        #region LOGGING

        public void LogVerbose(string str)
        {
            LogFormatDebug(str, true);
        }

        public void LogDev(string str)
        {
            LogFormatDebug(str, false);
        }

        public void LogError(string str)
        {
            LogFormat(LogLevel.Error, str);
        }

        public void LogWarning(string str)
        {
            LogFormat(LogLevel.Warning, str);
        }

        private void LogFormat(LogLevel level, string str)
        {
            Logger.LogInstance.Log(level, $"[{Name}] {str}");
        }

        private void LogFormatDebug(string str, bool verbose)
        {
            if (verbose)
                Logger.Verbose($"[{Name}] {str}");
            else
                Logger.Debug($"[{Name}] {str}");
        }

        #endregion LOGGING
    }
}