using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

public class OptionsMenu : MonoBehaviour
{
    [Header("UI Elements")]
    public TMP_Dropdown resolutionDropdown;
    public TMP_Dropdown windowModeDropdown;
    public Slider fpsSlider;
    public TMP_Text fpsText; // Textfeld für FPS-Anzeige
    public Button applyButton;
    public Button backButton;

    [Header("Menu Panels")]
    public GameObject optionsMenu;
    public GameObject mainMenu;

    private Resolution[] resolutions;
    private int selectedResolutionIndex;

    void Start()
    {

        SetupResolutionDropdown();
        SetupWindowModeDropdown();
        SetupFPSLimiter();

        applyButton.onClick.AddListener(ApplySettings);
        backButton.onClick.AddListener(BackToMainMenu);
        fpsSlider.onValueChanged.AddListener(UpdateFPSText); // Aktualisiert FPS-Anzeige
    }

    void SetupResolutionDropdown()
    {
        resolutions = Screen.resolutions;
        resolutionDropdown.ClearOptions();

        HashSet<string> addedResolutions = new HashSet<string>();
        selectedResolutionIndex = 0;

        for (int i = 0; i < resolutions.Length; i++)
        {
            int width = resolutions[i].width;
            int height = resolutions[i].height;
            float aspectRatio = (float)width / height;

            string resolutionText = width + "x" + height;
            if (Mathf.Approximately(aspectRatio, 16f / 9f) && !addedResolutions.Contains(resolutionText))
            {
                resolutionDropdown.options.Add(new TMP_Dropdown.OptionData(resolutionText));
                addedResolutions.Add(resolutionText);

                if (width == Screen.currentResolution.width && height == Screen.currentResolution.height)
                {
                    selectedResolutionIndex = resolutionDropdown.options.Count - 1;
                }
            }
        }

        resolutionDropdown.value = selectedResolutionIndex;
        resolutionDropdown.RefreshShownValue();
    }

    void SetupWindowModeDropdown()
    {
        windowModeDropdown.ClearOptions();
        windowModeDropdown.options.Add(new TMP_Dropdown.OptionData("Fullscreen"));
        windowModeDropdown.options.Add(new TMP_Dropdown.OptionData("Windowed"));

        windowModeDropdown.value = Screen.fullScreen ? 0 : 1;
        windowModeDropdown.RefreshShownValue();
    }

    void SetupFPSLimiter()
    {
        fpsSlider.minValue = 30;
        fpsSlider.maxValue = 120;
        fpsSlider.value = Application.targetFrameRate > 0 ? Application.targetFrameRate : 60;
        UpdateFPSText(fpsSlider.value); // Initiale Anzeige setzen
    }

    void UpdateFPSText(float value)
    {
        fpsText.text = Mathf.RoundToInt(value) + " FPS"; // Zeigt FPS als ganze Zahl an
    }

    public void ApplySettings()
    {
        string[] res = resolutionDropdown.options[resolutionDropdown.value].text.Split('x');
        int width = int.Parse(res[0]);
        int height = int.Parse(res[1]);
        Screen.SetResolution(width, height, Screen.fullScreen);

        Screen.fullScreen = windowModeDropdown.value == 0;

        Application.targetFrameRate = Mathf.RoundToInt(fpsSlider.value);
    }

    public void BackToMainMenu()
    {
        optionsMenu.SetActive(false);
        mainMenu.SetActive(true);
    }
}
