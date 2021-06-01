using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveFormUtil
{
    public static float[] GetWaveformFromAudio (AudioClip audio, int size, float sat) 
    {
        float[] samples = new float[audio.channels * audio.samples];
        float[] waveform = new float[size];
        audio.GetData(samples, 0);
        // Number of samples in a waveform column
        int packSize = audio.samples * audio.channels / size;
        float max = 0f;
        int column = 0;
        int sampleInColumn = 0;
        // Convert audio sample values into waveform columns
        for (int i = 0; i < audio.channels * audio.samples; i++)
        {
            // Summarize all samples into a column value
            waveform[column] += Mathf.Abs(samples[i]);
            sampleInColumn++;
            if (sampleInColumn > packSize)
            {
                // Store max waveform column value for normalization purposes
                if (max < waveform[column])
                    max = waveform[column];
                column++;
                sampleInColumn = 0;
            }
        }
        //Normalize waveform column values
        for (int i = 0; i < size; i++)
        {
            waveform[i] /= (max * sat);
            if (waveform[i] > 1f)
                waveform[i] = 1f;
        }

        return waveform;
    }
 
    public static Texture2D PaintWaveformSpectrum(float[] waveform, int height, Color c) 
    {
        Texture2D tex = new Texture2D (waveform.Length, height, TextureFormat.RGBA32, false);

        // Draw all waveform columns up and down
        for (int x = 0; x < waveform.Length; x++) {
            for (int y = 0; y <= waveform [x] * (float)height / 2f; y++) {
                tex.SetPixel (x, (height / 2) + y, c);
                tex.SetPixel (x, (height / 2) - y, c);
            }
        }
        tex.Apply ();

        return tex;
    }
}
