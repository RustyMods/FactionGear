
using System.Collections.Generic;
using System.Reflection;

using UnityEngine;

namespace FactionGear.Managers;

public static class TextureManager
{

    public static readonly Texture2D? ShieldPaint_WoodBanded = RegisterTexture("ShieldPaint_WoodBanded.png", "ShieldPaints");
    public static readonly Texture2D? ShieldPaint_BlackMetal = RegisterTexture("ShieldPaint_BlackMetal.png", "ShieldPaints");
    public static readonly Texture2D? ShieldPaint_Flametal = RegisterTexture("ShieldPaint_Flametal.png", "ShieldPaints");
    
    private static readonly Sprite? IronGate_Icon = RegisterSprite("IronGate_logo.png", "misc");

    private static readonly Sprite? CapeDeer_Fenrir = RegisterSprite("CapeDeerHideFenrir.png", "CapeDeer");
    private static readonly Sprite? CapeDeer_Freyr = RegisterSprite("CapeDeerHideFreyr.png", "CapeDeer");
    private static readonly Sprite? CapeDeer_Hounds = RegisterSprite("CapeDeerHideHounds.png", "CapeDeer");
    private static readonly Sprite? CapeDeer_Drage = RegisterSprite("CapeDeerHideDragemesters.png", "CapeDeer");

    public static readonly Texture2D? BaseFeatureCape_Tex = RegisterTexture("CapeFeatherFenrir.png", "CapeFeather");

    private static readonly Sprite? CapeFeatherFenrir_Icon = RegisterSprite("CapeFeathersFenrir_Icon.png", "CapeFeather");
    private static readonly Sprite? CapeFeatherFreyr_Icon = RegisterSprite("CapeFeathersFreyr_Icon.png", "CapeFeather");
    private static readonly Sprite? CapeFeatherHounds_Icon = RegisterSprite("CapeFeathersHounds_Icon.png", "CapeFeather");
    private static readonly Sprite? CapeFeatherDrage_Icon = RegisterSprite("CapeFeathersDragemesters_Icon.png", "CapeFeather");

    public static readonly Dictionary<Sprite?, Color> featherClones = new()
    {
        { CapeFeatherFenrir_Icon, new Color(1f, 1f, 1f, 1f) },
        { CapeFeatherDrage_Icon, new Color(1f, 1f, 0f, 1f) },
        { CapeFeatherFreyr_Icon, new Color(0f, 1f, 1f, 1f) },
        { CapeFeatherHounds_Icon, new Color(1f, 0f, 0f, 1f) }
    };

    public static readonly Dictionary<Sprite?, Color> deerClones = new()
    {
        {CapeDeer_Fenrir, new Color(1f, 1f, 1f, 1f)},
        {CapeDeer_Freyr, new Color(0f, 1f, 1f, 1f)},
        {CapeDeer_Hounds, new Color(1f, 0f, 0f, 1f)},
        {CapeDeer_Drage, new Color(1f, 1f, 0 ,1f)}
    };

    public static readonly List<Sprite?> WoodShieldIcons = new();
    public static readonly List<Sprite?> BandedShieldIcons = new();
    public static readonly List<Sprite?> BlackMetalShieldIcons = new();
    public static readonly List<Sprite?> BlackMetalTowerShieldIcons = new();
    public static readonly List<Sprite?> FlametalShieldIcons = new();
    public static readonly List<Sprite?> FlametalTowerShieldIcons = new();

    public static readonly Dictionary<Sprite?, Texture2D?> loxClones = new();

    public static readonly Dictionary<Sprite?, Texture2D?> ashClones = new();
    public static readonly Dictionary<Sprite?, Texture2D?> ashMageHelmets = new();
    public static readonly Dictionary<Sprite?, Texture2D?> ashMageChest = new();
    public static readonly Dictionary<Sprite?, Texture2D?> ashMageLegs = new();

    public static readonly Dictionary<Sprite?, Texture2D?> banners = new();

    private static readonly Dictionary<string, Texture2D?> ashMageTextures = new()
    {
        { "Drage", RegisterTexture("DrageTex.png", "AshMage") },
        { "Fenrir", RegisterTexture("FenrirTex.png", "AshMage") },
        { "Freyr", RegisterTexture("FreyrTex.png", "AshMage") },
        { "Hounds", RegisterTexture("HoundsTex.png", "AshMage") }
    };

