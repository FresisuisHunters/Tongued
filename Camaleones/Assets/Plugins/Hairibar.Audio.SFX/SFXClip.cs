using Random = System.Random;
using UnityEngine;

#pragma warning disable 649
namespace Hairibar.Audio.SFX
{
    [CreateAssetMenu(fileName ="sfx_NewSFXClip", menuName ="Audio/SFX Clip")]
    public class SFXClip : ScriptableObject
    {
        public const string NO_DUPLICATE_GROUP = "";

        public static Random rnd = new Random();

        private bool TMP_hasBeenWarnedEmpty = false;

        #region Inspector
        [Header("Clips")]
        [SerializeField]
        private AudioClip[] audioClips;

        [Header("Parameters")]
        [SerializeField]
        [Range(0, 1)]
        private float _volume = 1.0f;
        [SerializeField]
        [Range(0, 0.3f)]
        private float _pitchVariance = 0.1f;

        [Header("Clip Collision")]
        [SerializeField]
        private SFXPriority _priority;
        [SerializeField]
        private string duplicateGroup = NO_DUPLICATE_GROUP;
        [SerializeField]
        internal DuplicateInstanceBehaviour duplicateBehaviour = DuplicateInstanceBehaviour.StopOld;
        #endregion

        /// <summary>
        /// Returns true if both clips are the same clip, or if they are in the same duplicate group.
        /// </summary>
        public static bool AreDuplicates(SFXClip a, SFXClip b)
        {
            if (a == null || b == null) return false;

            if (a == b) return true;

            return (a.duplicateGroup == b.duplicateGroup);
        }

        #region Getters
        public SFXPriority Priority { get { return _priority; } }
        public float Volume { get { return _volume; } }
        
        public AudioClip RandomAudioClip {
            get {
                if (audioClips.Length == 0)
                {
                    if (!TMP_hasBeenWarnedEmpty)
                    {
                        Debug.LogWarning("SFX clip " + name + " has no assigned audio clips.");
                        TMP_hasBeenWarnedEmpty = true;
                    }

                    return EmptyAudioClip;
                }
                
                return audioClips[rnd.Next(audioClips.Length)];
            }
        }
        public float RandomPitch { get { return (float) (1 + (rnd.NextDouble() * (_pitchVariance * 2) - _pitchVariance)); } }
        #endregion

        #region Empty Failsafe
        private static AudioClip EmptyAudioClip
        {
            get
            {
                if (!_emptyAudioClip)
                {
                    _emptyAudioClip = AudioClip.Create("EmptyAudioClip", AudioSettings.outputSampleRate, 1, AudioSettings.outputSampleRate, false);
                }
                return _emptyAudioClip;
            }
        }
        private static AudioClip _emptyAudioClip;
        #endregion

        private void OnEnable()
        {
            TMP_hasBeenWarnedEmpty = false;
        }

        private void OnValidate()
        {
            duplicateGroup = duplicateGroup.Trim();
            duplicateGroup = duplicateGroup.ToUpper();

            duplicateGroup = duplicateGroup.Replace(' ', '_');

        }

        internal enum DuplicateInstanceBehaviour
        {
            Allow,
            StopOld,
            SkipNew
        }
    }
}

