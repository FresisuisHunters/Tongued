using UnityEngine;
using UnityEngine.Audio;

namespace Hairibar.Audio.SFX
{
    public class RepeatAtFrequencySFXPlayer : MonoBehaviour
    {
        #region Inspector
        public AudioMixerGroup mixerGroup;
        public SFXClip sfx;
        public float frequency;
        #endregion

        private AudioSource[] audioSources;

        #region Private State
        private int nextSourceToSchedule;
        private double timeAtLastScheduleStart;
        private double lastScheduledTime;
        #endregion


        private void Update()
        {
            double t = AudioSettings.dspTime;

            if (lastScheduledTime > t)
            {
                audioSources[1 - nextSourceToSchedule].SetScheduledStartTime(timeAtLastScheduleStart + 1 / frequency);
            }
            else
            {
                ScheduleClip(t + 1 / frequency);
            }
        }

        private void OnEnable()
        {
            ScheduleClip(AudioSettings.dspTime);
        }

        private void ScheduleClip(double time)
        {
            AudioSource source = audioSources[nextSourceToSchedule];
            source.clip = sfx.RandomAudioClip;
            source.pitch = sfx.RandomPitch;
            source.volume = sfx.Volume;

            source.PlayScheduled(time);

            lastScheduledTime = time;
            timeAtLastScheduleStart = AudioSettings.dspTime;
            nextSourceToSchedule = 1 - nextSourceToSchedule;
        }


        private void Awake()
        {
            audioSources = new AudioSource[2];

            GameObject holder = new GameObject("RepeatAtFrequencySFXPlayer AudioSources");
            holder.transform.SetParent(transform, false);

            audioSources[0] = holder.AddComponent<AudioSource>();
            audioSources[1] = holder.AddComponent<AudioSource>();

            audioSources[0].outputAudioMixerGroup = mixerGroup;
            audioSources[1].outputAudioMixerGroup = mixerGroup;
        }
    }
}
