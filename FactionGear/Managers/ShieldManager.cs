using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using UnityEngine;

namespace FactionGear.Managers;

public static class ShieldManager
{
    private static readonly int StyleTex = Shader.PropertyToID("_StyleTex");
    private static bool m_initiated;

    [HarmonyPatch(typeof(ObjectDB), nameof(ObjectDB.Awake))]
    private static class RegisterShieldStyles
    {
        private static void Postfix(ObjectDB __instance)
        {
            if (!__instance || !ZNetScene.instance) return;
            if (m_initiated) return;
            SetShieldStyle("ShieldBanded", TextureManager.BandedShieldIcons, TextureManager.ShieldPaint_WoodBanded);
            SetShieldStyle("ShieldWood", TextureManager.WoodShieldIcons, TextureManager.ShieldPaint_WoodBanded);
            SetShieldStyle("ShieldBlackmetal", TextureManager.BlackMetalShieldIcons, TextureManager.ShieldPaint_BlackMetal);
            SetShieldStyle("ShieldBlackmetalTower", TextureManager.BlackMetalTowerShieldIcons, TextureManager.ShieldPaint_BlackMetal);
            SetShieldStyle("ShieldFlametal", TextureManager.FlametalShieldIcons, TextureManager.ShieldPaint_Flametal);
            SetShieldStyle("ShieldFlametalTower", TextureManager.FlametalTowerShieldIcons, TextureManager.ShieldPaint_Flametal);
            m_initiated = true;
        }
    }

    private static void SetShieldStyle(string prefabName, List<Sprite?> icons, Texture2D? style)
    {
        GameObject shield = ObjectDB.instance.GetItemPrefab(prefabName);
        if (!shield) return;
        if (!shield.TryGetComponent(out ItemDrop component)) return;
        List<Sprite?> m_icons = component.m_itemData.m_shared.m_icons.ToList();
        m_icons.AddRange(icons);

        component.m_itemData.m_shared.m_icons = m_icons.ToArray();
        component.m_itemData.m_shared.m_variants = m_icons.Count;

        MeshRenderer? renderer = shield.GetComponentInChildren<MeshRenderer>();
        foreach (Material? material in renderer.materials)
        {
            if (material.HasProperty(StyleTex))
            {
                material.SetTexture(StyleTex, style);
            }
        }
    }
}