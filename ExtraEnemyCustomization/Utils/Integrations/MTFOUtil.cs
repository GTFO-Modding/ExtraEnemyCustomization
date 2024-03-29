﻿using BepInEx;
using BepInEx.Unity.IL2CPP;
using System;
using System.Linq;
using System.Reflection;

namespace EEC.Utils.Integrations
{
    public static class MTFOUtil
    {
        public const string PLUGIN_GUID = "com.dak.MTFO";
        public const BindingFlags PUBLIC_STATIC = BindingFlags.Public | BindingFlags.Static;

        public static readonly SemanticVersioning.Version MTFO_FORBID = new("4.2.0");
        public static readonly SemanticVersioning.Version MTFO_V5 = new("4.3.5");
        public static string GameDataPath { get; private set; } = string.Empty;
        public static string CustomPath { get; private set; } = string.Empty;
        public static bool HasCustomContent { get; private set; } = false;
        public static bool IsLoaded { get; private set; } = false;
        public static bool HasHotReload { get; private set; } = false;
        public static FieldInfo HotReloaderField { get; private set; } = null;

        public static event Action HotReloaded;

        static MTFOUtil()
        {
            if (!IL2CPPChainloader.Instance.Plugins.TryGetValue(PLUGIN_GUID, out var info))
                return;

            var version = info.Metadata.Version;

            if (version >= MTFO_V5)
            {
                InitMTFO_V5(info);
            }
            else if (version > MTFO_FORBID)
            {
                InitMTFO_V4(info);
            }
            else
            {
                throw new Exception("You are using unsupported version of MTFO!");
            }
        }

        private static void InitMTFO_V4(PluginInfo info)
        {
            try
            {
                var ddAsm = info?.Instance?.GetType()?.Assembly ?? null;

                if (ddAsm is null)
                    throw new Exception("Assembly is Missing!");

                var types = ddAsm.GetTypes();
                var cfgManagerType = types.First(t => t.Name == "ConfigManager");

                if (cfgManagerType is null)
                    throw new Exception("Unable to Find ConfigManager Class");

                var dataPathField = cfgManagerType.GetField("GameDataPath", PUBLIC_STATIC);
                var customPathField = cfgManagerType.GetField("CustomPath", PUBLIC_STATIC);
                var hasCustomField = cfgManagerType.GetField("HasCustomContent", PUBLIC_STATIC);

                if (dataPathField is null)
                    throw new Exception("Unable to Find Field: GameDataPath");

                if (customPathField is null)
                    throw new Exception("Unable to Find Field: CustomPath");

                if (hasCustomField is null)
                    throw new Exception("Unable to Find Field: HasCustomContent");

                GameDataPath = (string)dataPathField.GetValue(null);
                CustomPath = (string)customPathField.GetValue(null);
                HasCustomContent = (bool)hasCustomField.GetValue(null);
                IsLoaded = true;
            }
            catch (Exception e)
            {
                Logger.Error($"Exception thrown while reading path from DataDumper (MTFO V{info.Metadata.Version}): \n{e}");
            }
        }

        private static void InitMTFO_V5(PluginInfo info)
        {
            try
            {
                var ddAsm = info?.Instance?.GetType()?.Assembly ?? null;

                if (ddAsm is null)
                    throw new Exception("Assembly is Missing!");

                var types = ddAsm.GetTypes();
                var cfgManagerType = types.First(t => t.Name == "ConfigManager");

                if (cfgManagerType is null)
                    throw new Exception("Unable to Find ConfigManager Class");

                var dataPathField = cfgManagerType.GetField("GameDataPath", PUBLIC_STATIC);
                var customPathField = cfgManagerType.GetField("CustomPath", PUBLIC_STATIC);
                var hasCustomField = cfgManagerType.GetField("HasCustomContent", PUBLIC_STATIC);
                var hasHotReloadProperty = cfgManagerType.GetProperty("IsHotReloadEnabled", PUBLIC_STATIC);

                if (dataPathField is null)
                    throw new Exception("Unable to Find Field: GameDataPath");

                if (customPathField is null)
                    throw new Exception("Unable to Find Field: CustomPath");

                if (hasCustomField is null)
                    throw new Exception("Unable to Find Field: HasCustomContent");

                if (hasCustomField is null)
                    throw new Exception("Unable to Find Field: HasCustomContent");

                if (hasHotReloadProperty is null)
                    throw new Exception("Unable to Find Property: IsHotReloadEnabled");

                GameDataPath = (string)dataPathField.GetValue(null);
                CustomPath = (string)customPathField.GetValue(null);
                HasCustomContent = (bool)hasCustomField.GetValue(null);
                HasHotReload = (bool)hasHotReloadProperty.GetValue(null);

                if (HasHotReload)
                {
                    var hotreloadManageType = types.First(t => t.Name == "HotReloader");
                    if (hotreloadManageType is null)
                        throw new Exception("Unable to Find HotReloader Class");

                    var hotreloaderField = hotreloadManageType.GetField("Current", PUBLIC_STATIC);
                    if (hotreloaderField is null)
                        throw new Exception("Unable to Find Field: Current");

                    HotReloaderField = hotreloaderField;
                }

                IsLoaded = true;
            }
            catch (Exception e)
            {
                Logger.Error($"Exception thrown while reading metadata from MTFO (V{info.Metadata.Version}): \n{e}");
            }
        }

        internal static void OnHotReloaded(int _)
        {
            HotReloaded?.Invoke();
        }
    }
}