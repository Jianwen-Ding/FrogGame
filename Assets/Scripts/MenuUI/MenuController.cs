using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class MenuController : MonoBehaviour
{
    [Header("Customization")]
    public Animator cAnim;
    private int customizingHash;
    private int LoadoutHash;
    [SerializeField] private Toggle marchingHatT = null;

    [Header("Weapon Loadouts")]
    
    [Header("Volume Settings")]
    [SerializeField] private TMP_Text volumeTextValue = null;
    [SerializeField] private Slider volumeSlider = null;
    [SerializeField] private float defaultVolume = 0.5f;

    [Header("Graphics Settings")]
    [SerializeField] private TMP_Text brightnessTextValue = null;
    [SerializeField] private Slider brightnessSlider = null;
    [SerializeField] private float defaultBrightnesss = 1.0f;
    [SerializeField] private Toggle fullScreenToggle = null;

    private int qualityLevel;
    [SerializeField] private TMP_Dropdown QualityDropdown;
    private bool isFullScreen;
    private float brightnessLevel;

    //Resolution
    [SerializeField] private TMP_Dropdown resolutionDropdown;
    private Resolution[] resolutions;
    
    [Header("Gameplay Settings")]
    [SerializeField] private TMP_Text sensTextValue = null;
    [SerializeField] private Slider sensSlider = null;
    [SerializeField] private TMP_Text fovTextValue = null;
    [SerializeField] private Slider fovSlider = null;
    [SerializeField] private float defaultFOV = 60.0f;
    public float mainFOV = 60.0f;
    [SerializeField] private float defaultSens = 20.0f;
    public float mainSens = 20.0f;
    [SerializeField] private Toggle invertYToggle = null;

    [Header("Confirmation")]
    [SerializeField] private GameObject confirmationPrompt = null;


    [Header("Levels To Load")]
    public GameObject LevelFade;
    public string _newMatch;
    public string _PracticeArea;
    //private string levelToLoad;

    private void Start()
    {
        customizingHash = Animator.StringToHash("isCustomizing");
        LoadoutHash = Animator.StringToHash("isLoadout");

        resolutions = Screen.resolutions;
        resolutionDropdown.ClearOptions();

        List<string> options = new List<string>();

        int currentResolutionIndex = 0;

        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + " x " + resolutions[i].height;
            options.Add(option);

            if (resolutions[i].width == Screen.width && resolutions[i].height == Screen.height)
                currentResolutionIndex = i;
        }

        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();
    }

    IEnumerator JoinMatchFade()
    {
        yield return new WaitForSeconds(2f);
        StartCoroutine(JoinMatchDelay());
    }

    IEnumerator JoinMatchDelay()
    {
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene(_newMatch);
    }

    IEnumerator JoinPracticeFade()
    {
        yield return new WaitForSeconds(3f);
        StartCoroutine(JoinPracticeDelay());
    }

    IEnumerator JoinPracticeDelay()
    {
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene(_PracticeArea);
    }


    public void JoinMatchDialogYes()
    {
        LevelFade.SetActive(true);
        StartCoroutine(JoinMatchFade());
    }

    public void PracticeDialogYes()
    {
        LevelFade.SetActive(true);
        StartCoroutine(JoinPracticeFade());
    }

    public void Customization()
    {
        cAnim.SetBool(customizingHash, true);
    }
    
    public void CustomizationReturn()
    {
        cAnim.SetBool(customizingHash, false);
    }

    public void Loadout()
    {
        cAnim.SetBool(LoadoutHash, true);
        cAnim.SetBool(customizingHash, false);
    }

    public void LoadoutReturn()
    {
        cAnim.SetBool(LoadoutHash, false);
        cAnim.SetBool(customizingHash, true);
    }

    public void ToggleHat()
    {
        if (marchingHatT.isOn)
            PlayerPrefs.SetInt("masterMarchingHat", 1);
        else
            PlayerPrefs.SetInt("masterMarchingHat", 0);
    }

    public void ExitButton()
    {
        Application.Quit();
    }

    public void SetVolume(float volume)
    {
        AudioListener.volume = volume;
        volumeTextValue.text = volume.ToString("0.0");
    }

    public void VolumeApply()
    {
        PlayerPrefs.SetFloat("masterVolume", AudioListener.volume);
        StartCoroutine(ConfirmationBox());
    }

    public void ResetDefaults(string MenuType)
    {
        if (MenuType == "Audio")
        {
            AudioListener.volume = defaultVolume;
            volumeSlider.value = defaultVolume;
            volumeTextValue.text = defaultVolume.ToString("0.0");
            VolumeApply();
        }

        if (MenuType == "Gameplay")
        {
            sensTextValue.text = defaultSens.ToString("0.0");
            sensSlider.value = defaultSens;
            mainSens = defaultSens;
            fovTextValue.text = defaultFOV.ToString("0.0");
            fovSlider.value = defaultFOV;
            mainFOV = defaultFOV;
            invertYToggle.isOn = false;
            GameplayApply();
        }

        if (MenuType == "Graphics")
        {
            brightnessTextValue.text = defaultBrightnesss.ToString("0.0");
            brightnessSlider.value = defaultBrightnesss;
            fullScreenToggle.isOn = true;
            isFullScreen = true;
            SetQuality(1);
            SetResolution(resolutions.Length - 1);
            SetQuality(2);
            QualityDropdown.value = 2;
            resolutionDropdown.value = resolutions.Length;
            GraphicsApply();
        }

    }

    public void SetBrightness(float brightness)
    {
        brightnessLevel = brightness;
        brightnessTextValue.text = brightness.ToString("0.0");

    }

    public void SetFullscreen(bool fullScreen)
    {
        isFullScreen = fullScreen;
    }

    public void SetQuality(int qualityIndex)
    {
        qualityLevel = qualityIndex;
    }

    public void SetSensitivity(float sensitivity)
    {
        mainSens = sensitivity;
        sensTextValue.text = sensitivity.ToString("0.0");
    }

    public void SetFOV(float fov)
    {
        mainFOV = fov;
        fovTextValue.text = fov.ToString("0.0");
    }

    public void SetResolution(int resolutionIndex)
    {
        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }

    public void GraphicsApply()
    {
        PlayerPrefs.SetFloat("masterBrightness", brightnessLevel);
        //Sets post processing brightness
        
        PlayerPrefs.SetInt("masterQuality", qualityLevel);
        QualitySettings.SetQualityLevel(qualityLevel);

        PlayerPrefs.SetInt("MasterFullscreen", (isFullScreen ? 1 : 0));
        Screen.fullScreen = isFullScreen;
        
        StartCoroutine(ConfirmationBox());
    }

    public void GameplayApply()
    {
        if (invertYToggle.isOn)
            PlayerPrefs.SetInt("masterInvertY", 1);
        else
            PlayerPrefs.SetInt("masterInvertY", 0);

        PlayerPrefs.SetFloat("masterSen", mainSens);
        PlayerPrefs.SetFloat("masterFOV", mainFOV);
        StartCoroutine(ConfirmationBox());
    }

    public IEnumerator ConfirmationBox()
    {
        confirmationPrompt.SetActive(true);
        yield return new WaitForSeconds(2);
        confirmationPrompt.SetActive(false);
    }


    /*public void LoadGameDialogYes()
    {
        SceneManager.LoadScene(_NewGameLevel);
    }*/
    
    
}
