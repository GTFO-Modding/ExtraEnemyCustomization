﻿using UnityEngine;

namespace EEC.EnemyCustomizations.Shared
{
    public enum RepeatMode
    {
        Clamped,
        Unclamped,
        PingPong,
        Repeat
    }

    public interface IMultiplierShiftSetting
    {
        public float MinMulti { get; set; }
        public float MaxMulti { get; set; }
        public float Duration { get; set; }
        public bool StopAfterDuration { get; set; }
        public float StopMulti { get; set; }
        public eEasingType EasingMode { get; set; }
        public RepeatMode Mode { get; set; }
    }

    public sealed class MultiplierShiftSetting : IMultiplierShiftSetting
    {
        public bool Enabled { get; set; } = false;
        public float MinMulti { get; set; } = 1.0f;
        public float MaxMulti { get; set; } = 1.0f;
        public float Duration { get; set; } = 1.0f;
        public bool StopAfterDuration { get; set; } = true;
        public float StopMulti { get; set; } = 1.0f;
        public eEasingType EasingMode { get; set; } = eEasingType.Linear;
        public RepeatMode Mode { get; set; } = RepeatMode.Clamped;

        public float EvaluateMultiplier(float progress)
        {
            return Mode switch
            {
                RepeatMode.Unclamped => Mathf.LerpUnclamped(MinMulti, MaxMulti, Ease(progress)),
                RepeatMode.PingPong => Mathf.Lerp(MinMulti, MaxMulti, Ease(Mathf.PingPong(progress, 1.0f))),
                RepeatMode.Repeat => Mathf.Lerp(MinMulti, MaxMulti, Ease(Mathf.Repeat(progress, 1.0f))),
                _ => Mathf.LerpUnclamped(MinMulti, MaxMulti, Mathf.Clamp01(Ease(progress))) //Clamped, Etc
            };
        }

        private float Ease(float p)
        {
            return Easing.GetEasingValue(EasingMode, p, false);
        }
    }
}