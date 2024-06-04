using System.Collections.Generic;
using HarmonyLib;
using UnityEngine;

namespace FactionGear.Managers;

public static class ArmorManager
{
    private static readonly int MainTex = Shader.PropertyToID("_MainTex");
    private static readonly int Metal = Shader.PropertyToID("_Metallic");
    private static readonly int OcclusionStrength = Shader.PropertyToID("_OcclusionStrength");

    private static readonly List<Item> MaterialConfigs = new();
    private static readonly List<Item> UnderArmor = new();
    private static readonly List<Item> UnderPants = new();

    private static bool m_iniated;
    
    [HarmonyPriority(Priority.Last)]
    [HarmonyPatch(typeof(ObjectDB), nameof(ObjectDB.Awake))]
    private static class RegisterArmorClones
    {
        private static void Postfix(ObjectDB __instance)
        {
            if (!__instance || !ZNetScene.instance) return;
            if (m_iniated) return;
            CreateAshMageClones(__instance);
            AddSkinMaterial(__instance);
            CreateMaterialConfigs(__instance);
            m_iniated = true;
        }
    }

    private static void AddSkinMaterial(ObjectDB __instance)
    {
        var AshChest = __instance.GetItemPrefab("ArmorMageChest_Ashlands");
        var AshLegs = __instance.GetItemPrefab("ArmorMageLegs_Ashlands");

        if (!AshChest.TryGetComponent(out ItemDrop chestComponent)) return;
        if (!AshLegs.TryGetComponent(out ItemDrop legsComponent)) return;

        foreach (var item in UnderArmor)
        {
            var prefab = __instance.GetItemPrefab(item.Prefab.name);
            if (!prefab) continue;
            if (!prefab.TryGetComponent(out ItemDrop component)) continue;
            component.m_itemData.m_shared.m_armorMaterial = chestComponent.m_itemData.m_shared.m_armorMaterial;
        }

        foreach (var item in UnderPants)
        {
            var prefab = __instance.GetItemPrefab(item.Prefab.name);
            if (!prefab) continue;
            if (!prefab.TryGetComponent(out ItemDrop component)) continue;
            component.m_itemData.m_shared.m_armorMaterial = legsComponent.m_itemData.m_shared.m_armorMaterial;
        }
    }

    private static void CreateMaterialConfigs(ObjectDB __instance)
    {
        foreach (var item in MaterialConfigs)
        {
            var prefab = __instance.GetItemPrefab(item.Prefab.name);
            if (!prefab) continue;
            foreach (var renderer in prefab.GetComponentsInChildren<Renderer>(true))
            {
                foreach (var material in renderer.materials)
                {
                    // if (material.HasProperty(Metal))
                    // {
                    //     var metallicConfig = FactionGearPlugin._plugin.config(SplitCamelCase(prefab.name),
                    //         "0 - " + material.name.Replace("(Instance)", "") + " Metallic", material.GetFloat(Metal),
                    //         new ConfigDescription("Set shine of material", new AcceptableValueRange<float>(0.1f, 1f)));
                    //     metallicConfig.SettingChanged += (sender, args) =>
                    //     {
                    //         material.SetFloat(Metal, metallicConfig.Value);
                    //     };
                    //     material.SetFloat(Metal, metallicConfig.Value);
                    // }
                    //
                    // if (material.HasProperty(OcclusionStrength))
                    // {
                    //     var occlusionConfig = FactionGearPlugin._plugin.config(SplitCamelCase(prefab.name),
                    //         "1 - " + material.name.Replace("(Instance)", "") + " Occlusion", material.GetFloat(OcclusionStrength),
                    //         new ConfigDescription("Set occlusion of material", new AcceptableValueRange<float>(0.1f, 1f)));
                    //     occlusionConfig.SettingChanged += (sender, args) =>
                    //     {
                    //         material.SetFloat(OcclusionStrength, occlusionConfig.Value);
                    //     };
                    //     material.SetFloat(OcclusionStrength, occlusionConfig.Value);
                    // }

                    var colorConfig = FactionGearPlugin._plugin.config(Tools.SplitCamelCase(prefab.name),
                        material.name.Replace("(Instance)", "") + " Color", material.color, "Set color of material");
                    colorConfig.SettingChanged += (sender, args) =>
                    {
                        material.color = colorConfig.Value;
                    };
                    material.color = colorConfig.Value;
                }
            }
        }
    }

    private static void AddMaterialConfigs(Item item) => MaterialConfigs.Add(item);
    private static void AddUnderArmor(Item item) => UnderArmor.Add(item);
    private static void AddUnderPants(Item item) => UnderPants.Add(item);


