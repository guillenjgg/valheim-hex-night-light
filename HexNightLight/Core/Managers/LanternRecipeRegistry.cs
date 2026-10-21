using System.Linq;
using UnityEngine;

namespace HexNightLight.Core.Managers
{
    internal static class LanternRecipeRegistry
    {
        private const string RecipeName = "Recipe_HexNightLight";

        internal static void Register(ObjectDB database, GameObject lanternPrefab)
        {
            if (database == null || lanternPrefab == null) return;

            if (database.m_recipes.Any(x => x != null && x.name == RecipeName))
            {
                return;
            }

            var bronze = database.GetItemPrefab("Bronze")?.GetComponent<ItemDrop>();
            var surtlingCore = database.GetItemPrefab("SurtlingCore")?.GetComponent<ItemDrop>();
            var resin = database.GetItemPrefab("Resin")?.GetComponent<ItemDrop>();
            var forge = ZNetScene.instance?.GetPrefab("forge")?.GetComponent<CraftingStation>();
            var lantern = lanternPrefab.GetComponent<ItemDrop>();

            if (bronze == null || surtlingCore == null || resin == null || forge == null || lantern == null)
            {
                Plugin.Log.LogError("HexNightLight recipe registration failed: missing required prefabs.");
                return;
            }

            var recipe = ScriptableObject.CreateInstance<Recipe>();

            recipe.name = RecipeName;
            recipe.m_item = lantern;
            recipe.m_amount = 1;
            recipe.m_craftingStation = forge;
            recipe.m_minStationLevel = 1;
            recipe.m_resources = new[]
            {
                new Piece.Requirement { m_resItem = bronze, m_amount = 2 },
                new Piece.Requirement { m_resItem = surtlingCore, m_amount = 1 },
                new Piece.Requirement { m_resItem = resin, m_amount = 10 }
            };

            database.m_recipes.Add(recipe);

            Plugin.Log.LogInfo($"Registered recipe: {RecipeName}");
        }
    }
}