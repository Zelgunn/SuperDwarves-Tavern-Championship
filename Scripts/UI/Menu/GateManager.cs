using UnityEngine;
using System.Collections;

public class GateManager : MonoBehaviour
{
    static float s_gateSpeed = 10000;

    [SerializeField] int m_direction;
    private Rigidbody m_rigidbody;
    private WaitForSeconds m_closeTimer;
    private bool m_closed = true;
    private bool m_closing = false;

    public void Start()
    {
        m_rigidbody = GetComponent<Rigidbody>();
        m_closeTimer = new WaitForSeconds(0.5f);
    }

    public IEnumerator CloseGate()
    {
        m_closing = true;
        m_rigidbody.AddForce(m_direction * s_gateSpeed, 0, 0);

        while (!m_closed)
        {
            yield return null;
        }

        yield return m_closeTimer;
    }

    public IEnumerator OpenGate()
    {
        m_rigidbody.AddForce(-m_direction * s_gateSpeed * 2, 0, 0);

        while (m_closed)
        {
            yield return null;
        }
    }

    public void OnCollisionEnter(Collision other)
    {
        if (m_closing)
        {
            m_closed = true;
            m_closing = false;
        }
        else
            m_closed = false;
    }
}