using System.Collections;
using UnityEngine;
using UnityEditor;
using System.IO;

public class DatabasesManager : EditorWindow
{
    private ArmorDatabase armorDatabase;
    private PotionDatabase potionDatabase;
    private WeaponDatabase weaponDatabase;

    
    [MenuItem("Window/Item Manager/Databases Manager")]
    public static void ShowWindow()
    {
        GetWindow<DatabasesManager>("Databases Manager");
    }

    private void Start()
    {
        weaponDatabase = new WeaponDatabase();
        potionDatabase = new PotionDatabase();
        armorDatabase = new ArmorDatabase();
    }

    private void OnGUI()
    {
        // Title
        EditorGUILayout.LabelField("Item Database Manager", EditorStyles.boldLabel);

        // Description
        EditorGUILayout.HelpBox(
            "Welcome to the Item Database Manager! Here, you can access and manage different databases related to weapons, potions, armors, and other items.",
            MessageType.Info);

        // Information about overall data
        GUILayout.Label("Overall Statistics:", EditorStyles.boldLabel);
        GUILayout.Label($"Total Items: {AssetDatabase.FindAssets("t:BaseItem").Length}");
        GUILayout.Label($"Total Weapons: {AssetDatabase.FindAssets("t:Weapon").Length}");
        GUILayout.Label($"Total Potions: {AssetDatabase.FindAssets("t:Potion").Length}");
        GUILayout.Label($"Total Armors: {AssetDatabase.FindAssets("t:Armor").Length}");
        

        // Separator
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        EditorGUILayout.Space();

        // Buttons to access other windows

        void OpenPotionDatabase()
        {
            if (GUILayout.Button("Potion Database"))
            {
                if (potionDatabase == null)
                {
                    potionDatabase = new PotionDatabase();
                }
                PotionDatabase.ShowWindow();
            }
        }

        void OpenWeaponDatabase()
        {
            if (GUILayout.Button("Weapon Database"))
            {
                if (weaponDatabase == null)
                {
                    weaponDatabase = new WeaponDatabase();
                }
                WeaponDatabase.ShowWindow();
            }
        }

        void OpenArmorDatabase()
        {
            if (GUILayout.Button("Armor Database"))
            {
                if (armorDatabase == null)
                {
                    armorDatabase = new ArmorDatabase();
                }
                ArmorDatabase.ShowWindow();
            }
        }
    }
}