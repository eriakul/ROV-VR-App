﻿using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using Unity.XR.OpenVR.SimpleJSON;

[System.Serializable]
public class OverlayConfig
{
    public string overlayName;
    public bool isVisible;
    public float xPosition;
    public float yPosition;

    public void recordOverlayConfig(GameObject overlay)
    {
        overlayName = overlay.name;
        isVisible = overlay.activeInHierarchy;
        xPosition = overlay.transform.localPosition.x;
        yPosition = overlay.transform.localPosition.y;
    }
}

[System.Serializable]
public class User
{
    public List<OverlayConfig> overlaySettings = new List<OverlayConfig> { };
    public string userName;

    public User(string name)
    {
        userName = name;
    }

    public void RecordOverlaySettings(List<GameObject> overlays)
    {
        foreach (GameObject overlay in overlays)
        {
            OverlayConfig newOverlay = new OverlayConfig();
            newOverlay.recordOverlayConfig(overlay);
            overlaySettings.Add(newOverlay);
        }
    }
}

[System.Serializable]
public class RovVrSettings
{
    // TODO: save camera selection
    // Settings 
    public List<User> users = new List<User> { };
    public string PTGUIFilename;
    public string LCMURL;

    public float L_CX;
    public float L_CY;
    public float L_RX;
    public float L_RY;
    public float R_CX;
    public float R_CY;
    public float R_RX;
    public float R_RY;
    public float a;
    public float b;
    public float c;

    public bool stereoEnabled;

}

public class SaveData
{
    private RovVrSettings applicationSettings;
    string saveFilePath;

    public SaveData(RovVrSettings settings, string filepath)
    {
        applicationSettings = settings;
        saveFilePath = filepath;
        SaveIntoJson();
    }

    public void SaveIntoJson()
    {
        string serializedSettings = JsonUtility.ToJson(applicationSettings);
        System.IO.File.WriteAllText(saveFilePath, serializedSettings);
    }
}



public class SettingsManager : MonoBehaviour
{
    [Header("Settings UI")]
    public Canvas SettingsCanvas;
    public Button CloseButton;
    public Button SaveAllButton;

    [Header("User")]
    public Dropdown ChooseUserDropdown;
    public InputField NewUserInputField;
    public Button AddUserButton;

    [Header("LCM")]
    public InputField LCMInputField;
    public LCMListener LCMListenerObject;

    [Header("Camera Settings")]
    public InputField PTGUIFilepathInputField;
    public Material skyboxMaterial;
    public Toggle StereoToggle;

    [Header("Overlays")]
    public GameObject mainOverlayCanvas;

    RovVrSettings settings = new RovVrSettings();

    string saveFilePath;
    int currUserIdx = -1;
    bool prevEscBool = false;
    bool currEscBool = false;
    List<GameObject> overlayList;


    void Start()
    {
        CloseButton.onClick.AddListener(CloseCallback);
        AddUserButton.onClick.AddListener(AddUserCallback);
        SaveAllButton.onClick.AddListener(SaveAllCallback);

        ChooseUserDropdown.onValueChanged.AddListener(delegate
        { ChooseUserCallback(ChooseUserDropdown); });

        // Get all overlay gameobjects from mainOverlayCanvas
        Transform[] allChildren = mainOverlayCanvas.GetComponentsInChildren<Transform>(includeInactive: true);
        overlayList = new List<GameObject> { };

        foreach (Transform child in allChildren)
        {
            if (child.parent == mainOverlayCanvas.transform)
            {
                GameObject overlay = child.gameObject;
                overlayList.Add(overlay);
            }
        }

        StereoToggle.onValueChanged.AddListener(delegate
        { StereoCallback(StereoToggle); });

        // Load saved settings
        saveFilePath = Application.persistentDataPath + "/ROV-VR_Application_Settings.json";
        LoadSavedSettings();

        // Close settings on startup
        SettingsCanvas.enabled = false;
    }


    void Update()
    {
        currEscBool = Input.GetKey(KeyCode.Escape);

        // Press Escape to open/close window
        if (currEscBool == true && prevEscBool == false)
        {
            SettingsCanvas.enabled = !SettingsCanvas.enabled;
        }

        prevEscBool = currEscBool;


        //Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        ////Debug.Log(Input.mousePosition);

        //if (Input.GetMouseButtonDown(0))
        //    Debug.Log("Pressed primary button.");
    }

    void CloseCallback()
    {
        SettingsCanvas.enabled = false;
    }

    void AddUserCallback()
    {
        // TODO: Prevent duplicate names, ensure name is not empty
        // Create new user
        User newUser = new User(NewUserInputField.text);
        settings.users.Add(newUser);

        // Add new user name to drop down, select new user
        ChooseUserDropdown.AddOptions(new List<string> { newUser.userName });
        int userIdx = ChooseUserDropdown.options.FindIndex(ChooseUserDropdown => ChooseUserDropdown.text == newUser.userName);
        ChooseUserDropdown.value = userIdx;
        currUserIdx = userIdx;

        // Clear text when done
        NewUserInputField.text = "";
    }

