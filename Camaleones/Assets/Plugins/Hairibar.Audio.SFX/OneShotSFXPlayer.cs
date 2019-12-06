using System.Collections;
using UnityEngine.Audio;
using UnityEngine;
using UnityEngine.Assertions;

#pragma warning disable 649
namespace Hairibar.Audio.SFX
{
    public class OneShotSFXPlayer : MonoBehaviour
    {
        #region Inspector
        [SerializeField] private AudioSource configurationAudioSource;
        [SerializeField] private int numberOfAudioSources = 1;
        [SerializeField] private AudioMixerGroup outputMixerGroup;
        #endregion

        #region References
        private AudioSource[] audioSources;
        #endregion

        #region Private State
        private SFXInstance[] sfxInstances;

        private bool IsPlaying(int index)
        {
            return sfxInstances[index].IsPlaying;
        }
        #endregion

        #region Requests
        /// <summary>
        /// Requests the SFX to be played.
        /// </summary>
        public void RequestSFX(SFXClip requestedClip)
        {
            ProcessSFXRequest(requestedClip);
        }

        /// <summary>
        /// Requests the SFX to be played. Takes a callback that is called when the sound ends, or when it would have ended had it played.
        /// Returns wether the request was granted.
        /// </summary>
        public bool RequestSFX(SFXClip requestedClip, SFXEndCallback callback)
        {
            return ProcessSFXRequest(requestedClip, callback);
        }

        private bool ProcessSFXRequest(SFXClip requestedClip, SFXEndCallback callback = null)
        {
            if (!isActiveAndEnabled) return false;

            bool requestGranted = true;

            //Check for duplicate instances. Priority still applies for duplicate behaviour.
            SFXInstance duplicateInstance = GetDuplicateInstance(requestedClip);
            if (duplicateInstance != null)
            {
                switch (requestedClip.duplicateBehaviour)
                {
                    case SFXClip.DuplicateInstanceBehaviour.Allow:
                        requestGranted = true;
                        break;

                    case SFXClip.DuplicateInstanceBehaviour.StopOld:
                        if (requestedClip.Priority >= duplicateInstance.sfxClip.Priority)
                        {
                            StopPlaying(duplicateInstance);
                            requestGranted = true;
                        }
                        else
                        {
                            requestGranted = false;
                        }
                        break;

                    case SFXClip.DuplicateInstanceBehaviour.SkipNew:
                        requestGranted = requestedClip.Priority > duplicateInstance.sfxClip.Priority;
                        break;
                }
            }

            //Request an audio source
            int acquiredAudioSourceIndex = -1;
            if (requestGranted)
            {
                acquiredAudioSourceIndex = RequestAudioSource(requestedClip.Priority);
                requestGranted = acquiredAudioSourceIndex != -1;
            }

            //If the request was granted, we play the clip
            if (requestGranted)
            {
                SFXInstance instance = PlayClip(requestedClip, acquiredAudioSourceIndex);
                if (callback != null) instance.endCallback = callback;
            }
            //Else, if a callback was provided, we pretend that we played the clip for its sake.
            else if (callback != null)  
            {
                StartCoroutine(_DoDelayedCallback(callback, requestedClip.RandomAudioClip.length));
            }

            return requestGranted;
        }

        /// <summary>
        /// Return priority: Free AudioSource, playing lower priority, playing same priority. Returns -1 if request was denied.
        /// </summary>
        private int RequestAudioSource(SFXPriority priority)
        {
            for (int i = 0; i < audioSources.Length; i++)
            {
                if (!IsPlaying(i))
                {
                    return i;
                }
                else
                {
                    Assert.IsNotNull(sfxInstances[i].sfxClip, "An SFXInstance exists with no clip in it. This should not be possible. (At " + gameObject.name + ".)");
                    if (sfxInstances[i].sfxClip.Priority < priority) return i;
                }
            }

            //If we got here, it's time to see if there is one playing same priority.
            for (int i = 0; i < audioSources.Length; i++)
            {
                if (sfxInstances[i].sfxClip.Priority == priority) return i;
            }

            //If we got here, the request was denied.
            return -1;
        }

        /// <summary>
        /// If the given SFXClip is already being played, returns that instance. If there are multiple, just returns one. Returns null if it isn't already being played.
        /// </summary>
        /// <returns></returns>
        private SFXInstance GetDuplicateInstance(SFXClip requestedClip)
        {
            for (int i = 0; i < sfxInstances.Length; i++)
            {
                if (SFXClip.AreDuplicates(requestedClip, sfxInstances[i].sfxClip)) return sfxInstances[i];
            }

            return null;
        }
        #endregion

