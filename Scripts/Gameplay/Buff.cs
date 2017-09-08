using UnityEngine;
using System.Collections;

public class Buff
{
    public enum BuffTypes {Stun, SpeedUp};

    private BuffTypes m_type;
    private float m_totalDuration;
    private float m_durationLeft;

    public Buff(BuffTypes type, float duration)
    {
        m_type = type;
        m_durationLeft = duration;
        m_totalDuration = duration;
    }

    public float UpdateDuration(float time)
    {
        m_durationLeft -= time;
        return m_durationLeft;
    }

    public BuffTypes Type()
    {
        return m_type;
    }

    public float TotalDuration()
    {
        return m_totalDuration;
    }
}
