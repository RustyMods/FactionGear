using System.Collections.Generic;
using HarmonyLib;
using UnityEngine;

namespace FactionGear.Managers;

public static class CapeManager
{
    private static readonly int MainTex = Shader.PropertyToID("_MainTex");
    private static bool m_initated;

    [HarmonyPriority(Priority.Last)]
    [HarmonyPatch(typeof(ObjectDB), nameof(ObjectDB.Awake))]
    private static class RegisterCapes
    {
        private static void Postfix(ObjectDB __instance)
        {
            if (!__instance || !ZNetScene.instance) return;
            if (m_initated) return;
            CreateLoxClones(__instance);
            CreateDeerHideClones(__instance);
            CreateFeatherClones(__instance);
            CreateAshClones(__instance);
            m_initated = true;
        }
    }

    private static void CreateFeatherClones(ObjectDB __instance)
    {
        GameObject featherCape = __instance.GetItemPrefab("CapeFeather");
        if (!featherCape) return;
        if (!featherCape.TryGetComponent(out ItemDrop component)) return;
        Recipe recipe = __instance.GetRecipe(component.m_itemData);
        foreach (var kvp in TextureManager.featherClones)
        {
            var icon = kvp.Key;
            var color = kvp.Value;
            if (icon == null) continue;
            GameObject clone = Object.Instantiate(featherCape, FactionGearPlugin._Root.transform, false);
            clone.name = icon.name;
            
            SetCloneData(__instance, clone, icon, recipe, color, TextureManager.BaseFeatureCape_Tex);
        }
    }

    private static void CreateDeerHideClones(ObjectDB __instance)
    {
        GameObject deerCape = __instance.GetItemPrefab("CapeDeerHide");
        if (!deerCape) return;
        if (!deerCape.TryGetComponent(out ItemDrop component)) return;
        Recipe recipe = __instance.GetRecipe(component.m_itemData);
        foreach (var kvp in TextureManager.deerClones)
        {
            var icon = kvp.Key;
            var color = kvp.Value;
            if (icon == null) continue;
            GameObject clone = Object.Instantiate(deerCape, FactionGearPlugin._Root.transform, false);
            clone.name = icon.name;
            
            SetCloneData(__instance, clone, icon, recipe, color);
        }
    }

    private static void CreateAshClones(ObjectDB __instance)
    {
        GameObject ashCape = __instance.GetItemPrefab("CapeAsh");
        if (!ashCape) return;
        if (!ashCape.TryGetComponent(out ItemDrop component)) return;
        Recipe recipe = __instance.GetRecipe(component.m_itemData);
        foreach (var kvp in TextureManager.ashClones)
        {
            if (kvp.Key == null || kvp.Value == null) continue;

            var icon = kvp.Key;
            var texture = kvp.Value;

            GameObject clone = Object.Instantiate(ashCape, FactionGearPlugin._Root.transform, false);
            clone.name = texture.name;
            
            SetCloneData(__instance, clone, icon, recipe, texture);
        }
    }

    private static void CreateLoxClones(ObjectDB __instance)
    {
        GameObject loxCape = __instance.GetItemPrefab("CapeLox");
        if (!loxCape) return;
        if (!loxCape.TryGetComponent(out ItemDrop component)) return;
        Recipe recipe = __instance.GetRecipe(component.m_itemData);
        foreach (var kvp in TextureManager.loxClones)
        {
            if (kvp.Key == null || kvp.Value == null) continue;

            var icon = kvp.Key;
            var texture = kvp.Value;
            
            GameObject clone = Object.Instantiate(loxCape, FactionGearPlugin._Root.transform, false);
            clone.name = texture.name;

            SetCloneData(__instance, clone, icon, recipe, texture);
        }
    }
    
    private static void SetCloneData(ObjectDB __instance, GameObject clone, Sprite icon, Recipe recipe, Color color, Texture? texture = null)
    {
        if (!clone.TryGetComponent(out ItemDrop component)) return;
        var nameConfig = FactionGearPlugin._plugin.config(Tools.SplitCamelCase(clone.name), "Display Name", clone.name, "Set the display name of the cape");
        var armor = FactionGearPlugin._plugin.config(Tools.SplitCamelCase(clone.name), "Armor", component.m_itemData.m_shared.m_armor, "Set the armor value");
        nameConfig.SettingChanged += (sender, args) =>
        {
            component.m_itemData.m_shared.m_name = nameConfig.Value;
        };
        armor.SettingChanged += (sender, args) =>
        {
            if (!ObjectDB.instance) return;
            component.m_itemData.m_shared.m_armor = armor.Value;
        };

        component.m_itemData.m_shared.m_name = nameConfig.Value;
        component.m_itemData.m_shared.m_icons = new[] { icon };
        
        var renderers = clone.GetComponentsInChildren<Renderer>(true);
        foreach (var renderer in renderers)
        {
            List<Material> newMats = new();
            foreach (var material in renderer.materials)
            {
                Material newMat = new Material(material)
                {
                    color = color
                };
                if (texture != null)
                {
                    newMat.SetTexture(MainTex, texture);
                }

                var colorConfig = FactionGearPlugin._plugin.config(Tools.SplitCamelCase(clone.name), "Color", color, "Set the color");
                colorConfig.SettingChanged += (sender, args) =>
                {
                    foreach (var mat in renderer.materials)
                    {
                        mat.color = colorConfig.Value;
                    }
                };
            
                newMats.Add(newMat);
            }

            renderer.materials = newMats.ToArray();
            renderer.sharedMaterials = newMats.ToArray();
        }
        
        CreateRecipeConfigs(clone, component, recipe, __instance);

        Tools.RegisterToObjectDB(clone);
        Tools.RegisterToZNetScene(clone);
    }