    public static void RegisterArmors()
    {
        Item NecroHelmet = new Item("factionarmors", "HelmetHounds");
        NecroHelmet.Name.English("Hounds Helmet");
        NecroHelmet.Description.English("NecroCrownHelmet");
        NecroHelmet.Crafting.Add(Managers.CraftingTable.Forge, 2);
        NecroHelmet.RequiredItems.Add("FlametalNew", 16);
        NecroHelmet.RequiredItems.Add("AskHide", 3);
        NecroHelmet.RequiredItems.Add("CharredBone", 2);
        NecroHelmet.RequiredItems.Add("Eitr", 4);
        NecroHelmet.CraftAmount = 1;
        NecroHelmet.RequiredUpgradeItems.Free = false;
        NecroHelmet.MaximumRequiredStationLevel = 2;
        NecroHelmet.Configurable = Configurability.Full;
        AddMaterialConfigs(NecroHelmet);

        if (NecroHelmet.Prefab.TryGetComponent(out ItemDrop houndsComponent))
        {
            houndsComponent.m_itemData.m_shared.m_weight = 3.0f;
            houndsComponent.m_itemData.m_shared.m_maxDurability = 800f;
            houndsComponent.m_itemData.m_shared.m_armor = 38;
            houndsComponent.m_itemData.m_shared.m_damageModifiers = new List<HitData.DamageModPair>()
            {
                new HitData.DamageModPair()
                {
                    m_type = HitData.DamageType.Blunt, m_modifier = HitData.DamageModifier.Resistant
                }
            };
        }
        
        Item NecroCrownHelmet = new Item("factionarmors", "HelmetHoundsKing");
        NecroCrownHelmet.Name.English("Hounds King Helmet");
        NecroCrownHelmet.Description.English("");
        NecroCrownHelmet.Crafting.Add(Managers.CraftingTable.Forge, 2);
        NecroCrownHelmet.RequiredItems.Add("FlametalNew", 16);
        NecroCrownHelmet.RequiredItems.Add("AskHide", 3);
        NecroCrownHelmet.RequiredItems.Add("CharredBone", 5);
        NecroCrownHelmet.RequiredItems.Add("Eitr", 4);
        NecroCrownHelmet.CraftAmount = 1;
        NecroCrownHelmet.RequiredUpgradeItems.Free = false;
        NecroCrownHelmet.MaximumRequiredStationLevel = 2;
        NecroCrownHelmet.Configurable = Configurability.Full;
        AddMaterialConfigs(NecroCrownHelmet);

        if (NecroCrownHelmet.Prefab.TryGetComponent(out ItemDrop kingComponent))
        {
            kingComponent.m_itemData.m_shared.m_weight = 3.0f;
            kingComponent.m_itemData.m_shared.m_maxDurability = 800f;
            kingComponent.m_itemData.m_shared.m_armor = 38;
            kingComponent.m_itemData.m_shared.m_damageModifiers = new List<HitData.DamageModPair>()
            {
                new HitData.DamageModPair()
                {
                    m_type = HitData.DamageType.Blunt, m_modifier = HitData.DamageModifier.Resistant
                }
            };
        }

        Item NecroChest = new Item("factionarmors", "ArmorHoundsChest");
        NecroChest.Name.English("Hounds Chest");
        NecroChest.Description.English("");
        NecroChest.Crafting.Add(Managers.CraftingTable.Forge, 2);
        NecroChest.RequiredItems.Add("FlametalNew", 20);
        NecroChest.RequiredItems.Add("AskHide", 3);
        NecroChest.RequiredItems.Add("CharredBone",5);
        NecroChest.RequiredItems.Add("YmirRemains", 10);
        NecroChest.CraftAmount = 1;
        NecroChest.RequiredUpgradeItems.Free = false;
        NecroChest.MaximumRequiredStationLevel = 2;
        NecroChest.Configurable = Configurability.Full;
        AddUnderArmor(NecroChest);
        if (NecroChest.Prefab.TryGetComponent(out ItemDrop chestComponent))
        {
            chestComponent.m_itemData.m_shared.m_movementModifier = -0.05f;
            chestComponent.m_itemData.m_shared.m_heatResistanceModifier = 0.2f;
            chestComponent.m_itemData.m_shared.m_weight = 10f;
            chestComponent.m_itemData.m_shared.m_maxDurability = 1000f;
            chestComponent.m_itemData.m_shared.m_armor = 38;
            chestComponent.m_itemData.m_shared.m_damageModifiers = new List<HitData.DamageModPair>()
            {
                new HitData.DamageModPair()
                {
                    m_type = HitData.DamageType.Frost, m_modifier = HitData.DamageModifier.Resistant
                },
                new HitData.DamageModPair()
                {
                    m_type = HitData.DamageType.Frost, m_modifier = HitData.DamageModifier.Resistant
                }
            };
        }
        
        Item NecroLegs = new Item("factionarmors", "ArmorHoundsLegs");
        NecroLegs.Name.English("Hounds Legs");
        NecroLegs.Description.English("");
        NecroLegs.Crafting.Add(Managers.CraftingTable.Forge, 2);
        NecroLegs.RequiredItems.Add("FlametalNew", 20);
        NecroLegs.RequiredItems.Add("AskHide", 3);
        NecroLegs.RequiredItems.Add("CharredBone", 5);
        NecroLegs.CraftAmount = 1;
        NecroLegs.RequiredUpgradeItems.Free = false;
        NecroLegs.MaximumRequiredStationLevel = 2;
        NecroLegs.Configurable = Configurability.Full;
        AddUnderPants(NecroLegs);

        if (NecroLegs.Prefab.TryGetComponent(out ItemDrop legsComponent))
        {
            legsComponent.m_itemData.m_shared.m_movementModifier = -0.05f;
            legsComponent.m_itemData.m_shared.m_heatResistanceModifier = 0.2f;
            legsComponent.m_itemData.m_shared.m_maxDurability = 1000f;
            legsComponent.m_itemData.m_shared.m_weight = 10f;
            legsComponent.m_itemData.m_shared.m_armor = 38;
            chestComponent.m_itemData.m_shared.m_damageModifiers = new List<HitData.DamageModPair>()
            {
                new HitData.DamageModPair()
                {
                    m_type = HitData.DamageType.Slash, m_modifier = HitData.DamageModifier.Resistant
                }
            };
        }

        Dictionary<string, string> helmets = new()
        {
            ["BalrogsHelmet"] = "Balrogs Helmet",
            ["DarkHelmet"] = "Dark Helmet",
            ["HelmetFreyr"] = "Helmet Freyr",
            ["HelmetDrage"] = "Helmet Dragemesters",
            ["HelmetFenrir"] = "Helmet Fenrir",
            ["HelmetFenrirAlt"] = "Helmet Fenrir Alt",
            ["HelmetFenrirDark"] = "Helmet Fenrir Dark"
        };
        Dictionary<string, string> chests = new()
        {
            ["BalrogsChest"] = "Balrogs Chest",
            ["ArmorDrageChest"] = "Dragemesters Chest",
            ["DarkChest"] = "Dark Chest",
            ["ArmorFreyrChest"] = "Freyr Chest",
            ["ArmorFenrirChest"] = "Fenrir Chest"
        };
        Dictionary<string, string> legs = new()
        {
            ["BalrogsLegs"] = "Balrogs Legs",
            ["ArmorDrageLegs"] = "Dragemesters Legs",
            ["DarkLegs"] = "Dark Legs",
            ["ArmorFreyrLegs"] = "Freyr Legs",
            ["ArmorFenrirLegs"] = "Fenrir Legs"
        };
        foreach (var kvp in helmets)
        {
            Item item = new Item("factionarmors", kvp.Key);
            item.Name.English(kvp.Value);
            item.Description.English("");
            item.Crafting.Add(Managers.CraftingTable.Forge, 2);
            item.RequiredItems.Add("FlametalNew", 16);
            item.RequiredItems.Add("AskHide", 3);
            item.RequiredItems.Add("CharredBone", 5);
            item.RequiredItems.Add("Eitr", 10);
            item.CraftAmount = 1;
            item.RequiredUpgradeItems.Free = false;
            item.MaximumRequiredStationLevel = 2;
            item.Configurable = Configurability.Full;

            if (item.Prefab.TryGetComponent(out ItemDrop component))
            {
                component.m_itemData.m_shared.m_weight = 3.0f;
                component.m_itemData.m_shared.m_maxDurability = 800f;
                component.m_itemData.m_shared.m_armor = 38;
                component.m_itemData.m_shared.m_damageModifiers = new List<HitData.DamageModPair>()
                {
                    new HitData.DamageModPair()
                    {
                        m_type = HitData.DamageType.Blunt, m_modifier = HitData.DamageModifier.Resistant
                    }
                };
            }
        }
        foreach (var kvp in chests)
        {
            Item item = new Item("factionarmors", kvp.Key);
            item.Name.English(kvp.Value);
            item.Description.English("");
            item.Crafting.Add(Managers.CraftingTable.Forge, 2);
            item.RequiredItems.Add("FlametalNew", 16);
            item.RequiredItems.Add("AskHide", 3);
            item.RequiredItems.Add("CharredBone", 5);
            item.RequiredItems.Add("YmirRemains", 10);
            item.CraftAmount = 1;
            item.RequiredUpgradeItems.Free = false;
            item.MaximumRequiredStationLevel = 2;
            item.Configurable = Configurability.Full;

            if (item.Prefab.TryGetComponent(out ItemDrop component))
            {
                component.m_itemData.m_shared.m_movementModifier = -0.05f;
                component.m_itemData.m_shared.m_heatResistanceModifier = 0.2f;
                component.m_itemData.m_shared.m_weight = 10f;
                component.m_itemData.m_shared.m_maxDurability = 1000f;
                component.m_itemData.m_shared.m_armor = 38;
                component.m_itemData.m_shared.m_damageModifiers = new List<HitData.DamageModPair>()
                {
                    new HitData.DamageModPair()
                    {
                        m_type = HitData.DamageType.Fire, m_modifier = HitData.DamageModifier.Resistant
                    },
                    new HitData.DamageModPair()
                    {
                        m_type = HitData.DamageType.Frost, m_modifier = HitData.DamageModifier.Resistant
                    }
                };
            }
        }
        foreach (var kvp in legs)
        {
            Item item = new Item("factionarmors", kvp.Key);
            item.Name.English(kvp.Value);
            item.Description.English("");
            item.Crafting.Add(Managers.CraftingTable.Forge, 2);
            item.RequiredItems.Add("FlametalNew", 16);
            item.RequiredItems.Add("AskHide", 30);
            item.RequiredItems.Add("CharredBone", 5);
            item.RequiredItems.Add("YmirRemains", 10);
            item.CraftAmount = 1;
            item.RequiredUpgradeItems.Free = false;
            item.MaximumRequiredStationLevel = 2;
            item.Configurable = Configurability.Full;

            if (item.Prefab.TryGetComponent(out ItemDrop component))
            {
                component.m_itemData.m_shared.m_movementModifier = -0.05f;
                component.m_itemData.m_shared.m_heatResistanceModifier = 0.2f;
                component.m_itemData.m_shared.m_maxDurability = 1000f;
                component.m_itemData.m_shared.m_weight = 10f;
                component.m_itemData.m_shared.m_armor = 38;
                component.m_itemData.m_shared.m_damageModifiers = new List<HitData.DamageModPair>()
                {
                    new HitData.DamageModPair()
                    {
                        m_type = HitData.DamageType.Slash, m_modifier = HitData.DamageModifier.Resistant
                    }
                };
            }
        }

    }

