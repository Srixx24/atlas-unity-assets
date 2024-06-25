using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.IO;

public class ArmorCreation : BaseItemCreation<Armor>
{
    private string newArmorName = "";
    private Armor selectedArmor = null;
    public ArmorType armorType;
    public float defensePower;
    public float resistance;
    public float weight;
    public float movementSpeedModifier;
    private ArmorDatabase armorDatabase;

    private const string ARMOR_ASSET_PATH = "Assets/Items/Armor/";


    [MenuItem("Tools/Armor Creation")]  
    public static void ShowWindow()
    {
        GetWindow<ArmorCreation>("Armor Creation");
    }

    void OnGUI()
    {
        DrawArmorProperties();
    
        if (GUILayout.Button("Create Armor"))
        {
            CreateItem<Armor>();
        
        }
    }

    private void DrawArmorProperties()
    {
        DrawItemProperties();

        EditorGUILayout.LabelField("Armor Properties", EditorStyles.boldLabel);

        if (selectedArmor != null)
        {
            armorType = selectedArmor.armorType;
            defensePower = EditorGUILayout.FloatField("Defense Power", selectedArmor.defensePower);
            resistance = EditorGUILayout.FloatField("Resistance", selectedArmor.resistance);
            weight = EditorGUILayout.FloatField("Weight", selectedArmor.weight);
            movementSpeedModifier = EditorGUILayout.FloatField("Movement Speed Modifier", selectedArmor.movementSpeedModifier);
        }
        else
        {
            armorType = (ArmorType)EditorGUILayout.EnumPopup("Armor Type", armorType);
            defensePower = EditorGUILayout.FloatField("Defense Power", defensePower);
            resistance = EditorGUILayout.FloatField("Resistance", resistance);
            weight = EditorGUILayout.FloatField("Weight", weight);
            movementSpeedModifier = EditorGUILayout.FloatField("Movement Speed Modifier", movementSpeedModifier);
        }
    }

    private bool ValidateArmor()
    {
        ValidateItem();

        if (string.IsNullOrEmpty(newArmorName))
        {
            EditorUtility.DisplayDialog("Invalid Armor Name", "Please enter a name for the new armor.", "OK");
            return false;
        }

        if (AssetDatabase.FindAssets($"t:Armor {newArmorName}", new[] { ARMOR_ASSET_PATH }).Length > 0)
        {
         EditorUtility.DisplayDialog("Duplicate Armor Name", $"An armor with the name '{newArmorName}' already exists.", "OK");
            return false;
        }
        return true;
    }

    private void CreateArmor()
    {
        Armor NewArmor = CreateInstance<Armor>();
        NewArmor.name = itemName;
        NewArmor.armorType = armorType;
        NewArmor.defensePower = defensePower;
        NewArmor.resistance = resistance;
        NewArmor.weight = weight;
        NewArmor.movementSpeedModifier = movementSpeedModifier;

        // Add folder if not already present
        if (!AssetDatabase.IsValidFolder(ARMOR_ASSET_PATH))
            {
                System.IO.Directory.CreateDirectory(Application.dataPath + ARMOR_ASSET_PATH.Substring("Assets".Length));
                AssetDatabase.Refresh();
            }
        if (!File.Exists(itemName))
        {
            string saveFile = ARMOR_ASSET_PATH + itemName + ".asset";
            AssetDatabase.CreateAsset(NewArmor, saveFile);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }  
        else
        {
            Debug.Log("Item already exists");
        }
    }
}