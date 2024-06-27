using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System.IO;

public class WeaponDatabase : ItemDatabase<Weapon>
{
    //private List<Weapon> weaponItems;
    private List<Weapon> weapons = new List<Weapon>();
    private Weapon selectedWeapon = null;
    private Vector2 scrollPos;
    //private List<Weapon> filteredWeapons;
    //private string searchText = "";
    public WeaponType weaponType;
    //private List<bool> filterStates = new List<bool>();
    GUIStyle itemNameText = new GUIStyle(EditorStyles.largeLabel);
    GUIStyle miniTitle;
    private const string WEAPON_ASSET_PATH = "Assets/Items/Weapons/";


    [MenuItem("Window/Item Manager/Weapon Database")]
    public static void ShowWindow()
    {
        GetWindow<WeaponDatabase>("Weapon Database");
    }

    [Header("Weapon Database")]
    public bool showWeaponCreationButton = true;

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
        // Display the search bar
        //EditorGUILayout.BeginHorizontal();
        //searchText = EditorGUILayout.TextField(searchText, EditorStyles.toolbarSearchField);
        //if (GUILayout.Button("", EditorStyles.toolbarButton))
        //{
            // Clear the search text
        //    searchText = "";
        //}
        //EditorGUILayout.EndHorizontal();
        //DisplayFiltered();
        //EditorGUILayout.Space();

        //EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
        //for (int x = 0; x < filteredWeapons.Count; x++)
        //{
        //    filterStates[x] = EditorGUILayout.Toggle(filterStates[x], EditorStyles.toolbarButton, GUILayout.Width(100));
        //    EditorGUILayout.LabelField(filteredWeapons[x].Name, EditorStyles.toolbarButton);
        //}
        //GUILayout.FlexibleSpace();
        //EditorGUILayout.EndHorizontal();
        

        GetItemCount();
        EditorGUILayout.Space();

        DrawTopLeftOptions();
        EditorGUILayout.Space();

        if (showWeaponCreationButton)
        {
            GUIStyle buttonStyle = new GUIStyle(GUI.skin.button);
            buttonStyle.fontSize = 15;
            buttonStyle.fontStyle = FontStyle.Bold;
            buttonStyle.padding = new RectOffset(20, 20, 4, 4);
            buttonStyle.normal.textColor = Color.green;

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Create New Weapon", buttonStyle, GUILayout.Width(200), GUILayout.Height(40)))
            {
                WeaponCreation.ShowWindow();
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }

        Weapon[] weapons = AssetDatabase.FindAssets("t:Weapon", new[] { WEAPON_ASSET_PATH })
                          .Select(AssetDatabase.GUIDToAssetPath)
                          .Select(AssetDatabase.LoadAssetAtPath<Weapon>)
                          .ToArray();
        
        // Display the list of weapons
        if (weapons.Length > 0)
        {
            EditorGUILayout.BeginVertical();
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Weapon List", itemNameText);
            EditorGUILayout.Space();
            EditorGUILayout.Space();


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

        DeleteWeapon(selectedWeapon);

        DrawItemList();

        DrawPropertiesSection();
    }

    private void DeleteWeapon(Weapon weapon)
    {
        // Delete item and confirm
        if (selectedWeapon != null)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(selectedWeapon.itemName, itemNameText);
            GUIStyle deleteButtonText = new GUIStyle(EditorStyles.boldLabel);
            deleteButtonText.normal.textColor = Color.red;
            GUILayout.FlexibleSpace();

            if (GUILayout.Button("Delete", deleteButtonText))
            {
                bool Delete = EditorUtility.DisplayDialog($"Delete {selectedWeapon.itemName}?", $"Are you want to delete {selectedWeapon.itemName}?", "Yes", "No");
                if (Delete)
                {
                    if (weapon != null)
                    {
                        AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(weapon));
                        AssetDatabase.Refresh();
                    }
                    selectedWeapon = null;
                }
            }
            EditorGUILayout.EndHorizontal();
        }
    }

    protected override void DrawItemList()
    {
        EditorGUILayout.LabelField("Weapon Properties", miniTitle);

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
        if (selectedWeapon != null)
        {
            GUILayout.Label("Slide to change values or enter your own", miniTitle);
            
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

    // Weapon total count
    public void GetItemCount()
    {
        GUILayout.Label($"Total Weapons: {AssetDatabase.FindAssets("t:Weapon").Length}");
    }


    protected override void ExportItemsToCSV()
    {
        
    }

    protected override void ImportItemsFromCSV()
    {

    }

    //private List<Weapon> FilterWeapons(List<Weapon> weapons, string searchText, List<bool> filterStates)
    //{
        // Filter the weapons based on search text
    //var filteredWeapons = weapons.Where(w => w.name.ToLower().Contains(searchText.ToLower())).ToList();

    // Apply the filter based on the filterStates list
    //filteredWeapons = filteredWeapons.Where((w, i) => filterStates[i]).ToList();

    //return filteredWeapons;
    //}

    // Display the filtered weapons
    //public void DisplayFiltered()
    //{
        // Filter the weapons based on search and filters
    //    filteredWeapons = FilterWeapons(weapon, searchText, filterStates);
    //    if (weaponItems == null)
    //    {
    //        Debug.LogError("The 'weaponItems' list is null.");
     //       return;
       // }

        //if (filterStates == null)
        //{
        //    Debug.LogError("The 'filterStates' list is null.");
        //    return;
        //}

        //if (filteredWeapons.Count > 0)
        //{
         //   foreach (var weapon in filteredWeapons)
           // {
             //   bool isSelected = selectedWeapon != null && selectedWeapon.itemName == weapon.itemName;
               // EditorGUILayout.BeginHorizontal(isSelected ? EditorStyles.helpBox : EditorStyles.label, GUILayout.Height(64));
                //if (GUILayout.Button(new GUIContent(weapon.itemName, weapon.icon != null ? weapon.icon.texture : null), isSelected ? EditorStyles.boldLabel : EditorStyles.label, GUILayout.Width(100), GUILayout.Height(64)))
                //{
                  //  if (isSelected)
                   // {
                    //    selectedWeapon = null;
                    //}
                    //else
                    //{
                      //  selectedWeapon = weapon;
                    //}
                //}
                //EditorGUILayout.ObjectField(weapon, typeof(Weapon), false, GUILayout.ExpandWidth(true));
                //EditorGUILayout.EndHorizontal();
            //}
        //}
        //else
        //{
         //       EditorGUILayout.LabelField("No weapons found matching the search criteria.");
        //}
    //}
}