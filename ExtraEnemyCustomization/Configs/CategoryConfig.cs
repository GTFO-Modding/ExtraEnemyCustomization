﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace EEC.Configs
{
    public sealed class CategoryConfig : Config
    {
        public string[] Categories { get; set; } = Array.Empty<string>();
        public IdWithCategories[] CategoryPair { get; set; } = Array.Empty<IdWithCategories>();
        public CategoryWithIds[] IdPair { get; set; } = Array.Empty<CategoryWithIds>();

        private readonly Dictionary<string, CategoryDefinition> _categoryCache = new();

        public override string FileName => "Category";

        public override void Loaded()
        {
            //Assign Category
            foreach (var category in Categories)
            {
                if (_categoryCache.ContainsKey(category.ToLowerInvariant()))
                {
                    Logger.Error($"Overlapping Category Found, Category Name: {category}");
                    continue;
                }

                _categoryCache.Add(category.ToLowerInvariant(), new CategoryDefinition()
                {
                    Name = category
                });
                Logger.Debug($"Category Defined! '{category}'");
            }

            //Id-Categories Pair
            foreach (var categoryPair in CategoryPair)
            {
                foreach (var category in categoryPair.Categories)
                {
                    if (!_categoryCache.TryGetValue(category.ToLowerInvariant(), out var definition))
                    {
                        Logger.Error($"Unable to find Category: {category}");
                        continue;
                    }

                    definition.AddEnemyID(categoryPair.PersistentID);
                }

                Logger.Verbose($"Assign Categories to ID: '{categoryPair.PersistentID}', Categories: [{string.Join(", ", categoryPair.Categories)}]");
            }

            //Category-Ids Pair
            foreach (var idPair in IdPair)
            {
                if (!_categoryCache.TryGetValue(idPair.Category.ToLowerInvariant(), out var definition))
                {
                    Logger.Error($"Unable to find Category: {idPair.Category}");
                    continue;
                }

                definition.AddEnemyIDRange(idPair.PersistentIDs);
                Logger.Verbose($"Assign Id to Category: '{idPair.Category}', ids: [{string.Join(", ", idPair.PersistentIDs)}]");
            }

            //Final Cache
            foreach (var categoryCache in _categoryCache.Values)
            {
                categoryCache.CacheID();

                Logger.Debug($"Category Initialized! '{categoryCache.Name}', ids: [{string.Join(", ", categoryCache.PersistentIDs)}]");
            }
        }

        public bool Any(string[] categories, uint enemyID)
        {
            foreach (var category in categories)
            {
                if (!_categoryCache.TryGetValue(category.ToLowerInvariant(), out var categoryDef))
                {
                    Logger.Warning($"Unable to find Category with name: {category}");
                    continue;
                }

                if (categoryDef.PersistentIDs.Contains(enemyID))
                {
                    return true;
                }
            }

            return false;
        }

        public bool All(string[] categories, uint enemyID)
        {
            var result = true;
            foreach (var category in categories)
            {
                if (!_categoryCache.TryGetValue(category.ToLowerInvariant(), out var categoryDef))
                {
                    Logger.Warning($"Unable to find Category with name: {category}");
                    result = false;
                    break;
                }

                if (!categoryDef.PersistentIDs.Contains(enemyID))
                {
                    result = false;
                    break;
                }
            }

            return result;
        }
    }

    public class IdWithCategories
    {
        public uint PersistentID { get; set; }
        public string[] Categories { get; set; } = Array.Empty<string>();
    }

    public class CategoryWithIds
    {
        public string Category { get; set; } = string.Empty;
        public uint[] PersistentIDs { get; set; } = Array.Empty<uint>();
    }

    public class CategoryDefinition
    {
        public string Name { get; set; }
        public uint[] PersistentIDs { get; private set; } = Array.Empty<uint>();

        public readonly List<uint> _PersistentIDs = new();

        public void AddEnemyIDRange(uint[] ids)
        {
            Array.ForEach(ids, (uint id) => { AddEnemyID(id); });
        }

        public void AddEnemyID(uint id)
        {
            if (!_PersistentIDs.Contains(id))
            {
                _PersistentIDs.Add(id);
            }
        }

        public void CacheID()
        {
            PersistentIDs = _PersistentIDs.ToArray();
        }
    }
}