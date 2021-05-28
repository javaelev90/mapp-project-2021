using UnityEngine;
using UnityEngine.UI;

public class BeatVisualizer : MonoBehaviour
{
    [Header("Audio Settings")]
    [SerializeField] AudioSource musicAudioSource;
    [SerializeField] int visualizerSamples = 64;
    [Header("Audio objects")]
    [SerializeField] GameObject[] visualizerObjects;
    [SerializeField] Color visualizerColor;
    float minHeight;
    float maxHeight;
    float changeSensitivity = 0.2f;
    float[] spectrumData;
    // Start is called before the first frame update
    void Start()
    {
        spectrumData = new float[visualizerSamples];
        minHeight = Screen.height * 0.03f;
        maxHeight = Screen.height * 0.09f;
    }

    // Update is called once per frame
    void Update()
    {
        musicAudioSource.GetSpectrumData(spectrumData, 0, FFTWindow.Rectangular);
        for(int i = 0; i < visualizerObjects.Length; i++)
        {
            Vector2 newSize = visualizerObjects[i].GetComponent<RectTransform>().rect.size;
            newSize.y = Mathf.Lerp(newSize.y, minHeight + (spectrumData[i] * (maxHeight - minHeight) * 5f), changeSensitivity);
            
            visualizerObjects[i].GetComponent<RectTransform>().sizeDelta = newSize;
            visualizerObjects[i].GetComponent<Image>().color = visualizerColor;
        }
        // float maxSpectrumSample = 0f;
        // foreach(float spectrumSample in spectrumData){
        //     if(spectrumSample > maxSpectrumSample)
        //     {
        //         maxSpectrumSample = spectrumSample;
        //     }
        // }
        // Color newColor = new Color(visualizerColor.r, visualizerColor.b, visualizerColor.g, Mathf.Abs(maxSpectrumSample));
        // visualizerObjects[0].GetComponent<Image>().color = Color.Lerp(visualizerColor, newColor, changeSensitivity);
    }
}
