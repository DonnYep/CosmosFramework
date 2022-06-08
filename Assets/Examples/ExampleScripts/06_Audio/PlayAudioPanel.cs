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
        CosmosEntry.AudioManager.AudioRegistFailure += AudioRegistFailure; ;
        var audioAssetInfo = new AudioAssetInfo("AudioTechHouse", "AudioTechHouse");
        CosmosEntry.AudioManager.RegistAudioAsync(audioAssetInfo);
    }
    void AudioRegisterSuccess(AudioRegistSuccessEventArgs eventArgs)
    {
        Utility.Debug.LogInfo($" {eventArgs.AudioName} Register success", DebugColor.green);
    }
    void AudioRegistFailure(AudioRegistFailureEventArgs eventArgs)
    {
        Utility.Debug.LogError($" {eventArgs.AudioName} Register Failure");
    }
    void PlayAudio()
    {
        var ap = AudioParams.Default;
        ap.Loop = true;
        ap.FadeInTime= fadeTime;
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
        CosmosEntry.AudioManager.UnPauseAudio("AudioTechHouse", fadeTime);
        Utility.Debug.LogInfo("UnpauseAudio");
    }
    void StopAudio()
    {
        CosmosEntry.AudioManager.StopAudio("AudioTechHouse", fadeTime);
        Utility.Debug.LogInfo("StopAudio");
    }
}
