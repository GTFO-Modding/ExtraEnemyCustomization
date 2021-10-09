﻿using Enemies;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace EECustom.Events
{
    public static class EnemyScannerColorEvents
    {
        public static Action<EnemyAgent, Color> OnChanged;

        public static void RegisterOnChanged(EnemyAgent agent, Action<EnemyAgent, Color> onChanged)
        {
            var id = agent.GlobalID;
            var onChangedWrapper = new Action<EnemyAgent, Color>((EnemyAgent eventAgent, Color color) =>
            {
                if (eventAgent.GlobalID == id)
                {
                    onChanged?.Invoke(eventAgent, color);
                }
            });
            OnChanged += onChangedWrapper;
            agent.add_OnDeadCallback(new Action(() =>
            {
                OnChanged -= onChangedWrapper;
            }));
        }
    }
}
