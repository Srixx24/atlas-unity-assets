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

        if (showArmorCreationButton)
        {
           GUIStyle buttonStyle = new GUIStyle(GUI.skin.button);
            buttonStyle.fontSize = 15;
            buttonStyle.fontStyle = FontStyle.Bold;
            buttonStyle.padding = new RectOffset(20, 20, 4, 4);
            buttonStyle.normal.textColor = Color.green;

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Create New Armor", buttonStyle, GUILayout.Width(200), GUILayout.Height(40)))
            {
                ArmorCreation.ShowWindow();;
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }

        Armor[] armors = AssetDatabase.FindAssets("t:Armor", new[] { ARMOR_ASSET_PATH })
                        .Select(AssetDatabase.GUIDToAssetPath)
                        .Select(AssetDatabase.LoadAssetAtPath<Armor>)
                        .ToArray();
    
        // Display the list of armors
        if (armors.Length > 0)
        {
            EditorGUILayout.BeginVertical();
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Armor List", itemNameText);
            EditorGUILayout.Space();
            EditorGUILayout.Space();

            // Create a ScrollView to make the list scrollable
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos, false, true, GUILayout.ExpandHeight(true));

            for (int x = 0; x < armors.Length; x++)
            {
                bool isSelected = selectedArmor != null && selectedArmor.itemName == armors[x].itemName;
                EditorGUILayout.BeginVertical();
                if (GUILayout.Button(new GUIContent(armors[x].itemName, armors[x].icon != null ? armors[x].icon.texture : null), isSelected ? EditorStyles.boldLabel : EditorStyles.label, GUILayout.Width(75)))
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

                EditorGUILayout.Space(2);
                EditorGUILayout.EndVertical();
            }
            // End the ScrollView
            EditorGUILayout.EndScrollView();

            EditorGUILayout.EndVertical();
        }
        DeleteArmor(selectedArmor);

        DrawItemList();

        DrawPropertiesSection();
    }

    private void DeleteArmor(Armor armor)
    {
        // Delete item and confirm
        if (selectedArmor != null)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(selectedArmor.itemName, itemNameText);
            GUIStyle deleteButtonText = new GUIStyle(EditorStyles.boldLabel);
            deleteButtonText.normal.textColor = Color.red;
            GUILayout.FlexibleSpace();

            if (GUILayout.Button("Delete", deleteButtonText))
            {
                bool Delete = EditorUtility.DisplayDialog($"Delete {selectedArmor.itemName}?", $"Are you want to delete {selectedArmor.itemName}?", "Yes", "No");
                if (Delete)
                {
                    if (armor != null)
                    {
                        AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(armor));
                        AssetDatabase.Refresh();
                    }
                    selectedArmor = null;
                }
            }
            EditorGUILayout.EndHorizontal();
        }
    }

    protected override void DrawItemList()
    {
        EditorGUILayout.LabelField("Armor Properties", miniTitle);

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
        if (selectedArmor != null)
        {
            GUILayout.Label("Slide to change values or enter your own", miniTitle);

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

    public void GetItemCount()
    {
        GUILayout.Label($"Total Armors: {AssetDatabase.FindAssets("t:Armor").Length}");
    }

    protected override void ExportItemsToCSV()
    {
    
    }

    protected override void ImportItemsFromCSV()
    {

    }
}