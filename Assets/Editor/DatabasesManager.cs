using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;
using System.IO;

public class DatabasesManager : EditorWindow
{
    private ArmorDatabase armorDatabase;
    private PotionDatabase potionDatabase;
    private WeaponDatabase weaponDatabase;
    private Statistics statistics;
    private Vector2 scrollPos;
    private int totalItemCount = 0;
    private bool isInitialized = false;
    private GUIStyle smallHeader = new GUIStyle(EditorStyles.largeLabel);
    private GUIStyle helpBoxStyle = new GUIStyle(EditorStyles.largeLabel);

    
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

        isInitialized = true;
    }
    private void OnEnable()
    {
        smallHeader.fontStyle = FontStyle.Bold;
        smallHeader.fontSize = 15;
        smallHeader.normal.textColor = new Color32(180, 180, 180, 255); // Light grey color
        smallHeader.alignment = TextAnchor.MiddleCenter; // Center the text

        // Initialize the helpBoxStyle
        helpBoxStyle = new GUIStyle();
        helpBoxStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/node1.png") as Texture2D;
        helpBoxStyle.normal.textColor = new Color(0.8f, 0.8f, 0.8f);
        helpBoxStyle.fontSize = 20;
        helpBoxStyle.fontStyle = FontStyle.Bold;
        helpBoxStyle.padding = new RectOffset(200, 200, 30, 30);
        helpBoxStyle.alignment = TextAnchor.MiddleCenter;
    }

    private void OnGUI()
    {
        // Title
        EditorGUILayout.LabelField("Item Database Manager", EditorStyles.boldLabel);

        // Apply the custom helpBoxStyle to the help box
        EditorGUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        EditorGUILayout.BeginVertical();
        GUILayout.Label("Welcome to the Item Database Manager!\nHere, you can access and manage different databases\n related to weapons, potions, armors, and other items.", helpBoxStyle);
        EditorGUILayout.EndVertical();
        GUILayout.FlexibleSpace();
        EditorGUILayout.EndHorizontal();

        // Buttons to access other windows
        GUILayout.Label("Please select a database to view", smallHeader);
        EditorGUILayout.Space();
        EditorGUILayout.BeginHorizontal();
        OpenPotionDatabase();
        OpenWeaponDatabase();
        OpenArmorDatabase();
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space();
        

        EditorGUILayout.BeginHorizontal();
        // Start the ScrollView
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos, false, true);
        
        // Initialize the statistics object if it hasn't been initialized yet
        if (statistics == null)
        {
            statistics = new Statistics();
            statistics.DisplayGeneralStatistics();
            statistics.DisplayWeaponStatistics();
            statistics.DisplayArmorStatistics();
            statistics.DisplayPotionStatistics();
        }

        // Display statistics
        statistics.DisplayGeneralStatistics();
        EditorGUILayout.Space();
        statistics.DisplayWeaponStatistics();
        EditorGUILayout.Space();
        statistics.DisplayArmorStatistics();
        EditorGUILayout.Space();
        statistics.DisplayPotionStatistics();


        // End the ScrollView
        EditorGUILayout.EndScrollView();
        EditorGUILayout.EndHorizontal();
    }

    // Buttons to access other windows
    void OpenPotionDatabase()
    {
        // Icon texture
        Texture2D icon = EditorGUIUtility.FindTexture("Favorite");

        GUIStyle buttonStyle = new GUIStyle(GUI.skin.button);
        buttonStyle.fontSize = 18;
        buttonStyle.fontStyle = FontStyle.Bold;
        buttonStyle.padding = new RectOffset(8, 8, 4, 4);
        buttonStyle.normal.textColor = new Color32(100, 0, 128, 255); //purple

        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        if (GUILayout.Button("Potions Database", buttonStyle, GUILayout.Width(200), GUILayout.Height(40)))
        {
            if (potionDatabase == null)
            {
                potionDatabase = new PotionDatabase();
            }
            PotionDatabase.ShowWindow();
        }
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
    }

    void OpenWeaponDatabase()
    {
        // Icon texture
        Texture2D icon = EditorGUIUtility.FindTexture("Favorite");

        GUIStyle buttonStyle = new GUIStyle(GUI.skin.button);
        buttonStyle.fontSize = 18;
        buttonStyle.fontStyle = FontStyle.Bold;
        buttonStyle.padding = new RectOffset(8, 8, 4, 4);
        buttonStyle.normal.textColor = new Color32(100, 0, 128, 255); //purple

        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        if (GUILayout.Button("Weapon Database", buttonStyle, GUILayout.Width(200), GUILayout.Height(40)))
        {
            if (weaponDatabase == null)
            {
                weaponDatabase = new WeaponDatabase();
            }
            WeaponDatabase.ShowWindow();
        }
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
    }

    void OpenArmorDatabase()
    {
        // Icon texture
        Texture2D icon = EditorGUIUtility.FindTexture("Favorite");

        GUIStyle buttonStyle = new GUIStyle(GUI.skin.button);
        buttonStyle.fontSize = 18;
        buttonStyle.fontStyle = FontStyle.Bold;
        buttonStyle.padding = new RectOffset(8, 8, 4, 4);
        buttonStyle.normal.textColor = new Color32(100, 0, 128, 255); //purple

        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        if (GUILayout.Button("Armor Database", buttonStyle, GUILayout.Width(200), GUILayout.Height(40)))
        {
            if (armorDatabase == null)
            {
                armorDatabase = new ArmorDatabase();
            }
            ArmorDatabase.ShowWindow();
        }
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
    }

    public void SetGraphicsSettings(int resolution, int antiAliasing, int textureQuality, int shadowQuality)
{
    // Validate the anti-aliasing value
    if (antiAliasing < 1)
    {
        antiAliasing = 1;
    }

    // Set the graphics settings
    Screen.SetResolution(resolution, resolution, true);
    QualitySettings.antiAliasing = antiAliasing;
    QualitySettings.masterTextureLimit = (int)(1 - textureQuality / 100f);
    QualitySettings.shadowResolution = (ShadowResolution)shadowQuality;
}

    // Going to come back to work on graph
    //private void DisplayGraphStatistics(Rect barGraphRect)
    //{
        // Use the inherited statistical methods
        //float maxValue = Mathf.Max(values);
        //float barWidth = rect.width / values.Length;

        // Create a Rect object for the bar graph
        //Rect barGraphRect = new Rect(20, 20, 800, 400);

        // Call the DrawBarGraph method and pass the Rect object
        //statistics.DrawBarGraph(barGraphRect);
    //} 
} 