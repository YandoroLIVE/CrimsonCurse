using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionsMenu : MonoBehaviour
{
    public Dropdown resolutionDropdown;
    public Dropdown fullscreenDropdown;
    public Toggle vSyncToggle;
    public GameObject mainMenu;

    private Resolution[] resolutions;

    void Start()
    {
        InitializeResolutionOptions();
        InitializeFullscreenOptions();
        InitializeVSyncToggle();
    }

    void InitializeResolutionOptions()
    {
        resolutions = Screen.resolutions;
        resolutionDropdown.ClearOptions();

        List<string> options = new List<string>();
        int currentResolutionIndex = 0;

        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + " x " + resolutions[i].height;
            options.Add(option);

            if (resolutions[i].width == Screen.currentResolution.width &&
                resolutions[i].height == Screen.currentResolution.height)
            {
                currentResolutionIndex = i;
            }
        }

        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();
    }

    void InitializeFullscreenOptions()
    {
        List<string> options = new List<string> { "Windowed", "Fullscreen", "Borderless" };
        fullscreenDropdown.ClearOptions();
        fullscreenDropdown.AddOptions(options);

        // Set dropdown value based on current screen mode
        fullscreenDropdown.value = Screen.fullScreenMode == FullScreenMode.FullScreenWindow ? 1 :
                                   Screen.fullScreenMode == FullScreenMode.MaximizedWindow ? 2 : 0;

        fullscreenDropdown.RefreshShownValue();
    }

    void InitializeVSyncToggle()
    {
        vSyncToggle.isOn = QualitySettings.vSyncCount > 0;
    }

    public void SetVSync(bool isOn)
    {
        QualitySettings.vSyncCount = isOn ? 1 : 0;
    }

    public void ApplySettings()
    {
        // Set Resolution
        Resolution resolution = resolutions[resolutionDropdown.value];
        Screen.SetResolution(resolution.width, resolution.height, GetFullScreenMode());

        // Set VSync
        QualitySettings.vSyncCount = vSyncToggle.isOn ? 1 : 0;
    }

    private FullScreenMode GetFullScreenMode()
    {
        switch (fullscreenDropdown.value)
        {
            case 1: return FullScreenMode.FullScreenWindow;
            case 2: return FullScreenMode.MaximizedWindow;
            default: return FullScreenMode.Windowed;
        }
    }

    public void CancelButton()
    {
        mainMenu.SetActive(true);
        gameObject.SetActive(false);
    }
}
