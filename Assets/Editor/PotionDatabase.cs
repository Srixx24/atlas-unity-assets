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
    GUIStyle itemNameText = new GUIStyle(EditorStyles.largeLabel);
    GUIStyle miniTitle;


    private void OnEnable()
    {
        // Initialize the large, bold, green style
        itemNameText = new GUIStyle(EditorStyles.largeLabel);
        itemNameText.fontStyle = FontStyle.Bold;
        itemNameText.fontSize = 16;
        itemNameText.normal.textColor = Color.green;

        // Initialize the blue style
        miniTitle = new GUIStyle(EditorStyles.boldLabel);
        miniTitle.normal.textColor = new Color32(0, 255, 255, 255);
    }

    private void OnGUI()
    {
        GetItemCount();
        EditorGUILayout.Space();

        DrawTopLeftOptions();
        EditorGUILayout.Space();

        if (showPotionCreationButton)
        {
            GUIStyle buttonStyle = new GUIStyle(GUI.skin.button);
            buttonStyle.fontSize = 15;
            buttonStyle.fontStyle = FontStyle.Bold;
            buttonStyle.padding = new RectOffset(20, 20, 4, 4);
            buttonStyle.normal.textColor = Color.green;

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Create New Potion", buttonStyle, GUILayout.Width(200), GUILayout.Height(40)))
            {
                PotionCreation.ShowWindow();
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }

        Potion[] potions = AssetDatabase.FindAssets("t:Potion", new[] { POTION_ASSET_PATH })
                        .Select(AssetDatabase.GUIDToAssetPath)
                        .Select(AssetDatabase.LoadAssetAtPath<Potion>)
                        .ToArray();

        // Display the list of potions
        if (potions.Length > 0)
        {
            EditorGUILayout.BeginVertical();
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Potion List", itemNameText);
            EditorGUILayout.Space();
            EditorGUILayout.Space();


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

                EditorGUILayout.Space(2);
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
            EditorGUILayout.LabelField(selectedPotion.itemName, itemNameText);
            GUIStyle deleteButtonText = new GUIStyle(EditorStyles.boldLabel);
            deleteButtonText.normal.textColor = Color.red;
            GUILayout.FlexibleSpace();
        if (GUILayout.Button("Delete", deleteButtonText))
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
        EditorGUILayout.LabelField("Potion Properties", miniTitle);

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
        if (selectedPotion != null)
        {
            GUILayout.Label("Slide to change values or enter your own", miniTitle);

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

    public void GetItemCount()
    {
        GUILayout.Label($"Total Potions: {AssetDatabase.FindAssets("t:Potion").Length}");
    }

    protected override void ExportItemsToCSV()
    {
       
    }

    protected override void ImportItemsFromCSV()
    {
        
    }
}