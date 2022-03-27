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
        //这里使用了QuarkLoader替换了默认的资源加载器
        CosmosEntry.ResourceManager.AddOrUpdateBuildInLoadHelper(Cosmos.Resource.ResourceLoadMode.Resource, new QuarkLoader());
        CosmosEntry.AudioManager.AudioRegisterSuccess += AudioRegisterSuccess;
        CosmosEntry.AudioManager.AudioRegistFailure += AudioRegistFailure; ;
        var audioAssetInfo = new AudioAssetInfo("AudioTechHouse", "AudioTechHouse");
        CosmosEntry.AudioManager.RegistAudio(audioAssetInfo);
    }
    void AudioRegisterSuccess(AudioRegistSuccessEventArgs eventArgs)
    {
        Utility.Debug.LogInfo($" {eventArgs.AudioName} Register success", MessageColor.GREEN);
    }
    void AudioRegistFailure(AudioRegistFailureEventArgs eventArgs)
    {
        Utility.Debug.LogError($" {eventArgs.AudioName} Register Failure");
    }
    void PlayAudio()
    {
        var ap = AudioParams.Default;
        ap.Loop = true;
        CosmosEntry.AudioManager.PalyAudio("AudioTechHouse", ap, AudioPlayInfo.Default);
        Utility.Debug.LogInfo("PlayAudio");
    }
    void PauseAudio()
    {
        CosmosEntry.AudioManager.PauseAudio("AudioTechHouse");
        Utility.Debug.LogInfo("PuaseAudio");
    }
    void UnpauseAudio()
    {
        CosmosEntry.AudioManager.UnPauseAudio("AudioTechHouse");
        Utility.Debug.LogInfo("UnpauseAudio");
    }
    void StopAudio()
    {
        CosmosEntry.AudioManager.StopAudio("AudioTechHouse");
        Utility.Debug.LogInfo("StopAudio");
    }
}
