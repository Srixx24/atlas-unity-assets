using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System.IO;

public class ArmorDatabase : ItemDatabase<Armor>
{
    private List<Armor> armorItems;
    private Armor selectedArmor = null;
    private Vector2 scrollPos;
    private const string ARMOR_ASSET_PATH = "Assets/Items/Armor/";

    [MenuItem("Window/Item Manager/Armor Database")]
    public static void ShowWindow()
    {
        GetWindow<ArmorDatabase>("Armor Database");
    }

    [Header("Armor Database")]
    public bool showArmorCreationButton = true;

    private void OnGUI()
    {
        if (showArmorCreationButton)
        {
            if (GUILayout.Button("Create New Armor"))
            {
                ArmorCreation.ShowWindow();
            }
        }

        Armor[] armors = AssetDatabase.FindAssets("t:Armor", new[] { ARMOR_ASSET_PATH })
                        .Select(AssetDatabase.GUIDToAssetPath)
                        .Select(AssetDatabase.LoadAssetAtPath<Armor>)
                        .ToArray();
    
        // Display the list of armors
        if (armors.Length > 0)
        {
            EditorGUILayout.BeginVertical();
            EditorGUILayout.LabelField("Armor List", EditorStyles.boldLabel);

            // Create a ScrollView to make the list scrollable
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos, false, true, GUILayout.ExpandHeight(true));

            for (int x = 0; x < armors.Length; x++)
            {
                bool isSelected = selectedArmor != null && selectedArmor.itemName == armors[x].itemName;
                EditorGUILayout.BeginVertical();
                if (GUILayout.Button(new GUIContent(armors[x].itemName, armors[x].icon != null ? armors[x].icon.texture : null), isSelected ? EditorStyles.boldLabel : EditorStyles.label, GUILayout.Width(100)))
                {
                    if (isSelected)
                    {
                        selectedArmor = null;
                    }
                    else
                    {
                        selectedArmor = armors[x];
                    }
                }

                EditorGUILayout.EndVertical();
            }
            // End the ScrollView
            EditorGUILayout.EndScrollView();

            EditorGUILayout.EndVertical();
        }

        // Delete item and confirm
        if (selectedArmor != null)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(selectedArmor.itemName, EditorStyles.boldLabel);
            GUILayout.FlexibleSpace();
        if (GUILayout.Button("Delete"))
            {
                bool Delete = EditorUtility.DisplayDialog($"Delete {selectedArmor.itemName}?", $"Are you want to delete {selectedArmor.itemName}?", "Yes", "No");
                if (Delete)
                {
                    DeleteArmor(selectedArmor);
                    selectedArmor = null;
                }
            }
            EditorGUILayout.EndHorizontal();
        }

        DrawItemList();

        DrawPropertiesSection();
    }

    private void DeleteArmor(Armor armor)
    {
        if (armor != null)
        {
            AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(armor));
            AssetDatabase.Refresh();
        }
    }

    protected override void DrawItemList()
    {
        EditorGUILayout.LabelField("Armor Properties", EditorStyles.boldLabel);

        if (selectedArmor != null)
        {
            EditorGUILayout.LabelField("Name: " + selectedArmor.itemName);
            EditorGUILayout.LabelField("Defense Power: " + selectedArmor.defensePower);
            EditorGUILayout.LabelField("Resistance: " + selectedArmor.resistance);
            EditorGUILayout.LabelField("Weight: " + selectedArmor.weight);
            EditorGUILayout.LabelField("Movement Speed Modifier: " + selectedArmor.movementSpeedModifier);
            EditorGUILayout.LabelField("Armor Type: " + selectedArmor.armorType);
        }
        else
        {
            EditorGUILayout.LabelField("No armor selected");
        }
    }

    // Handles the display and modification of the armor properties
    protected override void DrawPropertiesSection()
    {
        EditorGUILayout.LabelField("Armor Properties", EditorStyles.boldLabel);

        if (selectedArmor != null)
        {
            // Name
            string newName = EditorGUILayout.TextField("Name", selectedArmor.itemName, GUILayout.Width(200f));
            if (newName != selectedArmor.itemName && !IsValidItemName(newName))
            {
                EditorGUILayout.HelpBox("Item name cannot contain special characters (except spaces, dashes, and single quotes).", MessageType.Error);
            }
            else
            {
                selectedArmor.itemName = newName;
            }

            // Defense Power
            float newDefensePower = EditorGUILayout.FloatField("Defense Power", selectedArmor.defensePower, GUILayout.Width(200f));
            if (newDefensePower < 0f)
            {
                EditorGUILayout.HelpBox("Defense power must be a non-negative value.", MessageType.Error);
            }
            else
            {
                selectedArmor.defensePower = newDefensePower;
            }

            // Resistance
            float newResistance = EditorGUILayout.FloatField("Resistance", selectedArmor.resistance, GUILayout.Width(200f));
            if (newResistance < 0f)
            {
                EditorGUILayout.HelpBox("Resistance must be a non-negative value.", MessageType.Error);
            }
            else
            {
                selectedArmor.resistance = newResistance;
            }

            // Weight
            float newWeight = EditorGUILayout.FloatField("Weight", selectedArmor.weight, GUILayout.Width(200f));
            if (newWeight < 0f)
            {
                EditorGUILayout.HelpBox("Weight must be a non-negative value.", MessageType.Error);
            }
            else
            {
                selectedArmor.weight = newWeight;
            }

            // Movement Speed Modifier
            float newMovementSpeedModifier = EditorGUILayout.FloatField("Movement Speed Modifier", selectedArmor.movementSpeedModifier, GUILayout.Width(200f));
            if (newMovementSpeedModifier < 0f)
            {
                EditorGUILayout.HelpBox("Movement speed modifier must be a non-negative value.", MessageType.Error);
            }
            else
            {
                selectedArmor.movementSpeedModifier = newMovementSpeedModifier;
            }

            // Armor Type
            selectedArmor.armorType = (ArmorType)EditorGUILayout.EnumPopup("Armor Type", selectedArmor.armorType, GUILayout.Width(200f));
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