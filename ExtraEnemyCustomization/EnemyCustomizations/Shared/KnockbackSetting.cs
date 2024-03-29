﻿using Agents;
using Player;
using UnityEngine;

namespace EEC.EnemyCustomizations.Shared
{
    public interface IKnockbackSetting
    {
        public float Velocity { get; set; }
        public float VelocityZ { get; set; }
        public bool DoMultDistance { get; set; }
        public bool DoMultDistanceZ { get; set; }
    }

    public sealed class KnockbackSetting : IKnockbackSetting
    {
        public bool Enabled { get; set; } = false;
        public float Velocity { get; set; } = 0.0f;
        public float VelocityZ { get; set; } = 0.0f;
        public bool DoMultDistance { get; set; } = false;
        public bool DoMultDistanceZ { get; set; } = false;

        public void DoKnockback(Agent inflictor, PlayerAgent player) => DoKnockback(inflictor.Position, player);

        public void DoKnockback(Vector3 inflictorPos, PlayerAgent player)
        {
            var playerPos = player.Position;
            var powerVec = playerPos - inflictorPos;

            var distance = powerVec.magnitude;
            var direction = powerVec / distance;

            var velocity = direction * Velocity;
            var velocityZ = Vector3.up * VelocityZ;
            if (DoMultDistance)
            {
                velocity *= distance;
            }

            if (DoMultDistanceZ)
            {
                velocityZ *= distance;
            }

            player.Locomotion.AddExternalPushForce(velocity);

            if (VelocityZ != 0.0f && player.Alive)
            {
                player.Locomotion.ChangeState(PlayerLocomotion.PLOC_State.Jump, true);
                player.Locomotion.VerticalVelocity = velocity + velocityZ;
            }
        }

        public void DoKnockbackIgnoreDistance(Agent inflictor, PlayerAgent player) => DoKnockback(inflictor.Position, player);

        public void DoKnockbackIgnoreDistance(Vector3 inflictorPos, PlayerAgent player)
        {
            var playerPos = player.Position;
            var powerVec = playerPos - inflictorPos;

            var distance = powerVec.magnitude;
            var direction = powerVec / distance;

            var velocity = direction * Velocity;
            var velocityZ = Vector3.up * VelocityZ;

            player.Locomotion.AddExternalPushForce(velocity);

            if (VelocityZ != 0.0f && player.Alive)
            {
                player.Locomotion.ChangeState(PlayerLocomotion.PLOC_State.Jump, true);
                player.Locomotion.VerticalVelocity = velocity + velocityZ;
            }
        }
    }
}