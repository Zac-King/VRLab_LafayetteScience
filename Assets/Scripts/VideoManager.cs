using UnityEngine;
using UnityEngine.Video;
using System.Collections;
using System.Collections.Generic;

public class VideoManager : MonoBehaviour
{
    public VideoPlayer m_videoPlayer;
    public GameObject m_fadeSphere;

    public List<string> m_videoClips = new List<string>();

	void Start ()
    {
        m_videoPlayer.source = VideoSource.Url;
        m_videoPlayer.SetTargetAudioSource(0, m_videoPlayer.GetComponent<AudioSource>());
	}
	
	void Update ()
    {
        CheckVideoEnd();
	}

    public void PlayVideoAtIndex(int index)
    {
        m_videoPlayer.gameObject.SetActive(true);
        m_videoPlayer.url = Application.streamingAssetsPath + "/" +m_videoClips[index];
        m_videoPlayer.Play();
    }

    public void StopVideo()
    {
        m_videoPlayer.Stop();
        m_videoPlayer.gameObject.SetActive(false);
    }

    public void CheckVideoEnd()
    {
        if((ulong)m_videoPlayer.frame == m_videoPlayer.frameCount - 60 
            && m_videoPlayer.gameObject.activeInHierarchy)
        {
            FadeVideoOut();
            //StopVideo();
        }
    }

    public IEnumerator _FadeIn()
    {
        Material mat = m_fadeSphere.GetComponent<Renderer>().material;
        Color color = mat.color;

        while(mat.color.a < 1)
        {
            color.a += Time.deltaTime;
            mat.color = color;
            yield return false;
        }

        yield return true;
    }

    public IEnumerator _FadeOut()
    {
        Material mat = m_fadeSphere.GetComponent<Renderer>().material;
        Color color = mat.color;

        while (mat.color.a > 0)
        {
            color.a -= Time.deltaTime;
            mat.color = color;
            yield return false;
        }

        yield return true;
    }

    public void FadeToVideoAtIndex(int index)
    {
        StartCoroutine(_FadeToVideoAtIndex(index));
    }

    public void FadeVideoOut()
    {
        StartCoroutine(_FadeVideoOut());
    }

    public IEnumerator _FadeToVideoAtIndex(int index)
    {
        yield return StartCoroutine(_FadeIn());
        PlayVideoAtIndex(index);
        yield return new WaitForSeconds(1);
        yield return StartCoroutine(_FadeOut());
    }

    public IEnumerator _FadeVideoOut()
    {
        yield return StartCoroutine(_FadeIn());
        StopVideo();
        yield return new WaitForSeconds(1);
        yield return StartCoroutine(_FadeOut());
    }
}