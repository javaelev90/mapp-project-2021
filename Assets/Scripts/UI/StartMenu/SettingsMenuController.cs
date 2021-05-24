using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using TMPro;

public class SettingsMenuController : MonoBehaviour
{
    [Header("Volume properties")]
    public AudioMixer audioMixer;
    public Slider musicSlider;
    public Slider effectsSlider;
    public TMP_Text musicText;
    public TMP_Text effectsText;
    [Header("Graphics properties")]
    [SerializeField] Toggle limitFPSToggle;

    private void Start()
    {
        // Init music volume
        float tmpValue = Database.Instance.settingsRepository.GetVolumeSetting();
        if (tmpValue != 999f)
        {
            audioMixer.SetFloat("musicVolume", tmpValue);
            musicSlider.value = tmpValue;
            musicText.text = "Music " + To0100(tmpValue, -80, 0);
        }

        // Init effects volume
        tmpValue = Database.Instance.settingsRepository.GetEffectsVolumeSetting();
        if(tmpValue != 999f)
        {
            audioMixer.SetFloat("effectsVolume", tmpValue);
            effectsSlider.value = tmpValue;
            effectsText.text = "Effects " + To0100(tmpValue, -80, 0);
        }

        // Init FPS
        // The game gets the value from the database to change FPS elsewhere
        tmpValue = Database.Instance.settingsRepository.GetFPSSetting();
        if (tmpValue == 0f)
        {
            limitFPSToggle.isOn = false;
        }
        else if(tmpValue == 1f)
        {
            limitFPSToggle.isOn = true;
        }

    }

    public void SetMusicVolume(float volume)
    {
        audioMixer.SetFloat("musicVolume", volume);
        Database.Instance.settingsRepository.UpdateVolumeSetting(volume);
        musicText.text = "Music " + To0100(volume, -80, 0);
    }

    public void SetEffectsVolume(float volume)
    {
        audioMixer.SetFloat("effectsVolume", volume);
        Database.Instance.settingsRepository.UpdateEffectsVolumeSetting(volume);
        effectsText.text = "Effects " + To0100(volume, -80, 0);
    }

    public void SetFPS(bool toggle)
    {
        // The game gets the value from the database to change the FPS elsewhere
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
