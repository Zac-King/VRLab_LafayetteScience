using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowTransform : MonoBehaviour
{
    public Transform m_target;
    public Vector3 m_offset;

	void Update ()
    {
        transform.position = m_target.position + m_offset;
    }
}
