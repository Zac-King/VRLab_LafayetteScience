using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowTransform : MonoBehaviour
{
    public Transform m_target;
	
	void Update ()
    {
        transform.position = m_target.position;
    }
}
