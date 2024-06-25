using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System.IO;

public class WeaponDatabase : ItemDatabase<Weapon>
{
    private List<Weapon> weaponItems;
    private Weapon selectedWeapon = null;
    private Vector2 scrollPos;
    private const string WEAPON_ASSET_PATH = "Assets/Items/Weapons/";

    [MenuItem("Window/Item Manager/Weapon Database")]
    public static void ShowWindow()
    {
        GetWindow<WeaponDatabase>("Weapon Database");
    }

    [Header("Weapon Database")]
    public bool showWeaponCreationButton = true;

    private void OnGUI()
    {
        if (showWeaponCreationButton)
        {
            if (GUILayout.Button("Create New Weapon"))
            {
                WeaponCreation.ShowWindow();
            }
        }

        Weapon[] weapons = AssetDatabase.FindAssets("t:Weapon", new[] { WEAPON_ASSET_PATH })
                          .Select(AssetDatabase.GUIDToAssetPath)
                          .Select(AssetDatabase.LoadAssetAtPath<Weapon>)
                          .ToArray();
        
        // Display the list of weapons
        if (weapons.Length > 0)
        {
            EditorGUILayout.BeginVertical();
            EditorGUILayout.LabelField("Weapon List", EditorStyles.boldLabel);

            // Create a ScrollView to make the list scrollable
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos, false, true, GUILayout.ExpandHeight(true));

            for (int x = 0; x < weapons.Length; x++)
            {
                bool isSelected = selectedWeapon != null && selectedWeapon.itemName == weapons[x].itemName;
                EditorGUILayout.BeginVertical();
                if (GUILayout.Button(new GUIContent(weapons[x].itemName, weapons[x].icon != null ? weapons[x].icon.texture : null), isSelected ? EditorStyles.boldLabel : EditorStyles.label, GUILayout.Width(100)))
                {
                    if (isSelected)
                    {
                        selectedWeapon = null;
                    }
                    else
                    {
                        selectedWeapon = weapons[x];
                    }
                }

                EditorGUILayout.EndVertical();
            }
            // End the ScrollView
            EditorGUILayout.EndScrollView();

            EditorGUILayout.EndVertical();
        }

        // Delete item and confirm
        if (selectedWeapon != null)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(selectedWeapon.itemName, EditorStyles.boldLabel);
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Delete"))
            {
                bool Delete = EditorUtility.DisplayDialog($"Delete {selectedWeapon.itemName}?", $"Are you want to delete {selectedWeapon.itemName}?", "Yes", "No");
                if (Delete)
                {
                    DeleteWeapon(selectedWeapon);
                    selectedWeapon = null;
                }
            }
            EditorGUILayout.EndHorizontal();
        }

        DrawItemList();

        DrawPropertiesSection();
    }

    private void DeleteWeapon(Weapon weapon)
    {
        if (weapon != null)
        {
            AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(weapon));
            AssetDatabase.Refresh();
        }
    }

    protected override void DrawItemList()
    {
        EditorGUILayout.LabelField("Weapon Properties", EditorStyles.boldLabel);

        if (selectedWeapon != null)
        {
            EditorGUILayout.LabelField("Name: " + selectedWeapon.itemName);
            EditorGUILayout.LabelField("Attack Power: " + selectedWeapon.attackPower);
            EditorGUILayout.LabelField("Attack Speed: " + selectedWeapon.attackSpeed);
            EditorGUILayout.LabelField("Durability: " + selectedWeapon.durability);
            EditorGUILayout.LabelField("Range: " + selectedWeapon.range);
            EditorGUILayout.LabelField("Critical Hit Chance: " + selectedWeapon.criticalHitChance);
            EditorGUILayout.LabelField("Weapon Type: " + selectedWeapon.weaponType);
        }
        else
        {
            EditorGUILayout.LabelField("No weapon selected");
        }
    }

    // Handles the display and modification of the weapon properties
    protected override void DrawPropertiesSection()
    {
        EditorGUILayout.LabelField("Weapon Properties", EditorStyles.boldLabel);

        if (selectedWeapon != null)
        {
            // Name
            string newName = EditorGUILayout.TextField("Name", selectedWeapon.itemName, GUILayout.Width(200f));
            if (newName != selectedWeapon.itemName && !IsValidItemName(newName))
            {
                EditorGUILayout.HelpBox("Item name cannot contain special characters (except spaces, dashes, and single quotes).", MessageType.Error);
            }
            else
            {
                selectedWeapon.itemName = newName;
            }

            // Attack Power
            float newAttackPower = EditorGUILayout.FloatField("Attack Power", selectedWeapon.attackPower, GUILayout.Width(200f));
            if (newAttackPower < 0f)
            {
                EditorGUILayout.HelpBox("Attack power must be a non-negative value.", MessageType.Error);
            }
            else
            {
                selectedWeapon.attackPower = newAttackPower;
            }

            // Attack Speed
            float newAttackSpeed = EditorGUILayout.FloatField("Attack Speed", selectedWeapon.attackSpeed, GUILayout.Width(200f));
            if (newAttackSpeed < 0f)
            {
                EditorGUILayout.HelpBox("Attack speed must be a non-negative value.", MessageType.Error);
            }
            else
            {
                selectedWeapon.attackSpeed = newAttackSpeed;
            }

            // Durability
            float newDurability = EditorGUILayout.FloatField("Durability", selectedWeapon.durability, GUILayout.Width(200f));
            if (newDurability < 0)
            {
                EditorGUILayout.HelpBox("Durability must be a non-negative value.", MessageType.Error);
            }
            else
            {
                selectedWeapon.durability = newDurability;
            }

            // Range
            float newRange = EditorGUILayout.FloatField("Range", selectedWeapon.range, GUILayout.Width(200f));
            if (newRange < 0f)
            {
                EditorGUILayout.HelpBox("Range must be a non-negative value.", MessageType.Error);
            }
            else
            {
                selectedWeapon.range = newRange;
            }

            // Critical Hit Chance
            float newCriticalHitChance = EditorGUILayout.FloatField("Critical Hit Chance", selectedWeapon.criticalHitChance, GUILayout.Width(200f));
            if (newCriticalHitChance < 0f || newCriticalHitChance > 1f)
            {
                EditorGUILayout.HelpBox("Critical hit chance must be between 0 and 1.", MessageType.Error);
            }
            else
            {
                selectedWeapon.criticalHitChance = newCriticalHitChance;
            }

            // Weapon Type
            selectedWeapon.weaponType = (WeaponType)EditorGUILayout.EnumPopup("Weapon Type", selectedWeapon.weaponType, GUILayout.Width(200f));
        }
    }

    private bool IsValidItemName(string name)
    {
        // Check if the name contains only allowed characters
        return string.IsNullOrEmpty(name) || name.All(c => char.IsLetterOrDigit(c) || c == ' ' || c == '-' || c == '\'');
    }

    protected override void ExportItemsToCSV()
    {
        
    }

    protected override void ImportItemsFromCSV()
    {

    }
}