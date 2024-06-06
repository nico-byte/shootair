using System.Collections.Generic;
using UnityEngine;

namespace ShootAirRLAgent
{
    public class SoundEffectPlayer : MonoBehaviour
    {
        AgentSettings agentSettings;

        Dictionary<string, AudioSource> audioSourcesDict = new Dictionary<string, AudioSource> {};

        private void Start()
        {
            AudioSource[] audioSources = GetComponents<AudioSource>();
            for (int i = 0; i < audioSources.Length; i++)
            {
                audioSourcesDict[audioSources[i].clip.name] = audioSources[i];
            }
            agentSettings = FindObjectOfType<AgentSettings>();
        }

        public void playSound(string soundName, float pitchOffset = 0f, float volume = 1f)
        {
            if (agentSettings.selfplay) {
                float pitchRandom = pitchOffset == 0f ? 0f : UnityEngine.Random.Range(-pitchOffset, pitchOffset);
                GameObject duplicateObj = new GameObject("Dupe_"+soundName);
                AudioSource duplicateAudioSource = duplicateObj.AddComponent<AudioSource>();

                duplicateAudioSource.clip = audioSourcesDict[soundName].clip;
                duplicateAudioSource.pitch += pitchRandom;
                duplicateAudioSource.volume = volume;
                
                duplicateAudioSource.Play();
                Destroy(duplicateObj,duplicateAudioSource.clip.length);
            }
        }
    }
}