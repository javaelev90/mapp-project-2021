using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using TMPro;

public class SettingsMenuController : MonoBehaviour
{
    [Header("Volume properties")]
    [SerializeField] AudioMixer audioMixer;
    [SerializeField] Slider musicSlider;
    [SerializeField] Slider effectsSlider;
    [SerializeField] TMP_Text musicText;
    [SerializeField] TMP_Text effectsText;
    [Header("Graphics properties")]
    [SerializeField] Toggle limitFPSToggle;

    float defaultMusicVolume = 0f;
    float defaultEffectsVolume = -8f;

    void Start()
    {
        InitializeVolume();
        InitializeTargetFPS();
    }

    void InitializeTargetFPS()
    {
        float tmpValue = Database.Instance.settingsRepository.GetFPSSetting();
        if (tmpValue == 0f || tmpValue == 999f)
        {
            limitFPSToggle.isOn = false;
        }
        else if (tmpValue == 1f)
        {
            limitFPSToggle.isOn = true;
        }
    }

    void InitializeVolume()
    {
        float tmpValue = 0f;

        // Init music volume
        tmpValue = Database.Instance.settingsRepository.GetVolumeSetting();
        if (tmpValue != 999f)
        {
            audioMixer.SetFloat("musicVolume", tmpValue);
            musicSlider.value = tmpValue;
            musicText.text = "Music " + To0100(tmpValue, -80f, 0f);
        }
        else if (tmpValue == 999f)
        {
            Database.Instance.settingsRepository.UpdateVolumeSetting(defaultMusicVolume);
            audioMixer.SetFloat("musicVolume", defaultMusicVolume);
            musicSlider.value = defaultMusicVolume;
            musicText.text = "Music " + To0100(defaultMusicVolume, -80f, 0f);
        }

        // Init effects volume
        tmpValue = Database.Instance.settingsRepository.GetEffectsVolumeSetting();
        if (tmpValue != 999f)
        {
            audioMixer.SetFloat("effectsVolume", tmpValue);
            effectsSlider.value = tmpValue;
            effectsText.text = "Effects " + To0100(tmpValue, -80f, 0f);
        }
        else if (tmpValue == 999f)
        {
            Database.Instance.settingsRepository.UpdateEffectsVolumeSetting(defaultEffectsVolume);
            audioMixer.SetFloat("effectsVolume", defaultEffectsVolume);
            musicSlider.value = defaultEffectsVolume;
            musicText.text = "Music " + To0100(defaultEffectsVolume, -80f, 0f);
        }

    }

    public void SetMusicVolume(float volume)
    {
        audioMixer.SetFloat("musicVolume", volume);
        Database.Instance.settingsRepository.UpdateVolumeSetting(volume);
        musicText.text = "Music " + To0100(volume, -80f, 0f);
    }

    public void SetEffectsVolume(float volume)
    {
        audioMixer.SetFloat("effectsVolume", volume);
        Database.Instance.settingsRepository.UpdateEffectsVolumeSetting(volume);
        effectsText.text = "Effects " + To0100(volume, -80, 0);
    }

    public void SetFPS(bool toggle)
    {
        // The game gets the value from the database to set the target FPS elsewhere
        Database.Instance.settingsRepository.UpdateFPSSetting((toggle == false ? 0f : 1f));
    }

    int To0100(float value, float minOld, float maxOld)
    {
        float minNew = 0f;
        float maxNew = 100f;
        float newValue = ((maxNew - minNew) / (maxOld - minOld) * (value - maxOld)) + maxNew;
        return Mathf.RoundToInt(newValue);
    }
}