    private static void CreateAshMageClones(ObjectDB __instance)
    {
        var helmet = __instance.GetItemPrefab("HelmetMage_Ashlands");
        var chest = __instance.GetItemPrefab("ArmorMageChest_Ashlands");
        var legs = __instance.GetItemPrefab("ArmorMageLegs_Ashlands");

        if (!helmet || !chest || !legs) return;

        if (!helmet.TryGetComponent(out ItemDrop helmetDrop)) return;
        if (!chest.TryGetComponent(out ItemDrop chestDrop)) return;
        if (!legs.TryGetComponent(out ItemDrop legsDrop)) return;

        var helmetRecipe = __instance.GetRecipe(helmetDrop.m_itemData);
        var chestRecipe = __instance.GetRecipe(chestDrop.m_itemData);
        var legRecipe = __instance.GetRecipe(legsDrop.m_itemData);

        IterateList(TextureManager.ashMageHelmets, helmet, __instance, helmetRecipe);
        IterateList(TextureManager.ashMageChest, chest, __instance, chestRecipe);
        IterateList(TextureManager.ashMageLegs, legs, __instance, legRecipe);

    }

    private static void IterateList(Dictionary<Sprite?, Texture2D?> list, GameObject prefab, ObjectDB __instance, Recipe recipe)
    {
        foreach (var kvp in list)
        {
            if (kvp.Key == null || kvp.Value == null) continue;
            var icon = kvp.Key;
            var texture = kvp.Value;

            var clone = Object.Instantiate(prefab, FactionGearPlugin._Root.transform, false);
            clone.name = icon.name;
            
            CreateClone(clone, recipe, __instance, icon,  texture);
        }
    }

    private static void CreateClone(GameObject clone, Recipe recipe, ObjectDB __instance, Sprite icon, Texture2D texture)
    {
        var nameConfig = FactionGearPlugin._plugin.config(Tools.SplitCamelCase(clone.name), "Display Name", clone.name,
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