    private static void CreateRecipeConfigs(GameObject clone, ItemDrop component, Recipe recipe, ObjectDB __instance)
    {
        string originalRecipeArray = Tools.FormatRecipe(recipe);
        
        var newRecipe = ScriptableObject.CreateInstance<Recipe>();
        newRecipe.name = clone.name;
        newRecipe.m_amount = 1;
        newRecipe.m_enabled = true;
        newRecipe.m_item = component;
        newRecipe.m_repairStation = recipe.m_craftingStation;
        newRecipe.m_craftingStation = recipe.m_craftingStation;

        var station = FactionGearPlugin._plugin.config(Tools.SplitCamelCase(clone.name), "Crafting Station", recipe.m_craftingStation.name, "Set the crafting station");
        station.SettingChanged += (sender, args) =>
        {
            GameObject prefab = ZNetScene.instance.GetPrefab(station.Value);
            if (!prefab) return;
            if (!prefab.TryGetComponent(out CraftingStation craftingStationComponent)) return;
            newRecipe.m_repairStation = craftingStationComponent;
            newRecipe.m_craftingStation = craftingStationComponent;
        };
        
        newRecipe.m_minStationLevel = 1;

        var reqConfig = FactionGearPlugin._plugin.config(Tools.SplitCamelCase(clone.name), "Recipe", originalRecipeArray, "Set the recipe for the cape, [prefabName]:[amount]");
        reqConfig.SettingChanged += (sender, args) =>
        {
            if (!ObjectDB.instance) return;
            newRecipe.m_resources = Tools.GetRecipeResources(reqConfig.Value, __instance).ToArray();
        };
        
        newRecipe.m_resources = Tools.GetRecipeResources(reqConfig.Value, __instance).ToArray();
        __instance.m_recipes.Add(newRecipe);
    }

    private static void SetCloneData(ObjectDB __instance, GameObject clone, Sprite icon, Recipe recipe, Texture2D texture)
    {
        var nameConfig = FactionGearPlugin._plugin.config(Tools.SplitCamelCase(clone.name), "Display Name", Tools.SplitCamelCase(clone.name),
                "Set the display name of the cape");

        if (!clone.TryGetComponent(out ItemDrop component)) return;
        component.m_itemData.m_shared.m_name = nameConfig.Value;
        component.m_itemData.m_shared.m_icons = new[] { icon };
        var armor = FactionGearPlugin._plugin.config(Tools.SplitCamelCase(clone.name), "Armor",
            component.m_itemData.m_shared.m_armor, "Set the armor value");
        armor.SettingChanged += (sender, args) =>
        {
            if (!ObjectDB.instance) return;
            component.m_itemData.m_shared.m_armor = armor.Value;
        };

        nameConfig.SettingChanged += (sender, args) =>
        {
            component.m_itemData.m_shared.m_name = nameConfig.Value;
        };

        var renderers = clone.GetComponentsInChildren<Renderer>(true);

        foreach (var renderer in renderers)
        {
            List<Material> newMats = new();
            foreach (var material in renderer.materials)
            {
                if (material.shader.name != "Custom/Creature") continue;
                Material newMat = new Material(material);
                newMat.SetTexture(MainTex, texture);
                newMats.Add(newMat);
            }

            renderer.materials = newMats.ToArray();
            renderer.sharedMaterials = newMats.ToArray();
        }

        string originalRecipeArray = Tools.FormatRecipe(recipe);

        var newRecipe = ScriptableObject.CreateInstance<Recipe>();
        newRecipe.name = clone.name;
        newRecipe.m_amount = 1;
        newRecipe.m_enabled = true;
        newRecipe.m_item = component;
        newRecipe.m_repairStation = recipe.m_craftingStation;
        newRecipe.m_craftingStation = recipe.m_craftingStation;

        var station = FactionGearPlugin._plugin.config(Tools.SplitCamelCase(clone.name), "Crafting Station", recipe.m_craftingStation.name,
            "Set the crafting station");
        station.SettingChanged += (sender, args) =>
        {
            GameObject prefab = ZNetScene.instance.GetPrefab(station.Value);
            if (!prefab) return;
            if (!prefab.TryGetComponent(out CraftingStation craftingStationComponent)) return;
            newRecipe.m_repairStation = craftingStationComponent;
            newRecipe.m_craftingStation = craftingStationComponent;
        };
        
        newRecipe.m_minStationLevel = 1;

        var reqConfig = FactionGearPlugin._plugin.config(Tools.SplitCamelCase(clone.name), "Recipe", originalRecipeArray,
            "Set the recipe for the cape, [prefabName]:[amount]");
        reqConfig.SettingChanged += (sender, args) =>
        {
            if (!ObjectDB.instance) return;
            newRecipe.m_resources = Tools.GetRecipeResources(reqConfig.Value, __instance).ToArray();
        };
        
        newRecipe.m_resources = Tools.GetRecipeResources(reqConfig.Value, __instance).ToArray();
        __instance.m_recipes.Add(newRecipe);
        
        Tools.RegisterToObjectDB(clone);
        Tools.RegisterToZNetScene(clone);
    }
}