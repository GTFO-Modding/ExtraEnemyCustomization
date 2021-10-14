﻿using AIGraph;
using EECustom.CustomSettings.DTO;
using EECustom.Events;
using Enemies;
using GameData;
using LevelGeneration;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EECustom.CustomSettings
{
    public static class CustomScoutWaveManager
    {
        private static readonly System.Random _Random = new System.Random();
        private static readonly List<ExpeditionScoutSetting> _ScoutSettings = new List<ExpeditionScoutSetting>();
        private static readonly List<ScoutSettingCache> _ActiveScoutSettings = new List<ScoutSettingCache>();
        private static readonly Dictionary<string, ScoutTargetSetting> _TargetSettingDict = new Dictionary<string, ScoutTargetSetting>();
        private static readonly Dictionary<string, ScoutWaveSetting> _WaveSettingDict = new Dictionary<string, ScoutWaveSetting>();

        private static string _PreviousExpKey = string.Empty;
        private static ExpeditionData _PreviousExpData = null;
        private static uint _DefaultWaveSettingID;
        private static uint _DefaultWavePopulationID;
        private static eRundownTier _CurrentExpeditionTier;
        private static int _CurrentExpeditionIndex;
        private static bool _IsDefaultSettingBlocked = false;

        static CustomScoutWaveManager()
        {
            LevelEvents.OnBuildStart += OnLevelStateUpdate;
            LevelEvents.OnLevelCleanup += OnLevelStateUpdate;
        }

        private static void OnLevelStateUpdate()
        {
            var rawKey = RundownManager.ActiveExpeditionUniqueKey;
            if (string.IsNullOrEmpty(rawKey))
            {
                Logger.Log("KEY WAS EMPTY");
                return;
            }

            if (rawKey.Equals(_PreviousExpKey))
            {
                return;
            }
            _PreviousExpKey = rawKey;

            var split = rawKey.Split("_");
            if (split.Length != 4)
            {
                Logger.Warning($"Key split length was not 4?: Length: {split.Length} Raw: {rawKey}");
                return;
            }

            if (!Enum.TryParse(split[2], out eRundownTier tier))
            {
                Logger.Warning($"Tier is not valid?: {split[2]}");
                return;
            }

            if (!int.TryParse(split[3], out var index))
            {
                Logger.Warning($"Index was not a number?: {split[3]}");
                return;
            }

            ExpeditionUpdate(tier, index, RundownManager.ActiveExpedition);
        }

        public static void ExpeditionUpdate(eRundownTier tier, int expIndex, ExpeditionInTierData inTierData)
        {
            _IsDefaultSettingBlocked = false;

            //Revert Previous Expedition Data
            if (_PreviousExpData != null)
            {
                _PreviousExpData.ScoutWaveSettings = _DefaultWaveSettingID;
                _PreviousExpData.ScoutWavePopulation = _DefaultWavePopulationID;
            }

            _PreviousExpData = inTierData.Expedition;
            _DefaultWaveSettingID = _PreviousExpData.ScoutWaveSettings;
            _DefaultWavePopulationID = _PreviousExpData.ScoutWavePopulation;

            _CurrentExpeditionTier = tier;
            _CurrentExpeditionIndex = expIndex;

            _ActiveScoutSettings.Clear();
            foreach (var setting in _ScoutSettings)
            {
                if (!setting.IsMatch(_CurrentExpeditionTier, _CurrentExpeditionIndex))
                    continue;

                foreach (var scoutSetting in setting.ScoutSettings)
                {
                    var targetSetting = GetTargetSetting(scoutSetting.TargetSetting);
                    var waveSetting = GetWaveSetting(scoutSetting.WaveSetting);

                    if (targetSetting == null)
                        continue;

                    if (waveSetting == null)
                        continue;

                    var settingCache = new ScoutSettingCache()
                    {
                        TargetSetting = targetSetting,
                        WaveSetting = waveSetting
                    };
                    _ActiveScoutSettings.Add(settingCache);
                }
            }

            if (_ActiveScoutSettings.Count > 0)
            {
                _PreviousExpData.ScoutWaveSettings = 0u;
                _PreviousExpData.ScoutWavePopulation = 0u;
                _IsDefaultSettingBlocked = true;
            }
        }

        public static void AddScoutSetting(params ExpeditionScoutSetting[] scoutSettings)
        {
            foreach (var setting in scoutSettings)
                AddScoutSetting(setting);
        }

        public static void AddScoutSetting(ExpeditionScoutSetting scoutSetting)
        {
            _ScoutSettings.Add(scoutSetting);
        }

        public static void AddWaveSetting(params ScoutWaveSetting[] waveSettings)
        {
            foreach (var setting in waveSettings)
                AddWaveSetting(setting);
        }

        public static void AddWaveSetting(ScoutWaveSetting waveSetting)
        {
            var key = waveSetting.Name.ToLower();
            if (!_WaveSettingDict.ContainsKey(key))
            {
                _WaveSettingDict.Add(key, waveSetting);
            }
        }

        public static ScoutWaveSetting GetWaveSetting(string name)
        {
            var key = name.ToLower();
            if (_WaveSettingDict.ContainsKey(key))
                return _WaveSettingDict[key];
            else
                return null;
        }

        public static void AddTargetSetting(params ScoutTargetSetting[] targetSettings)
        {
            foreach (var setting in targetSettings)
                AddTargetSetting(setting);
        }

        public static void AddTargetSetting(ScoutTargetSetting targetSetting)
        {
            var key = targetSetting.Name.ToLower();
            if (!_TargetSettingDict.ContainsKey(key))
            {
                _TargetSettingDict.Add(key, targetSetting);
            }
        }

        public static ScoutTargetSetting GetTargetSetting(string name)
        {
            var key = name.ToLower();
            if (_TargetSettingDict.ContainsKey(key))
                return _TargetSettingDict[key];
            else
                return null;
        }

        public static void ClearAll()
        {
            _ScoutSettings.Clear();
            _WaveSettingDict.Clear();
            _TargetSettingDict.Clear();
        }

        public static void TriggerScoutWave(EnemyAgent scoutAgent)
        {
            if (!_IsDefaultSettingBlocked)
                return;

            bool triggeredAny = false;
            List<ushort> stopOnDeathWaves = new List<ushort>();
            foreach (var scoutSetting in _ActiveScoutSettings)
            {
                var targetSetting = scoutSetting.TargetSetting;
                var waveSetting = scoutSetting.WaveSetting;

                if (!targetSetting.Target.IsMatch(scoutAgent))
                    continue;

                if (waveSetting.Waves.GetLength(0) <= 0)
                    continue;

                var waveVariantCount = waveSetting.Waves.GetLength(0);
                if (waveVariantCount > 1)
                {
                    var weightValues = new float[waveVariantCount];
                    if (waveSetting.WeightsOverride.Length > 0)
                    {
                        for (int i = 0; i < waveSetting.WeightsOverride.Length; i++)
                        {
                            weightValues[i] = Math.Min(0.0f, waveSetting.WeightsOverride[i]);
                        }
                    }

                    var index = PickWeightedRandom(weightValues);
                    TriggerWaves(waveSetting.Waves[index], scoutAgent.CourseNode, ref stopOnDeathWaves);
                    triggeredAny = true;
                    break;
                }
                else if (waveVariantCount == 1)
                {
                    TriggerWaves(waveSetting.Waves[0], scoutAgent.CourseNode, ref stopOnDeathWaves);
                    triggeredAny = true;
                    break;
                }
                else
                {
                    Logger.Warning("WaveSetting count was zero: Wave has ignored");
                }
            }

            if (triggeredAny)
            {
                if (stopOnDeathWaves.Count <= 0)
                    return;

                scoutAgent.add_OnDeadCallback(new Action(() =>
                {
                    foreach (var waveid in stopOnDeathWaves)
                    {
                        if (Mastermind.Current.TryGetEvent(waveid, out var e))
                        {
                            e.StopEvent();
                        }
                    }
                }));
            }
            else
            {
                TriggerDefaultWave(scoutAgent.CourseNode);
            }
        }

        private static int PickWeightedRandom(float[] weights)
        {
            if (weights.Length <= 0)
                throw new ArgumentException("weights length is zero!");

            if (weights.Length == 1)
                return 0;

            var weightValues = new float[weights.Length];
            weights.CopyTo(weightValues, 0);

            var weightsTotal = weightValues.Sum();
            if (weightsTotal <= 0.0f)
            {
                return _Random.Next(0, weights.Length);
            }

            var accumulatedWeight = 0.0f;
            for (int i = 0; i < weightValues.Length; i++)
            {
                if (weightValues[i] <= 0.0f)
                {
                    weightValues[i] = -1.0f;
                    continue;
                }

                accumulatedWeight += weightValues[i];
                weightValues[i] = accumulatedWeight;
            }

            var randValue = (float)_Random.NextDouble() * weightsTotal;
            for (int i = 0; i < weightValues.Length; i++)
            {
                if (weightValues[i] >= randValue)
                {
                    return i;
                }
            }

            throw new IndexOutOfRangeException("index was not picked?!");
        }

        private static void TriggerWaves(WaveSetting[] waves, AIG_CourseNode sourceNode, ref List<ushort> stopOnDeathList)
        {
            foreach (var wave in waves)
            {
                var waveSpawnType = wave.SpawnSetting.SpawnType switch
                {
                    WaveSpawnType.ClosestAlive => SurvivalWaveSpawnType.InRelationToClosestAlivePlayer,
                    WaveSpawnType.ClosestNoBetweenPlayers => SurvivalWaveSpawnType.ClosestToSuppliedNodeButNoBetweenPlayers,
                    WaveSpawnType.InNode => SurvivalWaveSpawnType.InSuppliedCourseNode,
                    WaveSpawnType.InNodeZone => SurvivalWaveSpawnType.InSuppliedCourseNodeZone,
                    _ => SurvivalWaveSpawnType.InRelationToClosestAlivePlayer,
                };

                var node = sourceNode;
                switch (wave.SpawnSetting.NodeType)
                {
                    case SpawnNodeType.FromArea:
                        if (Builder.CurrentFloor.TryGetZoneByLocalIndex(wave.SpawnSetting.Layer, wave.SpawnSetting.LocalIndex, out var zone))
                        {
                            if (wave.SpawnSetting.AreaIndex < zone.m_areas.Count)
                            {
                                node = zone.m_areas[wave.SpawnSetting.AreaIndex].m_courseNode;
                            }
                            else
                            {
                                node = zone.m_areas[0].m_courseNode;
                            }
                        }
                        break;

                    case SpawnNodeType.FromElevatorArea:
                        node = Builder.GetElevatorArea().m_courseNode;
                        break;
                }

                Mastermind.Current.TriggerSurvivalWave(
                    refNode: node,
                    settingsID: wave.WaveSettingID,
                    populationDataID: wave.WavePopulationID,
                    eventID: out var eventID,
                    spawnType: waveSpawnType,
                    spawnDelay: wave.Delay,
                    playScreamOnSpawn: true
                    );

                if (wave.StopWaveOnDeath)
                {
                    stopOnDeathList.Add(eventID);
                }
            }
        }

        private static void TriggerDefaultWave(AIG_CourseNode sourceNode)
        {
            Logger.Debug("Can't found good setting for scout, Spawning Default");
            Mastermind.Current.TriggerSurvivalWave(
                    refNode: sourceNode,
                    settingsID: _DefaultWaveSettingID,
                    populationDataID: _DefaultWavePopulationID,
                    eventID: out _,
                    spawnType: SurvivalWaveSpawnType.InRelationToClosestAlivePlayer,
                    spawnDelay: 0.0f,
                    playScreamOnSpawn: true
                    );
        }
    }

    public struct ScoutSettingCache
    {
        public ScoutTargetSetting TargetSetting;
        public ScoutWaveSetting WaveSetting;
    }
}