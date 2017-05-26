using UnityEngine;
using Valve.VR.InteractionSystem;

public class RaySelector : MonoBehaviour
{
    [SerializeField] LineRenderer m_line;
    [SerializeField] LayerMask m_mask;
    [SerializeField] private float m_range = 500;
    [SerializeField] private Hand m_hand;

    private RaycastHit m_rayCursor;
    private GameObject m_selectedGameobject;

    private void Start()
    {
        m_hand = GetHandFromParent(gameObject);
    }

    private void Update()
    {
        if (Physics.Raycast(transform.position, transform.forward, out m_rayCursor, m_range, m_mask))
        {
            m_line.SetPosition(0, gameObject.transform.position);
            m_line.SetPosition(1, m_rayCursor.point);


            GameObject g = m_rayCursor.collider.gameObject;

            if (g.GetComponent<MenuItemSelectable>() != null)
            {
                g.GetComponent<MenuItemSelectable>().RayOn();
                m_line.SetColors(Color.blue, Color.blue);

                if(m_selectedGameobject != g)
                {
                    if (m_selectedGameobject != null)
                        m_selectedGameobject.GetComponent<MenuItemSelectable>().RayOff();
                    m_selectedGameobject = g;
                }
            }
            else
            {
                if(m_selectedGameobject != null)
                    m_selectedGameobject.GetComponent<MenuItemSelectable>().RayOff();

                m_line.SetColors(Color.green, Color.green);
                m_selectedGameobject = null;
            }

        }

        if(m_hand.controller.GetHairTriggerDown() && m_selectedGameobject != null)
        {
            m_selectedGameobject.GetComponent<MenuItemSelectable>().ItemSelect();
        }

    }

    private Hand GetHandFromParent(GameObject child)
    {
        if (child.transform.parent == null) return null;
        else if (child.GetComponentInParent<Hand>() == null) return GetHandFromParent(child.transform.parent.gameObject);
        else return child.GetComponentInParent<Hand>();
    }

}
