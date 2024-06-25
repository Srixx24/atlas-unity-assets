using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System.IO;

public class PotionDatabase : ItemDatabase<Potion>
{
    private List<Potion> potionItems;
    private Potion selectedPotion = null;
    private Vector2 scrollPos;
    private const string POTION_ASSET_PATH = "Assets/Items/Potions/";


    [MenuItem("Window/Item Manager/Potion Database")]
    public static void ShowWindow()
    {
        GetWindow<PotionDatabase>("Potion Database");
    }

    [Header("Potion Database")]
    public bool showPotionCreationButton = true;

    private void OnGUI()
    {
        if (showPotionCreationButton)
        {
            if (GUILayout.Button("Create New Potion"))
            {
                PotionCreation.ShowWindow();
            }
        }

        Potion[] potions = AssetDatabase.FindAssets("t:Potion", new[] { POTION_ASSET_PATH })
                        .Select(AssetDatabase.GUIDToAssetPath)
                        .Select(AssetDatabase.LoadAssetAtPath<Potion>)
                        .ToArray();

        // Display the list of potions
        if (potions.Length > 0)
        {
            EditorGUILayout.BeginVertical();
            EditorGUILayout.LabelField("Potion List", EditorStyles.boldLabel);

            // Create a ScrollView to make the list scrollable
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos, false, true, GUILayout.ExpandHeight(true));

            for (int x = 0; x < potions.Length; x++)
            {
                bool isSelected = selectedPotion != null && selectedPotion.itemName == potions[x].itemName;
                EditorGUILayout.BeginVertical();
                if (GUILayout.Button(new GUIContent(potions[x].itemName, potions[x].icon != null ? potions[x].icon.texture : null), isSelected ? EditorStyles.boldLabel : EditorStyles.label, GUILayout.Width(100)))
                {
                    if (isSelected)
                    {
                        selectedPotion = null;
                    }
                    else
                    {
                        selectedPotion = potions[x];
                    }
                }

                EditorGUILayout.EndVertical();
            }
            // End the ScrollView
            EditorGUILayout.EndScrollView();

            EditorGUILayout.EndVertical();
        }

        // Delete item and confirm
        if (selectedPotion != null)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(selectedPotion.itemName, EditorStyles.boldLabel);
            GUILayout.FlexibleSpace();
        if (GUILayout.Button("Delete"))
            {
                bool Delete = EditorUtility.DisplayDialog($"Delete {selectedPotion.itemName}?", $"Are you want to delete {selectedPotion.itemName}?", "Yes", "No");
                if (Delete)
                {
                    DeletePotion(selectedPotion);
                    selectedPotion = null;
                }
            }
            EditorGUILayout.EndHorizontal();
        }

        DrawItemList();

        DrawPropertiesSection();
    }

    private void DeletePotion(Potion potion)
    {
        if (potion != null)
        {
            AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(potion));
            AssetDatabase.Refresh();
        }
    }

    protected override void DrawItemList()
    {
        EditorGUILayout.LabelField("Potion Properties", EditorStyles.boldLabel);

        if (selectedPotion != null)
        {
            EditorGUILayout.LabelField("Name: " + selectedPotion.itemName);
            EditorGUILayout.LabelField("Potion Effect: " + selectedPotion.potionEffect);
            EditorGUILayout.LabelField("Effect Power: " + selectedPotion.effectPower);
            EditorGUILayout.LabelField("Duration: " + selectedPotion.duration);
            EditorGUILayout.LabelField("Cooldown: " + selectedPotion.cooldown);
            EditorGUILayout.LabelField("Stackable: " + selectedPotion.isStackable);
        }
        else
        {
            EditorGUILayout.LabelField("No potion selected");
        }
    }

    // Handles the display and modification of the potion properties
    protected override void DrawPropertiesSection()
    {
        EditorGUILayout.LabelField("Potion Properties", EditorStyles.boldLabel);

        if (selectedPotion != null)
        {
            // Name
            string newName = EditorGUILayout.TextField("Name", selectedPotion.itemName, GUILayout.Width(200f));
            if (newName != selectedPotion.itemName && !IsValidItemName(newName))
            {
                EditorGUILayout.HelpBox("Item name cannot contain special characters (except spaces, dashes, and single quotes).", MessageType.Error);
            }
            else
            {
                selectedPotion.itemName = newName;
            }

            // Potion Effect
            selectedPotion.potionEffect = (PotionEffect)EditorGUILayout.EnumPopup("Potion Effect", selectedPotion.potionEffect, GUILayout.Width(200f));

            // Effect Power
            float newEffectPower = EditorGUILayout.FloatField("Effect Power", selectedPotion.effectPower, GUILayout.Width(200f));
            if (newEffectPower < 0f)
            {
                EditorGUILayout.HelpBox("Effect power must be a non-negative value.", MessageType.Error);
            }
            else
            {
                selectedPotion.effectPower = newEffectPower;
            }

            // Duration
            float newDuration = EditorGUILayout.FloatField("Duration", selectedPotion.duration, GUILayout.Width(200f));
            if (newDuration < 0f)
            {
                EditorGUILayout.HelpBox("Duration must be a non-negative value.", MessageType.Error);
            }
            else
            {
                selectedPotion.duration = newDuration;
            }

            // Cooldown
            float newCooldown = EditorGUILayout.FloatField("Cooldown", selectedPotion.cooldown, GUILayout.Width(200f));
            if (newCooldown < 0f)
            {
                EditorGUILayout.HelpBox("Cooldown must be a non-negative value.", MessageType.Error);
            }
            else
            {
                selectedPotion.cooldown = newCooldown;
            }

            // Stackable
            selectedPotion.isStackable = EditorGUILayout.Toggle("Stackable", selectedPotion.isStackable, GUILayout.Width(200f));
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