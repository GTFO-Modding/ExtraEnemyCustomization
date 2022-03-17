﻿using GTFO.API.Utilities;
using System.IO;

namespace EEC.Managers
{
    public static partial class ConfigManager
    {
        private static void OnConfigFileEdited_ReloadConfig(object sender, FileSystemEventArgs e)
        {
            var fileExtension = Path.GetExtension(e.Name);
            if (fileExtension.InvariantEquals(".json", ignoreCase: true) ||
                fileExtension.InvariantEquals(".jsonc", ignoreCase: true))
            {
                EnqueueJob(e.Name);
            }
        }

        private static void EnqueueJob(string path)
        {
            ThreadDispatcher.Dispatch(() =>
            {
                var filename = Path.GetFileNameWithoutExtension(path);
                if (_configFileNameToType.TryGetValue(filename.ToLowerInvariant(), out var type))
                {
                    filename = _configTypeToFileName[type];

                    Logger.Log($"Config File Changed: {filename}");

                    ReloadConfig();

                    //TODO: Implement Reload Individual File

                    /*
                    _configInstances[filename].Unloaded();
                    if (_configInstances[filename] is CustomizationConfig custom)
                    {
                        var settings = custom.GetAllSettings();
                        foreach (var setting in settings)
                        {
                            setting.OnConfigUnloaded();
                        }
                    }
                    LoadConfig(type);
                    Current.GenerateBuffer();
                    */
                }
            });
        }
    }
}