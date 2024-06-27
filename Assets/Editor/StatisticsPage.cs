using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;
using System.IO;

public class Statistics : EditorWindow
{
    GUIStyle largeBoldGreenStyle = new GUIStyle(EditorStyles.largeLabel);
    private GUIStyle blueStyle;

    private void OnEnable()
    {
        // Initialize the large, bold, green style
        largeBoldGreenStyle = new GUIStyle(EditorStyles.largeLabel);
        largeBoldGreenStyle.fontStyle = FontStyle.Bold;
        largeBoldGreenStyle.fontSize = 15;
        largeBoldGreenStyle.normal.textColor = Color.green;

        // Initialize the blue style
        blueStyle = new GUIStyle(EditorStyles.miniLabel);
        blueStyle.normal.textColor = new Color32(0, 255, 255, 255);
    }
    
    public void DisplayGeneralStatistics()
    {
        // Information about overall data
        GUILayout.Label("Overall Statistics", largeBoldGreenStyle);
        GUILayout.Label($"Total Items: {AssetDatabase.FindAssets("t:BaseItem").Length}");
        GUILayout.Label($"Total Weapons: {AssetDatabase.FindAssets("t:Weapon").Length}");
        GUILayout.Label($"Total Potions: {AssetDatabase.FindAssets("t:Potion").Length}");
        GUILayout.Label($"Total Armors: {AssetDatabase.FindAssets("t:Armor").Length}");
    }

    public void DisplayWeaponStatistics()
    {
        EditorGUILayout.LabelField("Weapon Statistics", largeBoldGreenStyle);
        EditorGUILayout.Space();

        // Description
        EditorGUILayout.LabelField("Listed are the statistics on the weapons in the database.", blueStyle);

        List<Weapon> weapons = LoadAllAssets<Weapon>();
        float averageAttackPower = 0f;
        float averageAttackSpeed = 0f;
        float averageDurability = 0f;
        float averageCriticalHitChance = 0f;
        float averageRange = 0f;

        foreach (var weapon in weapons)
        {
            averageAttackPower += weapon.attackPower;
            averageAttackSpeed += weapon.attackSpeed;
            averageDurability += weapon.durability;
            averageCriticalHitChance += weapon.criticalHitChance;
            averageRange += weapon.range;
        }

        if (weapons.Count > 0)
        {
            averageAttackPower /= weapons.Count;
            averageAttackSpeed /= weapons.Count;
            averageDurability /= weapons.Count;
            averageCriticalHitChance /= weapons.Count;
            averageRange /= weapons.Count;
        }

        EditorGUILayout.LabelField($"Average Attack Power: {averageAttackPower:F2}");
        EditorGUILayout.LabelField($"Average Crit Chance: {averageAttackSpeed:F2}%");
        EditorGUILayout.LabelField($"Average Crit Damage: {averageDurability:F2}%");
        EditorGUILayout.LabelField($"Average Critical Hit Chance: {averageCriticalHitChance:F2}%");
        EditorGUILayout.LabelField($"Average Range: {averageRange:F2}");
    }

    public void DisplayArmorStatistics()
    {
        EditorGUILayout.LabelField("Armor Statistics", largeBoldGreenStyle);
        EditorGUILayout.Space();

        // Description
        EditorGUILayout.LabelField("Listed are the statistics on the armor in the database.", blueStyle);

        List<Armor> armors = LoadAllAssets<Armor>();

        float totalDefensePower = 0f;
        float totalResistance = 0f;
        float totalWeight = 0f;
        float totalMovementSpeedModifier = 0f;
        int totalArmorCount = armors.Count;

        foreach (var armor in armors)
        {
            totalDefensePower += armor.defensePower;
            totalResistance += armor.resistance;
            totalWeight += armor.weight;
            totalMovementSpeedModifier += armor.movementSpeedModifier;
        }

        EditorGUILayout.LabelField($"Total Armor: {totalArmorCount}");
        EditorGUILayout.LabelField($"Average Defense Power: {totalDefensePower / totalArmorCount:F2}");
        EditorGUILayout.LabelField($"Average Resistance: {totalResistance / totalArmorCount:F2}");
        EditorGUILayout.LabelField($"Average Weight: {totalWeight / totalArmorCount:F2}");
        EditorGUILayout.LabelField($"Average Movement Speed Modifier: {totalMovementSpeedModifier / totalArmorCount:F2}");
        EditorGUILayout.LabelField("Armor Type Counts:");
    }

    public void DisplayPotionStatistics()
    {

        EditorGUILayout.LabelField("Potion Statistics", largeBoldGreenStyle);

        // Description
        EditorGUILayout.LabelField("Listed are the statistics on the potions in the database.", blueStyle);
        EditorGUILayout.Space();
        
        List<Potion> potions = LoadAllAssets<Potion>();
        Dictionary<PotionEffect, int> potionEffectCount = new Dictionary<PotionEffect, int>();

        foreach (var potion in potions)
        {
            if (!potionEffectCount.ContainsKey(potion.potionEffect))
            {
                potionEffectCount[potion.potionEffect] = 0;
            }
            potionEffectCount[potion.potionEffect]++;
        }

        EditorGUILayout.LabelField("Most Common Potion Effects:");
        var mostCommonEffect = potionEffectCount.OrderByDescending(x => x.Value).FirstOrDefault();
        EditorGUILayout.LabelField($"{mostCommonEffect.Key}: {mostCommonEffect.Value}");
    }

    private List<T> LoadAllAssets<T>() where T : ScriptableObject
    {
        List<T> assets = new List<T>();
        string[] guids = AssetDatabase.FindAssets($"t:{typeof(T).Name}");
        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            T asset = AssetDatabase.LoadAssetAtPath<T>(path);
            assets.Add(asset);
        }
        return assets;
    }

    //public void DrawBarGraph(Rect rect, float[] values, string[] labels)
    //{
        //float maxValue = Mathf.Max(values);
        //float barWidth = rect.width / values.Length;

        //for (int i = 0; i < values.Length; i++)
        //{
            //float barX = rect.x + i * barWidth;
            //float barY = rect.y + rect.height - values[i] / maxValue * rect.height;
            //float barW = barWidth * 0.8f; // Reduce bar width slightly to add spacing
            //float barH = values[i] / maxValue * rect.height;

            // Draw the bar
            //GUI.DrawTexture(new Rect(barX, barY, barW, barH), Texture2D.whiteTexture, ScaleMode.StretchToFill);

            // Draw the label
            //float labelY = rect.y + rect.height + 10; // Position labels below the bars
            //GUI.Label(new Rect(barX, labelY, barW, 20), labels[i]);
        //}
    //}
}