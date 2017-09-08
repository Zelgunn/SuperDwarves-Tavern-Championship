using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class TrapManager : NetworkBehaviour
{
    public enum TrapState
    {
        Armed,
        Released,
        Waiting,
        Rearming
    };

    [SerializeField] private GameObject m_leftTrap;
    [SerializeField] private GameObject m_rightTrap;
    [SerializeField] private GameObject m_rightTrigger;
    [SerializeField] private GameObject m_leftTrigger;

    private Animator m_leftLeverAnimator;
    private Animator m_rightLeverAnimator;
    private int m_hashIsUp = Animator.StringToHash("isUp");

    private float m_trapsAcceleration = 0.05f;
    private float m_trapsSpeed = 0;
    private float m_currentAngle = 0;
    private bool m_waiting;

    static private TrapManager s_singleton;
    static private TrapState s_trapsState = TrapState.Armed;

	public void Start ()
    {
        s_singleton = this;
        m_leftLeverAnimator = m_leftTrigger.GetComponent<Animator>();
        m_rightLeverAnimator = m_rightTrigger.GetComponent<Animator>();
	}

    public void Update()
    {
        switch(s_trapsState)
        {
            case TrapState.Released:
                if (m_currentAngle >= 59.9)
                {
                    s_trapsState = TrapState.Waiting;
                    StartCoroutine(WaitBeforeRearming());
                }
                else
                {
                    m_trapsSpeed += m_trapsAcceleration;
                    m_currentAngle = Mathf.Min(m_currentAngle + m_trapsSpeed, 60);

                    Vector3 angle = new Vector3(-90 + m_currentAngle, 0, 0);
                    m_leftTrap.transform.localEulerAngles = angle;
                    m_rightTrap.transform.localEulerAngles = angle;
                    
                }
                break;
            case TrapState.Rearming:
                if (m_currentAngle <= 0.1)
                {
                    m_trapsSpeed = 0;
                    s_trapsState = TrapState.Armed;
                }
                else
                {
                    m_currentAngle = Mathf.Max(m_currentAngle - 1, 0);

                    Vector3 angle = new Vector3(-90 + m_currentAngle, 0, 0);
                    m_leftTrap.transform.localEulerAngles = angle;
                    m_rightTrap.transform.localEulerAngles = angle;
                }
                break;
        }

        m_leftLeverAnimator.SetBool(m_hashIsUp, s_trapsState == TrapState.Armed);
        m_rightLeverAnimator.SetBool(m_hashIsUp, s_trapsState == TrapState.Armed);
    }

    private IEnumerator WaitBeforeRearming()
    {
        yield return new WaitForSeconds(10);

        s_trapsState = TrapState.Rearming;
        
    }

    [Command]
    private void CmdActivateTraps()
    {
        if (ActivateTraps())
        {
            RpcActivateTraps();
        }
    }

    [ClientRpc]
    private void RpcActivateTraps()
    {
        ActivateTraps();
    }

    private bool ActivateTraps()
    {
        if (s_trapsState == TrapState.Armed)
        {
            s_trapsState = TrapState.Released;
            return true;
        }
        else
            return false;
    }

    static public void TriggerTraps()
    {
        if (s_trapsState == TrapState.Armed)
        {
            if(s_singleton.isServer)
            {
                s_singleton.RpcActivateTraps();
                s_singleton.ActivateTraps();
            }
            else
            {
                s_singleton.CmdActivateTraps();
            }
        }
    }

    private IEnumerator LowerLever()
    {
        yield return null;
    }
}
