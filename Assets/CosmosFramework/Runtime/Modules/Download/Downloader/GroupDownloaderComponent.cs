using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


// mono-behavior for downloading files, just set 'PendingURLS' in editor
// invoke 'Download' or set 'DownloadOnStart' to true
// some fields are provided for editor-proxy access to GroupDownloader object
class GroupDownloaderComponent : MonoBehaviour
{

    [Serializable]
    public struct DownloadStruct
    {
        public string URI;
        public string Filename;
    }


    private GroupDownloader _downloader;

    public GroupDownloader Downloader
    {
        get
        {
            return _downloader;
        }
    }

    // true if 'Download' should be invoked upon 'Start'
    [SerializeField]
    private bool _downloadOnStart = true;

    [SerializeField]
    private string _downloadPath;

    public string DownloadPath;

    // true if the download handler should complete on failure
    [SerializeField]
    private bool _abandonOnFailure = false;

    [SerializeField]
    private List<string> _pendingUrls = new List<string>();

    [SerializeField]
    public DownloadStruct[] _uriToFilenames;

    public List<string> PendingURLS
    {
        get
        {
            return _pendingUrls;
        }
    }

    // init GroupDownloader and invoke Download if enabled
    void Start()
    {
        _downloadPath = Application.persistentDataPath;
        _downloader = new GroupDownloader(PendingURLS);
        Dictionary<string, string> URIToFilenameMap = new Dictionary<string, string>();
        foreach (var ds in _uriToFilenames)
        {
            URIToFilenameMap.Add(ds.URI, ds.Filename);
        }
        _downloader.URIFilenameMap = URIToFilenameMap;
        if (_downloadOnStart && _downloader != null)
        {
            _downloader.Download();
        }
    }

    // update downloader with internal values
    void Update()
    {
        if (_downloader == null || _downloadPath == null) return;
        _downloader.DownloadPath = _downloadPath;
        _downloader.AbandonOnFailure = _abandonOnFailure;
    }
}
