using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SampleAccurateLoop : MonoBehaviour
{
    [Header("Sample rates")]
    //public int fileSampleRate = 44100;
    public int targetSampleRate = 48000;

    [Header("Loop Settings")]
    public int loopStartSample = 1290240; // Start of loop in samples
    public int loopEndSample = 5274964;   // End of loop in samples


    private AudioSource audioSource;
    private float[] originalAudioData;
    private float[] resampledAudioData;
    private int numChannels;
    private int currentSample = 0;
    private int originalSampleRate = 0;
    private int adjustedLoopStart;
    private int adjustedLoopEnd;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();

        if (audioSource.clip == null)
        {
            Debug.LogError("AudioSource does not have an assigned AudioClip!");
            return;
        }
    }

    /// <summary>
    /// Resamples the clip and adjusts the loop points to the target sample rate
    /// </summary>
    public void PrepareClip()
    {
        originalSampleRate = audioSource.clip.frequency;
        numChannels = audioSource.clip.channels;

        adjustedLoopStart = Mathf.RoundToInt(loopStartSample * ((float)targetSampleRate / originalSampleRate));
        adjustedLoopEnd = Mathf.RoundToInt(loopEndSample * ((float)targetSampleRate / originalSampleRate));

        // Load original audio data
        originalAudioData = new float[audioSource.clip.samples * numChannels];
        audioSource.clip.GetData(originalAudioData, 0);

        // Resample the clip to target sample rate
        resampledAudioData = ResampleAudio(originalAudioData, originalSampleRate, targetSampleRate, numChannels);

        // Replace the clip with resampled audio
        AudioClip resampledClip = AudioClip.Create("ResampledClip", resampledAudioData.Length / numChannels, numChannels, targetSampleRate, false);
        resampledClip.SetData(resampledAudioData, 0);
        audioSource.clip = resampledClip;
    }

    private float[] ResampleAudio(float[] source, int sourceRate, int targetRate, int channels)
    {
        float ratio = (float)sourceRate / targetRate;
        int newSampleCount = Mathf.CeilToInt(source.Length / ratio);
        float[] resampled = new float[newSampleCount];

        for (int i = 0; i < newSampleCount / channels; i++)
        {
            int oldIndex = Mathf.FloorToInt(i * ratio) * channels;
            for (int channel = 0; channel < channels; channel++)
            {
                if (oldIndex + channel < source.Length)
                    resampled[i * channels + channel] = source[oldIndex + channel];
            }
        }

        return resampled;
    }

    private void OnAudioFilterRead(float[] data, int channels)
    {
        if (resampledAudioData == null || resampledAudioData.Length == 0) return;

        int bufferSamples = data.Length / channels;

        for (int i = 0; i < bufferSamples; i++)
        {
            if (currentSample >= adjustedLoopEnd)
            {
                currentSample = adjustedLoopStart;
            }

            int sampleIndex = currentSample * numChannels;

            for (int channel = 0; channel < channels; channel++)
            {
                int audioDataIndex = sampleIndex + channel;

                if (audioDataIndex < resampledAudioData.Length)
                {
                    data[i * channels + channel] = resampledAudioData[audioDataIndex];
                }
                else
                {
                    data[i * channels + channel] = 0; // Prevent out-of-bounds errors
                }
            }

            currentSample++;
        }
    }
}
