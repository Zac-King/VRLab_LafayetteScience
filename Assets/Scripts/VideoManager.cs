﻿using UnityEngine;
using UnityEngine.Video;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.IO;

public class VideoManager : MonoBehaviour
{
    public OZOPlayer m_ozoPlayer;
    public PlayControl m_ozoPlayController;

    public UnityEngine.Events.UnityEvent m_onVideoEnd;

    [XmlArray("VideosMetaData"), XmlArrayItem("Meta")]
    public VideoMetas[] m_videoClips = new VideoMetas[8];

    [SerializeField]
    private AudioSource[] m_steroSources = new AudioSource[6];

	void Start ()
    {
        //LoadVideoMetaData();

        //m_videoPlayer.source = VideoSource.Url;
        //SetSurroundSound();
        //m_videoPlayer.SetTargetAudioSource(0, m_videoPlayer.GetComponent<AudioSource>());
	}
	
	void Update ()
    {
        //CheckVideoEnd();
	}

    public void PlayVideoAtIndex(int index)
    {
        m_ozoPlayer.gameObject.SetActive(true);

        //m_videoPlayer.url = Application.streamingAssetsPath + "/" + m_videoClips[index].m_video;
        //m_videoPlayer.Play();
    }

    //public void StopVideo()
    //{
    //    m_videoPlayer.Stop();
    //    m_videoPlayer.gameObject.SetActive(false);
    //    m_onVideoEnd.Invoke();
    //}

    //public void CheckVideoEnd()
    //{
    //    if((ulong)m_videoPlayer.frame == m_videoPlayer.frameCount - 60 
    //        && m_videoPlayer.gameObject.activeInHierarchy)
    //    {
    //        FadeVideoOut();
    //    }
    //}


    //public void SetSurroundSound()
    //{
    //    for (int i = 0; i < m_steroSources.Length; ++i)
    //    {
    //        m_videoPlayer.SetTargetAudioSource((ushort)i, m_steroSources[i]);
    //    }
    //}


    //public IEnumerator _FadeIn()
    //{
    //    Material mat = m_fadeSphere.GetComponent<Renderer>().material;
    //    Color color = mat.color;

    //    while(mat.color.a < 1)
    //    {
    //        color.a += Time.deltaTime;
    //        mat.color = color;
    //        yield return false;
    //    }

    //    yield return true;
    //}

    //public IEnumerator _FadeOut()
    //{
    //    Material mat = m_fadeSphere.GetComponent<Renderer>().material;
    //    Color color = mat.color;

    //    while (mat.color.a > 0)
    //    {
    //        color.a -= Time.deltaTime;
    //        mat.color = color;
    //        yield return false;
    //    }

    //    yield return true;
    //}

    //public void FadeToVideoAtIndex(int index)
    //{
    //    StartCoroutine(_FadeToVideoAtIndex(index));
    //}

    //public void FadeVideoOut()
    //{
    //    StartCoroutine(_FadeVideoOut());
    //}

    //public IEnumerator _FadeToVideoAtIndex(int index)
    //{
    //    yield return StartCoroutine(_FadeIn());

    //    m_videoPlayer.GetTargetAudioSource(0).volume = 0;
    //    PlayVideoAtIndex(index);

    //    yield return new WaitForSeconds(1);
    //    StartCoroutine(_FadeOut());

    //    while (m_videoPlayer.GetTargetAudioSource(0).volume < 1)
    //    {
    //        m_videoPlayer.GetTargetAudioSource(0).volume += Time.deltaTime * 0.5f;
    //        yield return null;
    //    }
    //}

    //public IEnumerator _FadeVideoOut()
    //{
    //    yield return StartCoroutine(_FadeIn());

    //    while (m_videoPlayer.GetTargetAudioSource(0).volume > 0)
    //    {
    //        m_videoPlayer.GetTargetAudioSource(0).volume -= Time.deltaTime * 0.5f;
    //        yield return null;
    //    }

    //    StopVideo();
    //    StartCoroutine(_FadeOut());
    //}




    private void LoadVideoMetaData()
    {
        XmlSerializer serializer = new XmlSerializer(typeof(VideoMetas[]));
        FileStream stream = new FileStream(Path.Combine(Application.streamingAssetsPath, "_VIDEO_METADATA.xml"), FileMode.OpenOrCreate);

        if (stream.Length < 1)
        {
            serializer.Serialize(stream, m_videoClips);
        }

        else
        {
            m_videoClips = serializer.Deserialize(stream) as VideoMetas[];
        }

        stream.Close();
    }
}

[System.Serializable, XmlRoot]
public class VideoMetas
{
    public string m_video;
    //public string m_thumbnail;
    //public string m_description;

    public VideoMetas()
    {
        m_video = " ";
        //m_thumbnail = " ";
        //m_description = " ";
    }
}