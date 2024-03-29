﻿using AssetShards;

namespace EEC.Events
{
    public delegate void AssetLoadedHandler();

    public static class AssetEvents
    {
        public static bool IsStartupAssetLoaded { get; private set; } = false;
        public static bool IsEnemyAssetLoaded { get; private set; } = false;
        public static bool IsShardAssetLoaded { get; private set; } = false;
        public static bool IsAllAssetLoaded { get; private set; } = false;

        public static event AssetLoadedHandler StartupAssetLoaded;

        public static event AssetLoadedHandler EnemyAssetLoaded;

        public static event AssetLoadedHandler ShardAssetLoaded;

        public static event AssetLoadedHandler AllAssetLoaded;

        static AssetEvents()
        {
            IsStartupAssetLoaded = AssetShardManager.StartupAssetsIsLoaded;
            IsEnemyAssetLoaded = AssetShardManager.EnemyAssetsIsLoaded;
            IsShardAssetLoaded = AssetShardManager.SharedAssetsIsLoaded;
        }

        internal static void OnStartupAssetLoaded()
        {
            IsStartupAssetLoaded = true;
            StartupAssetLoaded?.Invoke();
            CheckLoadedStatus();
        }

        internal static void OnEnemyAssetLoaded()
        {
            IsEnemyAssetLoaded = true;
            EnemyAssetLoaded?.Invoke();
            CheckLoadedStatus();
        }

        internal static void OnShardAssetLoaded()
        {
            IsShardAssetLoaded = true;
            ShardAssetLoaded?.Invoke();
            CheckLoadedStatus();
        }

        private static void CheckLoadedStatus()
        {
            if (!IsAllAssetLoaded)
            {
                IsAllAssetLoaded = IsStartupAssetLoaded & IsEnemyAssetLoaded & IsShardAssetLoaded;
                if (IsAllAssetLoaded)
                    AllAssetLoaded?.Invoke();
            }
        }
    }
}