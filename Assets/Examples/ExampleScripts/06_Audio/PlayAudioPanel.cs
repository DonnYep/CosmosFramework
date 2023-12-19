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
    [SerializeField] [Range(0,10)]float fadeTime;
    [SerializeField] [Range(0, 1)] float volume=1;
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
        CosmosEntry.AudioManager.AudioRegisterSuccess += AudioRegisterSuccess;
        CosmosEntry.AudioManager.AudioRegisterFailure += AudioRegistFailure; ;
        var audioAssetInfo = new AudioAssetInfo("AudioTechHouse", "AudioTechHouse");
        CosmosEntry.AudioManager.RegisterAudioAsync(audioAssetInfo);
    }
    void AudioRegisterSuccess(AudioRegisterSuccessEventArgs eventArgs)
    {
        Utility.Debug.LogInfo($" {eventArgs.AudioName} Register success", DebugColor.green);
    }
    void AudioRegistFailure(AudioRegisterFailureEventArgs eventArgs)
    {
        Utility.Debug.LogError($" {eventArgs.AudioName} Register Failure");
    }
    void PlayAudio()
    {
        var ap = AudioParams.Default;
        ap.Loop = true;
        ap.FadeInTime= fadeTime;
        ap.Volume = volume;
        CosmosEntry.AudioManager.PlayAudio("AudioTechHouse", ap, AudioPlayInfo.Default);
        Utility.Debug.LogInfo("PlayAudio");
    }
    void PauseAudio()
    {
        CosmosEntry.AudioManager.PauseAudio("AudioTechHouse", fadeTime);
        Utility.Debug.LogInfo("PuaseAudio");
    }
    void UnpauseAudio()
    {
        CosmosEntry.AudioManager.UnpauseAudio("AudioTechHouse", fadeTime);
        Utility.Debug.LogInfo("UnpauseAudio");
    }
    void StopAudio()
    {
        CosmosEntry.AudioManager.StopAudio("AudioTechHouse", fadeTime);
        Utility.Debug.LogInfo("StopAudio");
    }
}