    public static void PrepareBannerClones()
    {
        List<string> assetNames = new()
        {
            "Dragemesters", "Fenrir", "Freyr", "Hounds", "Server"
        };
        foreach (var name in assetNames)
        {
            banners[RegisterSprite("Banner" + name + "_Icon.png", "Banners")] = RegisterTexture("Banner" + name + ".png", "Banners");
        }
    }

    public static void PrepareAshMageClones()
    {
        List<string> assetNames = new()
        {
            "Drage", "Fenrir", "Freyr", "Hounds"
        };
        foreach (var name in assetNames)
        {
            ashMageHelmets[RegisterSprite(name + "Helmet_Icon.png", "AshMage")] = ashMageTextures[name];
            ashMageChest[RegisterSprite(name + "Chest_Icon.png", "AshMage")] = ashMageTextures[name];
            ashMageLegs[RegisterSprite(name + "Legs_Icon.png", "AshMage")] = ashMageTextures[name];
        }
    }

    public static void PrepareShieldClones()
    {
        List<string> assetNames = new()
        {
            "Hounds", "Freyr", "Fenrir", "Dragemesters"
        };
        Dictionary<string, List<Sprite?>> folderMap = new()
        {
            { "BandedShield", BandedShieldIcons },
            { "WoodShield", WoodShieldIcons },
            { "BlackmetalShield", BlackMetalShieldIcons },
            { "BlackmetalTowerShield", BlackMetalTowerShieldIcons },
            { "FlametalShield", FlametalShieldIcons },
            { "FlametalTowerShield", FlametalTowerShieldIcons }
        };
        foreach (var kvp in folderMap)
        {
            var folderName = kvp.Key;
            if (folderName.StartsWith("Flametal"))
            {
                kvp.Value.Add(IronGate_Icon);
            }
            foreach (var name in assetNames)
            {
                kvp.Value.Add(RegisterSprite(folderName + name + "_Icon.png", folderName + "Icons"));
            }
        }
        
    }

    public static void PrepareAshClones()
    {
        List<string> assetNames = new()
        {
            "Drage", "Hounds", "Freyr", "Fenrir"
        };
        foreach (string name in assetNames)
        {
            ashClones[RegisterSprite(name + "_Ash_Icon.png", "CapeAsh")] = RegisterTexture(name + "_Ash.png", "CapeAsh");
        }
    }

    public static void PrepareLoxClones()
    {
        List<string> assetNames = new()
        {
            "CapeLoxDragemesters", "CapeLoxFenrir", "CapeLoxFreyr", "CapeLoxHounds"
        };
        foreach (string name in assetNames)
        {
            loxClones[RegisterSprite(name + "_Icon.png", "CapeLox")] = RegisterTexture(name + ".png", "CapeLox");
        }
    }
    
    
    private static Sprite? RegisterSprite(string fileName, string folderName, string parentFolder = "assets")
    {
        Assembly assembly = Assembly.GetExecutingAssembly();

        string path = $"{FactionGearPlugin.ModName}.{parentFolder}.{folderName}.{fileName}";
        using var stream = assembly.GetManifestResourceStream(path);
        if (stream == null) return null;
        byte[] buffer = new byte[stream.Length];
        stream.Read(buffer, 0, buffer.Length);
        Texture2D texture = new Texture2D(2, 2);
        Sprite? sprite = texture.LoadImage(buffer) ? Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero) : null;
        if (sprite != null)
        {
            sprite.name = fileName.Replace(".png", string.Empty).Replace("_Icon", string.Empty).Replace("_", "");
        }
        return sprite;
    }
    
    private static Texture2D? RegisterTexture(string fileName, string folderName, string parentFolder = "assets")
    {
        Assembly assembly = Assembly.GetExecutingAssembly();

        string path = $"{FactionGearPlugin.ModName}.{parentFolder}.{folderName}.{fileName}";
        using var stream = assembly.GetManifestResourceStream(path);
        if (stream == null) return null;
        byte[] buffer = new byte[stream.Length];
        stream.Read(buffer, 0, buffer.Length);
        Texture2D texture = new Texture2D(2, 2);

        texture.name = fileName.Replace(".png", string.Empty).Replace("_", "");

        return texture.LoadImage(buffer) ? texture : null;
    }
}