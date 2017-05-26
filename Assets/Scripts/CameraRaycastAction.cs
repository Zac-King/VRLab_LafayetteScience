using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class CameraRaycastAction : MonoBehaviour
{
    private Transform m_mainCamera;
    private RaycastHit m_rayHit;

    [SerializeField]
    private UnityEngine.UI.Image m_fillImage;

    [SerializeField]
    private float m_loadTime;

    [SerializeField]
    UnityEngine.Events.UnityEvent m_onLoad;

    void Start()
    {
        m_mainCamera = Camera.main.transform;
    }
	
	void Update ()
    {
        Physics.Raycast(m_mainCamera.position, m_mainCamera.forward, out m_rayHit);

        if(m_rayHit.collider.gameObject == gameObject)
        {
            m_fillImage.fillAmount += Time.deltaTime / m_loadTime;
            if(m_fillImage.fillAmount >= 1f)
            {
                m_onLoad.Invoke();
            }
        }

        else if(m_fillImage.fillAmount > 0)
        {
            m_fillImage.fillAmount -= Time.deltaTime;
        }
	}
}
