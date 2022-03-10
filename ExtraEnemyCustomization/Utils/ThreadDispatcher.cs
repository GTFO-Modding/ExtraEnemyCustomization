﻿using EECustom.Attributes;
using System;
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;

namespace EECustom.Utils
{
    [InjectToIl2Cpp]
    public sealed class ThreadDispatcher : MonoBehaviour
    {
        public static ThreadDispatcher Current { get; private set; } = null;

        private static readonly ConcurrentQueue<Action> _queue = new();

        internal static void Initialize()
        {
            if (Current == null)
            {
                var dispatcher = new GameObject();
                DontDestroyOnLoad(dispatcher);

                Current = dispatcher.AddComponent<ThreadDispatcher>();
            }
        }

        public static void Enqueue(Action action)
        {
            if (action == null)
                return;

            _queue.Enqueue(action);
        }

        [SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "MonoBehaviour Callback can't be static")]
        internal void Update()
        {
            while (_queue.TryDequeue(out var action))
            {
                action.Invoke();
            }
        }
    }
}