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
    public Transform tf;

    public void recordOverlayConfig(GameObject overlay)
    {
        overlayName = overlay.name;
        isVisible = overlay.activeInHierarchy;
        tf = overlay.transform;
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
        foreach(GameObject overlay in overlays)
        {
            OverlayConfig newOverlay = new OverlayConfig();
            newOverlay.recordOverlayConfig(overlay);
            overlaySettings.Add(newOverlay);
        }
    }

    void RestoreOverlaySettings(List<GameObject> overlays)
    {
        /*
         * for overlay in overlays:
         *      find matching name
         *      set enabled, and recttransform based on overlaySettings
         */
    }

}

[System.Serializable]
public class RovVrSettings
{
    // Settings 
    // TODO: Save these in a JSON
    public List<User> users = new List<User> { }; // TODO: finish implementing this class
    public string PTGUIFilename;
    public string LCMURL;
}

public class SaveData
{
    private RovVrSettings applicationSettings;

    public SaveData(RovVrSettings settings)
    {
        applicationSettings = settings;
        SaveIntoJson();
    }
    

    public void SaveIntoJson()
    {
        string serializedSettings = JsonUtility.ToJson(applicationSettings);
        System.IO.File.WriteAllText(Application.persistentDataPath + "/ROV-VR_Application_Settings.json", serializedSettings);
    }
}



public class PopupSettings : MonoBehaviour
{
    [Header("Settings UI")]
    public Canvas SettingsCanvas;
    public Button CloseButton;
    public Button ConfirmGlobalSettingsButton;
    public Button SaveAllButton;

    [Header("User")]
    public Dropdown ChooseUserDropdown;
    public InputField NewUserInputField;
    public Button AddUserButton;
    public Button LoadUserSettingsButton;
    public Button WriteUserSettingsButton;

    [Header("LCM")]
    public InputField LCMInputField;
    public LCMListener LCMListenerObject;

    [Header("PTGUI")]
    public Button ChooseFileButton;
    public Text PTGUIFilenameText;
    public Material skyboxMaterial;

    [Header("Overlays")]
    public GameObject mainOverlayCanvas;
    // TODO: Include all the overlays here so we can save their xy coords

    // TODO: load in individaul gui gameobjects
    // TODO: create a list of gui gameobjects
    // TODO: modify WriteUserSettingsCallback to take in this list 
    // TODO: WE LEFT OFF HERE

    RovVrSettings settings = new RovVrSettings();

    int currUserIdx = -1;
    bool prevEscBool = false;
    bool currEscBool = false;


    void Start()
    {
        // TODO: When we implement load settings, remember to update LCM URL at start

        ChooseFileButton.onClick.AddListener(ChooseFileCallback);
        CloseButton.onClick.AddListener(CloseCallback);
        ConfirmGlobalSettingsButton.onClick.AddListener(ConfirmGlobalSettingsCallback);
        AddUserButton.onClick.AddListener(AddUserCallback);
        SaveAllButton.onClick.AddListener(SaveAllCallback);
        WriteUserSettingsButton.onClick.AddListener(WriteUserSettingsCallback);

        ChooseUserDropdown.onValueChanged.AddListener(delegate 
            { ChooseUserCallback(ChooseUserDropdown); });
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

   
    void ChooseFileCallback()
    {
        settings.PTGUIFilename = EditorUtility.OpenFilePanel("Select Calibration File", "", "pts");
        PTGUIFilenameText.text = settings.PTGUIFilename;

        var fileContent = File.ReadAllBytes(settings.PTGUIFilename);
        string jsonString = System.Text.Encoding.UTF8.GetString(fileContent);
        JSONNode PTGUISettings = JSON.Parse(jsonString);
        string test_a = PTGUISettings["contenttype"].Value;

        //if (path.Length != 0)
        //{
        //    var fileContent = File.ReadAllBytes(path);
        //    texture.LoadImage(fileContent);
        //}
        //JsonTextReader reader = new JsonTextReader(new StringReader(json));
    }

    void CloseCallback()
    {
        SettingsCanvas.enabled = false;
    }

    void ConfirmGlobalSettingsCallback()
    {
        settings.LCMURL = LCMInputField.text;
        LCMListenerObject.changeURL(settings.LCMURL);
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
        print("Attempting to save data");
        SaveData dataSaver = new SaveData(settings);
    }

    void WriteUserSettingsCallback()
    {
        if (currUserIdx == -1)
        {
            Debug.LogWarning("No user selected. Overlay settings will not be saved");
            return;
        }

        // Get all overlay gameobjects from mainOverlayCanvas
        Transform[] allChildren = mainOverlayCanvas.GetComponentsInChildren<Transform>(includeInactive: true);
        List<GameObject> overlayList = new List<GameObject> { };

        foreach (Transform child in allChildren)
        {
            if (child.parent == mainOverlayCanvas.transform)
            {
                GameObject overlay = child.gameObject;
                overlayList.Add(overlay);
            }
        }

        settings.users[currUserIdx].RecordOverlaySettings(overlayList);
    }

    void ChooseUserCallback(Dropdown ChooseUserDropdown)
    {
        int i = ChooseUserDropdown.value;
        currUserIdx = i;
    }
}