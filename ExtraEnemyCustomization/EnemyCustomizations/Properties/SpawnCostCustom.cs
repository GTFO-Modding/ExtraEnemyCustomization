﻿using Agents;
using Enemies;

namespace EEC.EnemyCustomizations.Properties
{
    public sealed class SpawnCostCustom : EnemyCustomBase, IEnemySpawnedEvent
    {
        public float SpawnCost { get; set; } = 0.0f;

        public override string GetProcessName()
        {
            return "SpawnCost";
        }

        public void OnSpawned(EnemyAgent agent)
        {
            agent.m_enemyCost = SpawnCost;
            if (agent.AI.Mode == AgentMode.Agressive)
            {
                float delta = EnemyCostManager.Current.m_enemyTypeCosts[(int)agent.EnemyData.EnemyType] - SpawnCost;
                EnemyCostManager.AddCost(agent.DimensionIndex, -delta);
                if (Logger.DevLogAllowed)
                    LogDev($"Decremented cost by {delta}!");
            }
            else
            {
                if (Logger.DevLogAllowed)
                    LogDev($"Set Enemy Cost to {SpawnCost}!");
            }
        }
    }
}