        #region Audio Source operations
        private SFXInstance PlayClip(SFXClip clip, int audioSourceIndex, SFXEndCallback callback = null)
        {
            SFXInstance sfxInstance = sfxInstances[audioSourceIndex];

            if (sfxInstance.IsPlaying) StopPlaying(sfxInstance);

            //Prepare the audio source and play it
            sfxInstance.AssignedAudioSource.clip = clip.RandomAudioClip;
            sfxInstance.AssignedAudioSource.volume = clip.Volume;
            sfxInstance.AssignedAudioSource.pitch = clip.RandomPitch;

            sfxInstance.AssignedAudioSource.Play();

            //Remember the playing clip
            sfxInstance.sfxClip = clip;
            sfxInstance.audioClip = sfxInstance.AssignedAudioSource.clip;

            //Set the callback
            sfxInstance.endCallback = callback;

            //Start the coroutine that checks when the clip is over
            if (sfxInstance.checkForEndCoroutine != null) Debug.LogError("There was already a coroutine running! The SFXPlayer is fucked.");
            sfxInstance.checkForEndCoroutine = StartCoroutine(_CheckForClipEnd(sfxInstance));

            //print("Playing " + sfxInstance.sfxClip.name + " at audio source #" + audioSourceIndex);

            return sfxInstance;
        }

        private void StopPlaying(SFXInstance sfxInstance)
        {
            if (sfxInstance.checkForEndCoroutine != null)
            {
                StopCoroutine(sfxInstance.checkForEndCoroutine);
            }

            //Execute the callback. Delay the call if the sound was interrupted.
            if (sfxInstance.endCallback != null)
            {
                float timeLeft = sfxInstance.audioClip.length - sfxInstance.AssignedAudioSource.time;
                if (timeLeft <= 0)
                {
                    sfxInstance.endCallback();
                }
                else
                {
                    SFXEndCallback callback = sfxInstance.endCallback;
                    StartCoroutine(_DoDelayedCallback(callback, timeLeft));
                }
            }
            
            sfxInstance.AssignedAudioSource.Stop();
            sfxInstance.AssignedAudioSource.clip = null;

            sfxInstance.Reset();
        }

        private void DoSFXEndCallback(SFXEndCallback callback)
        {
            callback();
        }

        private IEnumerator _CheckForClipEnd(SFXInstance sfxInstance)
        {
            AudioSource audioSource = sfxInstance.AssignedAudioSource;
            yield return new WaitUntil(() => !audioSource.isPlaying);
            StopPlaying(sfxInstance);
        }

        private IEnumerator _DoDelayedCallback(SFXEndCallback callback, float delay)
        {
            yield return new WaitForSeconds(delay);

            callback();
        }
        #endregion

        #region Initialization
        private void Awake()
        {
            CreateAudioSources();

            InitializeSFXInstances();
        }

        private void CreateAudioSources()
        {
            audioSources = new AudioSource[numberOfAudioSources];
            GameObject holder;

            if (configurationAudioSource) holder = configurationAudioSource.gameObject;
            else
            {
                holder = new GameObject("OneShotSFXPlayer AudioSources");
                holder.transform.SetParent(transform, false);
            }

            for (int i = 0; i < numberOfAudioSources; i++)
            {
                if (configurationAudioSource)
                {
                    if (i == 0) audioSources[i] = configurationAudioSource;
                    else audioSources[i] = Instantiate(configurationAudioSource);
                }
                else
                {
                    audioSources[i] = holder.AddComponent<AudioSource>();
                }
            }

            if (!configurationAudioSource)
            {
                //Set them up
                foreach (AudioSource audioSource in audioSources)
                {
                    audioSource.outputAudioMixerGroup = outputMixerGroup;
                    audioSource.playOnAwake = false;
                }
            }
        }

        private void InitializeSFXInstances()
        {
            sfxInstances = new SFXInstance[numberOfAudioSources];
            for (int i = 0; i < numberOfAudioSources; i++)
            {
                sfxInstances[i] = new SFXInstance(audioSources[i]);
            }
        }
        #endregion

        public delegate void SFXEndCallback();

        private class SFXInstance
        {
            public SFXClip sfxClip;
            public AudioClip audioClip;
            public Coroutine checkForEndCoroutine;
            public SFXEndCallback endCallback;

            public AudioSource AssignedAudioSource { get; private set; }

            public bool IsPlaying
            {
                get{
                    return ((AssignedAudioSource.isPlaying && AssignedAudioSource.clip != null) || checkForEndCoroutine != null);
                }
            }

            public void Reset()
            {
                sfxClip = null;
                audioClip = null;
                checkForEndCoroutine = null;
                endCallback = null;
            }

            public SFXInstance (AudioSource audioSource)
            {
                AssignedAudioSource = audioSource;
            }
        }
    }
}


