using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OZOVideoManager : MonoBehaviour
{
    public OZOPlayer m_ozoViewer;
    public PlayControl m_ozoPlayerControler;

    public string m_licenseID = "";

    public string[] m_videos;

    private int m_currentVideoIndex = -1;
    private IOZOPlayer _ozoViewer = null;


	void Awake ()
    {
        _ozoViewer = m_ozoViewer;
        _ozoViewer.Init(m_licenseID, false, false, false);
	}
	

	void Update ()
    {
        if (CheckVideoEnd())
            StopVideo();
	}


    public void PlayVideoAtIndex(int index)
    {
        if (index < 0 || index > m_videos.Length) return;

        else m_currentVideoIndex = index;

        m_ozoViewer.gameObject.SetActive(true);

        string videoPath = Application.streamingAssetsPath + "/" + m_videos[m_currentVideoIndex];
        _ozoViewer.LoadVideo(videoPath);

        _ozoViewer.PlayLoaded();
    }

    public void StopVideo()
    {
        _ozoViewer.Stop();
        m_ozoViewer.gameObject.SetActive(false);
    }

    public bool CheckVideoEnd()
    {
        return _ozoViewer.ElapsedTime() >= _ozoViewer.Duration();
    }
}
