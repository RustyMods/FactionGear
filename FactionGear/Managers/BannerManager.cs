using System;
using System.Collections.Generic;
using HarmonyLib;
using UnityEngine;
using Object = UnityEngine.Object;

namespace FactionGear.Managers;

public static class BannerManager
{
    private static readonly int MainTex = Shader.PropertyToID("_MainTex");
    private static readonly int BumpMap = Shader.PropertyToID("_BumpMap");
    private static bool m_iniated;

    [HarmonyPatch(typeof(ObjectDB), nameof(ObjectDB.Awake))]
    private static class RegisterFactionBanners
    {
        private static void Postfix(ObjectDB __instance)
        {
            if (!__instance || !ZNetScene.instance) return;
            if (m_iniated) return;
            var hammer = __instance.GetItemPrefab("Hammer");
            if (!hammer.TryGetComponent(out ItemDrop component)) return;
            var banner = ZNetScene.instance.GetPrefab("piece_banner01");
            if (!banner) return;

            foreach (var kvp in TextureManager.banners)
            {
                var icon = kvp.Key;
                var texture = kvp.Value;
                if (icon == null || texture == null) continue;

                var clone = Object.Instantiate(banner, FactionGearPlugin._Root.transform, false);
                clone.name = texture.name;

                foreach (var renderer in clone.GetComponentsInChildren<Renderer>(true))
                {
                    foreach (var material in renderer.materials)
                    {
                        if (material.shader.name != "Custom/Vegetation") continue;
                        material.SetTexture(MainTex, texture);
                        material.SetTexture(BumpMap, null);
                    }
                }
                
                if (!clone.TryGetComponent(out Piece piece)) continue;

                piece.m_icon = icon;

                var name = FactionGearPlugin._plugin.config(Tools.SplitCamelCase(clone.name), "Display Name", Tools.SplitCamelCase(clone.name),
                    "Set the display name");

                piece.m_name = name.Value;
                name.SettingChanged += (sender, args) =>
                {
                    piece.m_name = name.Value;
                };

                var description = FactionGearPlugin._plugin.config(Tools.SplitCamelCase(clone.name), "Description", "",
                    "Set description of banner");
                piece.m_description = description.Value;
                description.SettingChanged += (sender, args) =>
                {
                    piece.m_description = description.Value;
                };

                var reqs = FormatPieceRecipe(piece.m_resources);
                var recipe = FactionGearPlugin._plugin.config(Tools.SplitCamelCase(clone.name), "Recipe", reqs,
                    "Set recipe of banner, [prefabName]:[amount]");

                recipe.SettingChanged += (sender, args) =>
                {
                    piece.m_resources = Tools.GetRecipeResources(recipe.Value, __instance).ToArray();
                };

                var category = FactionGearPlugin._plugin.config(Tools.SplitCamelCase(clone.name), "Category",
                    piece.m_category.ToString(), "Set category");
                category.SettingChanged += (sender, args) =>
                {
                    if (Enum.TryParse(category.Value, out Piece.PieceCategory pieceCategory))
                    {
                        piece.m_category = pieceCategory;
                    }
                };
                
                component.m_itemData.m_shared.m_buildPieces.m_pieces.Add(clone);

                Tools.RegisterToZNetScene(clone);
            }

            m_iniated = true;
        }
    }

    private static string FormatPieceRecipe(Piece.Requirement[] requirements)
    {
        List<string> output = new();
        foreach (var req in requirements)
        {
            var name = req.m_resItem.name;
            var amount = req.m_amount;
            var format = name + ":" + amount;
            output.Add(format);
        }

        return string.Join(",", output);
    }
}