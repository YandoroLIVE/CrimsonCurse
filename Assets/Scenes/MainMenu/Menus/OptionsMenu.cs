using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class OptionsMenu : MonoBehaviour
{
    public TMP_Dropdown resolutionDropdown;
    public TMP_Dropdown fullscreenDropdown;
    public Toggle vSyncToggle;
    public Slider maxFPSSlider;
    public TextMeshProUGUI maxFPSText;
    public GameObject mainMenu;
    public GameObject optionsMenu;
    public Button applyButton;
    public Button cancelButton;
    private Resolution[] resolutions;

    void Start()
    {
        InitializeResolutionOptions();
        InitializeFullscreenOptions();
        InitializeVSyncToggle();
        InitializeMaxFPSSlider();
        SetupNavigation();
    }

    void InitializeResolutionOptions()
    {
        resolutions = Screen.resolutions;
        resolutionDropdown.ClearOptions();

        List<string> options = new List<string>();
        int currentResolutionIndex = 0;

        for (int i = 0; i < resolutions.Length; i++)
        {
            float aspectRatio = (float)resolutions[i].width / resolutions[i].height;
            if (Mathf.Approximately(aspectRatio, 16f / 9f))
            {
                string option = resolutions[i].width + " x " + resolutions[i].height;
                options.Add(option);

                if (resolutions[i].width == Screen.currentResolution.width &&
                    resolutions[i].height == Screen.currentResolution.height)
                {
                    currentResolutionIndex = options.Count - 1;
                }
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

        fullscreenDropdown.value = Screen.fullScreenMode == FullScreenMode.FullScreenWindow ? 1 :
                                   Screen.fullScreenMode == FullScreenMode.MaximizedWindow ? 2 : 0;

        fullscreenDropdown.RefreshShownValue();
    }

    void InitializeVSyncToggle()
    {
        vSyncToggle.isOn = QualitySettings.vSyncCount > 0;
    }

    void InitializeMaxFPSSlider()
    {
        maxFPSSlider.minValue = 30;
        maxFPSSlider.maxValue = 240;
        maxFPSSlider.value = Application.targetFrameRate > 0 ? Application.targetFrameRate : 60;
        maxFPSText.text = ((int)maxFPSSlider.value).ToString();
        maxFPSSlider.onValueChanged.AddListener(value => SetMaxFPS((int)value));
    }

    void SetupNavigation()
    {
        EventSystem.current.SetSelectedGameObject(resolutionDropdown.gameObject);
    }

    public void SetVSync(bool isOn)
    {
        QualitySettings.vSyncCount = isOn ? 1 : 0;
    }

    public void SetMaxFPS(int value)
    {
        Application.targetFrameRate = value;
        maxFPSText.text = value.ToString();
    }

    public void ApplySettings()
    {
        Resolution resolution = resolutions[resolutionDropdown.value];
        Screen.SetResolution(resolution.width, resolution.height, GetFullScreenMode());

        QualitySettings.vSyncCount = vSyncToggle.isOn ? 1 : 0;
        Application.targetFrameRate = (int)maxFPSSlider.value;
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
        optionsMenu.SetActive(false);
        EventSystem.current.SetSelectedGameObject(mainMenu);
    }
}