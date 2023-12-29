using UnityEngine;
using Cosmos;
using Cosmos.Audio;
using UnityEngine.UI;
public class PlayAudioPanel : MonoBehaviour
{
    Button btnPlay;
    Button btnPause;
    Button btnUnPause;
    Button btnStop;
    [SerializeField] [Range(0, 10)] float fadeTime;
    [SerializeField] [Range(0, 1)] float volume = 1;
    int audioSerialId;
    void Awake()
    {
        btnPlay = gameObject.GetComponentInChildren<Button>("Play");
        btnPause = gameObject.GetComponentInChildren<Button>("Pause");
        btnUnPause = gameObject.GetComponentInChildren<Button>("Unpause");
        btnStop = gameObject.GetComponentInChildren<Button>("Stop");

        btnPlay.onClick.AddListener(PlayAudio);
        btnPause.onClick.AddListener(PauseAudio);
        btnStop.onClick.AddListener(StopAudio);
        btnUnPause.onClick.AddListener(UnpauseAudio);
    }
    void Start()
    {
        CosmosEntry.AudioManager.AudioAssetRegisterSuccess += AudioRegisterSuccess;
        CosmosEntry.AudioManager.AudioAssetRegisterFailure += AudioRegistFailure; ;
        CosmosEntry.AudioManager.RegisterAudioAsset("AudioTechHouse");
    }
    void AudioRegisterSuccess(AudioRegisterSuccessEventArgs eventArgs)
    {
        Utility.Debug.LogInfo($" {eventArgs.AudioAssetName} Register success", DebugColor.green);
    }
    void AudioRegistFailure(AudioRegisterFailureEventArgs eventArgs)
    {
        Utility.Debug.LogError($" {eventArgs.AudioAssetName} Register Failure");
    }
    void PlayAudio()
    {
        var ap = AudioParams.Default;
        ap.Loop = true;
        ap.FadeInSeconds = fadeTime;
        ap.Volume = volume;
        var has = CosmosEntry.AudioManager.HasAudio(audioSerialId);
        if (!has)
        {

            //CosmosEntry.AudioManager.PlayAudio(audioSerialId, ap, AudioPositionParams.Default);
        }
        else
        {
            audioSerialId = CosmosEntry.AudioManager.PlayAudio("AudioTechHouse", ap, AudioPositionParams.Default);
        }
        Utility.Debug.LogInfo("PlayAudio");
    }
    void PauseAudio()
    {
        CosmosEntry.AudioManager.PauseAudio(audioSerialId, fadeTime);
        Utility.Debug.LogInfo("PuaseAudio");
    }
    void UnpauseAudio()
    {
        CosmosEntry.AudioManager.ResumeAudio(audioSerialId, fadeTime);
        Utility.Debug.LogInfo("UnpauseAudio");
    }
    void StopAudio()
    {
        CosmosEntry.AudioManager.StopAudio(audioSerialId, fadeTime);
        Utility.Debug.LogInfo("StopAudio");
    }
}
