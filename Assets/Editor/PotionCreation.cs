using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.IO;

public class PotionCreation : BaseItemCreation<Potion>
{
    public string newPotionName = "";
    public PotionEffect potionEffect;
    public PotionEffect newPotionEffect;
    private float newEffectPower = 0f;
    private float newDuration = 0f;
    private float newCooldown = 0f;
    private bool newIsStackable = false;
    public float effectPower;
    public float duration;
    public float cooldown;
    public bool isStackable;
    private Potion selectedPotion = null;
    private PotionDatabase potionDatabase;
    private const string POTION_ASSET_PATH = "Assets/Items/Potions/";


    [MenuItem("Tools/Potion Creation")]
    public static void ShowWindow()
    {
        GetWindow<PotionCreation>("Potion Creation");
    }

    void OnGUI()
    {
        DrawPotionProperties();

        if (GUILayout.Button("Create Potion"))
        {
           CreateItem<Potion>();
        }
    }

    // Unused, will comeback to work of functionality
    private void CreatePotionButton()
    {
        if (!ValidatePotion())
        {
            // Do not create the potion
            return;
        }
        else
        {
            // Create the potion
            CreateItem<Potion>();
        }
    }

    private void DrawPotionProperties()
    {
        DrawItemProperties();

        EditorGUILayout.LabelField("Potion Properties", EditorStyles.boldLabel);

        if (selectedPotion != null)
        {
            potionEffect = (PotionEffect)EditorGUILayout.EnumPopup("Potion Effect", selectedPotion.potionEffect);
            effectPower = EditorGUILayout.FloatField("Effect Power", selectedPotion.effectPower);
            duration = EditorGUILayout.FloatField("Duration", selectedPotion.duration);
            cooldown = EditorGUILayout.FloatField("Cooldown", selectedPotion.cooldown);
            isStackable = EditorGUILayout.Toggle("Is Stackable", selectedPotion.isStackable);
        }
        else
        {
            potionEffect = (PotionEffect)EditorGUILayout.EnumPopup("Potion Effect", potionEffect);
            effectPower = EditorGUILayout.FloatField("Effect Power", effectPower);
            duration = EditorGUILayout.FloatField("Duration", duration);
            cooldown = EditorGUILayout.FloatField("Cooldown", cooldown);
            isStackable = EditorGUILayout.Toggle("Is Stackable", isStackable);
    
        }
    }

    private bool ValidatePotion()
    {
        ValidateItem();

        if (string.IsNullOrEmpty(newPotionName))
        {
            EditorUtility.DisplayDialog("Invalid Potion Name", "Please enter a name for the new potion.", "OK");
            return false;
        }

        if (AssetDatabase.FindAssets($"t:Potion {newPotionName}", new[] { POTION_ASSET_PATH }).Length > 0)
        {
            EditorUtility.DisplayDialog("Duplicate Potion Name", $"A potion with the name '{newPotionName}' already exists.", "OK");
            return false;
        }

        if (newEffectPower <= 0f)
        {
            EditorUtility.DisplayDialog("Invalid Effect Power", "Effect power must be greater than 0.", "OK");
            return false;
        }

        if (newDuration <= 0f)
        {
            EditorUtility.DisplayDialog("Invalid Duration", "Duration must be greater than 0.", "OK");
            return false;
        }

        if (newCooldown <= 0f)
        {
            EditorUtility.DisplayDialog("Invalid Cooldown", "Cooldown must be greater than 0.", "OK");
            return false;
        }

        if (newPotionEffect == PotionEffect.None)
        {
            EditorUtility.DisplayDialog("Invalid Potion Effect", "Potion effect must be set.", "OK");
            return false;
        }

        return true;
    }

    private void CreatePotion()
    {
        Potion NewPotion = CreateInstance<Potion>();
        NewPotion.potionEffect = potionEffect;
        NewPotion.effectPower = effectPower;
        NewPotion.duration = duration;
        NewPotion.cooldown = cooldown;
        NewPotion.isStackable = isStackable;
    
        // Add folder if not already present
        if (!AssetDatabase.IsValidFolder(POTION_ASSET_PATH))
            {
                System.IO.Directory.CreateDirectory(Application.dataPath + POTION_ASSET_PATH.Substring("Assets".Length));
                AssetDatabase.Refresh();
            }
        if (!File.Exists(itemName))
        {
            string saveFile = POTION_ASSET_PATH + itemName + ".asset";
            AssetDatabase.CreateAsset(NewPotion, saveFile);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }  
        else
        {
            Debug.Log("Item already exists");       
        }
        ValidatePotion();
    }
}