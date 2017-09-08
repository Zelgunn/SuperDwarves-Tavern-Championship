using UnityEngine;
using System.Collections;

public class Drinker : MonoBehaviour
{
    [Header("Mouvement")]
    [SerializeField] private float m_maximumBalancing = 30.0f;
    [SerializeField] private float m_balancingSpeed = 1.0f;
    private float m_currentBalancing = 0;
    private float m_balancingSpeedModifier = 1.0f;
    private float m_randomBalanceDelta = 1.0f;

    private bool m_drinking = false;

	public void Start ()
    {
        if (Random.Range(0.0f, 1.0f) > 0.5) m_balancingSpeed *= -1;
        SetDrinking(false);
	}
	
    public void Update()
    {
	    if(m_drinking)
        {
            float balanceDelta = m_balancingSpeed * m_balancingSpeedModifier * m_randomBalanceDelta;
            transform.eulerAngles += new Vector3(0, 0, balanceDelta);
            m_currentBalancing += balanceDelta;

            if((Mathf.Abs(m_currentBalancing) > m_maximumBalancing))
            {
                m_balancingSpeed = -m_balancingSpeed;
            }
        }
	}

    public void SetDrinking(bool drinking)
    {
        m_drinking = drinking;
        if(drinking)
        {
            m_randomBalanceDelta = Random.Range(0.8f, 1.2f);
        }
    }

    public void SetWaiting(bool waiting)
    {
        SetDrinking(!waiting);
    }

    public void SetSpeedModifier(float speedModifier)
    {
        m_balancingSpeedModifier = speedModifier;
        m_randomBalanceDelta = Random.Range(0.8f, 1.2f);
    }
}
