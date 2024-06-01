using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace FactionGear.Managers;

public static class Tools
{
    public static string SplitCamelCase(string input)
    {
        // Invalid cast error when trying to format string
        // Using this to create configs
        return input;
        if (string.IsNullOrEmpty(input))
            return input;

        StringBuilder result = new StringBuilder(input[0].ToString());
        for (int i = 1; i < input.Length; i++)
        {
            if (char.IsUpper(input[i]) && !char.IsUpper(input[i - 1]))
            {
                result.Append(' ');
            }
            result.Append(input[i]);
        }

        return result.ToString();
    }
    
    public static void RegisterToObjectDB(GameObject prefab)
    {
        if (!ObjectDB.instance.m_items.Contains(prefab))
        {
            ObjectDB.instance.m_items.Add(prefab);
        }

        ObjectDB.instance.m_itemByHash[prefab.name.GetStableHashCode()] = prefab;
    }

    public static void RegisterToZNetScene(GameObject prefab)
    {
        if (!ZNetScene.instance.m_prefabs.Contains(prefab))
        {
            ZNetScene.instance.m_prefabs.Add(prefab);
        }

        ZNetScene.instance.m_namedPrefabs[prefab.name.GetStableHashCode()] = prefab;
    }
    
    public static string FormatRecipe(Recipe recipe)
    {
        List<string> formatted = new();
        foreach (var requirement in recipe.m_resources)
        {
            string name = requirement.m_resItem.name;
            string amount = requirement.m_amount.ToString();
            string format = name + ":" + amount;
            formatted.Add(format);
        }

        return string.Join(",", formatted);
    }
    
    public static List<Piece.Requirement> GetRecipeResources(string config, ObjectDB __instance)
    {
        List<Piece.Requirement> requirements = new();
        string[] array = config.Split(',');
        foreach (var configs in array)
        {
            string[] pairs = configs.Split(':');
            if (pairs.Length != 2) continue;
            string prefabName = pairs[0];
            if (!int.TryParse(pairs[1], out int amount)) continue;
            GameObject prefab = __instance.GetItemPrefab(prefabName);
            if (!prefab) continue;
            if (!prefab.TryGetComponent(out ItemDrop itemDrop)) continue;
            var require = new Piece.Requirement()
            {
                m_amount = amount,
                m_recover = true,
                m_resItem = itemDrop
            };
            requirements.Add(require);
        }

        return requirements;
    }
}