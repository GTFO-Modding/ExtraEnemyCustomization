﻿using EEC.EnemyCustomizations.EnemyAbilities.Abilities;
using EEC.Events;
using EEC.Networking;
using SNetwork;
using System.Collections.Generic;

namespace EEC.EnemyCustomizations.EnemyAbilities
{
    [CallConstructorOnLoad]
    public static class EnemyAbilityManager
    {
        private static ushort _syncIDBuffer = 1;

        private static readonly AbilityEvent _abilityEvent = new();
        private static readonly Dictionary<string, IAbility> _abilities = new();
        private static readonly Dictionary<ushort, IAbility> _abilityIDLookup = new();
        private static bool _allAssetLoaded = false;

        static EnemyAbilityManager()
        {
            _abilityEvent.Setup();
            _abilityEvent.OnReceive += OnReceiveEvent;
            _allAssetLoaded = AssetEvents.IsAllAssetLoaded;
            AssetEvents.AllAssetLoaded += AllAssetLoaded;
        }

        private static void AllAssetLoaded()
        {
            _allAssetLoaded = true;
            Setup();
        }

        public static void AddAbility(IAbility ability)
        {
            var key = ability.Name.ToLowerInvariant();

            if (string.IsNullOrEmpty(key))
            {
                Logger.Error("EnemyAbility Name cannot be empty or null!");
                return;
            }

            if (_abilities.ContainsKey(key))
            {
                Logger.Error($"Duplicated EnemyAbility Name was detected! : \"{key}\"");
                return;
            }

            _abilities.Add(key, ability);
        }

        public static IAbility GetAbility(string key)
        {
            key = key.ToLowerInvariant();
            _abilities.TryGetValue(key, out var ability);
            return ability;
        }

        public static bool TryGetAbility(string key, out IAbility ability)
        {
            return _abilities.TryGetValue(key.ToLowerInvariant(), out ability);
        }

        public static void Setup()
        {
            if (!_allAssetLoaded)
                return;

            foreach (var ab in _abilities.Values)
            {
                var id = _syncIDBuffer;
                _abilityIDLookup[id] = ab;
                ab.Setup(id);

                _syncIDBuffer++;
            }
        }

        public static void Clear()
        {
            foreach (var ab in _abilities.Values)
                ab.Unload();

            _abilities.Clear();
            _syncIDBuffer = 1;
            _abilityIDLookup.Clear();
        }

        public static void SendEvent(ushort syncID, ushort enemyID, AbilityPacketType type)
        {
            if (!SNet.IsMaster)
                return;

            _abilityEvent.Send(new AbilityEvent.Packet()
            {
                Type = type,
                SyncID = syncID,
                EnemyID = enemyID,
            });
        }

        private static void OnReceiveEvent(AbilityEvent.Packet packet)
        {
            if (_abilityIDLookup.TryGetValue(packet.SyncID, out var ability))
            {
                switch (packet.Type)
                {
                    case AbilityPacketType.DoExit:
                        ability.Exit(packet.EnemyID);
                        break;

                    case AbilityPacketType.DoTrigger:
                        ability.Trigger(packet.EnemyID);
                        break;

                    case AbilityPacketType.DoExitAll:
                        ability.ExitAll();
                        break;

                    case AbilityPacketType.DoTriggerAll:
                        ability.TriggerAll();
                        break;

                    default:
                        Logger.Error($"PacketType was invalid: {packet.Type}");
                        break;
                }
            }
            else
            {
                Logger.Error($"Packet was invalid: {packet.SyncID} {packet.EnemyID} {packet.Type}");
            }
        }
    }

    public class AbilityEvent : SyncedEvent<AbilityEvent.Packet>
    {
        public override string GUID => "ABE";

        public struct Packet
        {
            public AbilityPacketType Type;
            public ushort SyncID;
            public ushort EnemyID;
        }
    }

    public enum AbilityPacketType : byte
    {
        DoTrigger,
        DoTriggerAll,
        DoExit,
        DoExitAll,
    }
}