    void SaveAllCallback()
    {
        // Save LCM URL
        settings.LCMURL = LCMInputField.text;
        LCMListenerObject.changeURL(settings.LCMURL);

        // Save PTGUI Filepath
        settings.PTGUIFilename = PTGUIFilepathInputField.text;

        // Save camera settings
        settings.stereoEnabled = StereoToggle.isOn;
        settings.L_CX = skyboxMaterial.GetFloat("_L_CX");
        settings.L_CY = skyboxMaterial.GetFloat("_L_CY");
        settings.L_RX = skyboxMaterial.GetFloat("_R_CX");
        settings.L_RY = skyboxMaterial.GetFloat("_R_CY");
        settings.R_CX = skyboxMaterial.GetFloat("_L_RX");
        settings.R_CY = skyboxMaterial.GetFloat("_L_RY");
        settings.R_RX = skyboxMaterial.GetFloat("_R_RX");
        settings.R_RY = skyboxMaterial.GetFloat("_R_RY");
        settings.a = skyboxMaterial.GetFloat("_a");
        settings.b = skyboxMaterial.GetFloat("_b");
        settings.c = skyboxMaterial.GetFloat("_c");

        // TODO: Implement file reading and extract ptgui settings
        //var fileContent = File.ReadAllBytes(settings.PTGUIFilename);
        //string jsonString = System.Text.Encoding.UTF8.GetString(fileContent);
        //JSONNode PTGUISettings = JSON.Parse(jsonString);
        //string test_a = PTGUISettings["contenttype"].Value;

        // Save User Settings
        if (currUserIdx == -1)
        {
            Debug.LogWarning("No user selected. Overlay settings will not be saved");
            return;
        }
        settings.users[currUserIdx].RecordOverlaySettings(overlayList);

        // Write data to file
        print("Attempting to save data");
        SaveData dataSaver = new SaveData(settings, saveFilePath);
    }

    void ChooseUserCallback(Dropdown ChooseUserDropdown)
    {
        int i = ChooseUserDropdown.value;
        currUserIdx = i;

        LoadUserSettings();
    }

    void LoadSavedSettings()
    {
        if (File.Exists(saveFilePath))
        {
            // Read the entire file and save its contents.
            string fileContents = File.ReadAllText(saveFilePath);

            // Deserialize the JSON data
            settings = JsonUtility.FromJson<RovVrSettings>(fileContents);

            // Update the UI
            // Create a list containing all user names
            List<string> userNameList = new List<string> { };
            foreach (User user in settings.users)
                userNameList.Add(user.userName);

            currUserIdx = 0; // TODO: add checking to make sure there are actually users

            // Update Dropdown Options
            ChooseUserDropdown.AddOptions(userNameList);
            ChooseUserDropdown.value = currUserIdx;

            // Update LCM InputField
            LCMInputField.text = settings.LCMURL;
            PTGUIFilepathInputField.text = settings.PTGUIFilename;

            // Update Stereo settings
            StereoToggle.isOn = settings.stereoEnabled;
            skyboxMaterial.SetFloat("_L_CX", settings.L_CX);
            skyboxMaterial.SetFloat("_L_CY", settings.L_CY);
            skyboxMaterial.SetFloat("_R_CX", settings.L_RX);
            skyboxMaterial.SetFloat("_R_CY", settings.L_RY);
            skyboxMaterial.SetFloat("_L_RX", settings.R_CX);
            skyboxMaterial.SetFloat("_L_RY", settings.R_CY);
            skyboxMaterial.SetFloat("_R_RX", settings.R_RX);
            skyboxMaterial.SetFloat("_R_RY", settings.R_RY);
            skyboxMaterial.SetFloat("_a", settings.a);
            skyboxMaterial.SetFloat("_b", settings.b);
            skyboxMaterial.SetFloat("_c", settings.c);

            // Load default player settings
            LoadUserSettings();
        }
        else
        {
            Debug.LogWarning("Save file not found!");
        }
    }

    void LoadUserSettings()
    {
        foreach (GameObject overlay in overlayList)
        {
            foreach (OverlayConfig overlayConfig in settings.users[currUserIdx].overlaySettings)
            {
                if (overlay.name == overlayConfig.overlayName)
                {
                    Vector3 newPos = new Vector3(overlayConfig.xPosition, overlayConfig.yPosition, 0);
                    overlay.transform.localPosition = newPos;
                    overlay.SetActive(overlayConfig.isVisible);
                }
            }
        }
    }

    void StereoCallback(Toggle toggle)
    {
        if (toggle.isOn)
            skyboxMaterial.SetInt("_Mode", 0);
        else
            skyboxMaterial.SetInt("_Mode", 1);

    }

}