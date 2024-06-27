using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.IO;

public class WeaponCreation : BaseItemCreation<Weapon>
{
    private string newWeaponName = "";
    private int newWeaponDamage = 0;
    private float newWeaponRange = 0f;

    public WeaponType weaponType;
    public float attackPower;
    public float attackSpeed;
    public float durability { get; set; }
    public float range;
    public float criticalHitChance;
    private Weapon selectedWeapon = null;
    private WeaponDatabase weaponDatabase;

    private const string WEAPON_ASSET_PATH = "Assets/Items/Weapons/";
    

    [MenuItem("Tools/Weapon Creation")]
    public static void ShowWindow()
    {
        GetWindow<WeaponCreation>("Weapon Creation");
    }

    void OnGUI()
    {
        DrawWeaponProperties();

        if (GUILayout.Button("Create Weapon"))
        {
            CreateItem<Weapon>();
        }
    }

    // Unused, will comeback to work of functionality
    private void CreateWeaponButton()
    {
        if (!ValidateWeapon())
        {
            // Do not create the Weapon
            return;
        }
        else
        {
            // Create the Weapon
            CreateItem<Weapon>();
        }
    }

    private void DrawWeaponProperties()
    {
        DrawItemProperties();

        EditorGUILayout.LabelField("Weapon Properties", EditorStyles.boldLabel);

        if (selectedWeapon != null)
        {
            attackPower = EditorGUILayout.FloatField("AttackPower", selectedWeapon.attackPower);
            attackSpeed = EditorGUILayout.FloatField("AttackSpeed", selectedWeapon.attackSpeed);
            durability = EditorGUILayout.FloatField("Durability", selectedWeapon.durability);
            range = EditorGUILayout.FloatField("Range", selectedWeapon.range);
            criticalHitChance = EditorGUILayout.FloatField("CriticalHitChance", selectedWeapon.criticalHitChance);
            weaponType = selectedWeapon.weaponType;
        }
        else
        {
            attackPower = EditorGUILayout.FloatField("AttackPower", attackPower);
            attackSpeed = EditorGUILayout.FloatField("AttackSpeed", attackSpeed);
            durability = EditorGUILayout.FloatField("Durability", durability);
            range = EditorGUILayout.FloatField("Range", range);
            criticalHitChance = EditorGUILayout.FloatField("CriticalHitChance", criticalHitChance);
            weaponType = (WeaponType)EditorGUILayout.EnumPopup("Weapon Type", weaponType);
        }
    }

    private bool ValidateWeapon()
    {
        ValidateItem();
        
        if (string.IsNullOrEmpty(newWeaponName))
        {
            EditorUtility.DisplayDialog("Invalid Weapon Name", "Please enter a name for the new weapon.", "OK");
            return false;
        }

        if (AssetDatabase.FindAssets($"t:Weapon {newWeaponName}", new[] { WEAPON_ASSET_PATH }).Length > 0)
        {
            EditorUtility.DisplayDialog("Duplicate Weapon Name", $"A weapon with the name '{newWeaponName}' already exists.", "OK");
            return false;
        }

        if (newWeaponDamage <= 0)
        {
            EditorUtility.DisplayDialog("Invalid Damage Value", "Weapon damage must be greater than 0.", "OK");
            return false;
        }

        if (newWeaponRange <= 0f)
        {
            EditorUtility.DisplayDialog("Invalid Range Value", "Weapon range must be greater than 0.", "OK");
            return false;
        }

        return true;
    }

    private void CreateWeapons()
    {
        Weapon NewWeapon = CreateInstance<Weapon>();
        NewWeapon.weaponType = weaponType;
        NewWeapon.attackPower = attackPower;
        NewWeapon.attackSpeed = attackSpeed;
        NewWeapon.durability = durability;
        NewWeapon.range = range;
        NewWeapon.criticalHitChance = criticalHitChance;

        // Add folder if not already present
        if (!AssetDatabase.IsValidFolder(WEAPON_ASSET_PATH))
            {
                System.IO.Directory.CreateDirectory(Application.dataPath + WEAPON_ASSET_PATH.Substring("Assets".Length));
                AssetDatabase.Refresh();
            }
        if (!File.Exists(itemName))
        {
            string saveFile = WEAPON_ASSET_PATH + itemName + ".asset";
            AssetDatabase.CreateAsset(NewWeapon, saveFile);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }  
        else
        {
            Debug.Log("Item already exists");       
        }
    }
}