using UnityEngine;
using System.Collections;

public class BearHead : MonoBehaviour
{
    private Animator m_animator;
    private int m_hashLeft = Animator.StringToHash("Left");
    private int m_hashRight = Animator.StringToHash("Right");

	public void Start ()
    {
        m_animator = GetComponent<Animator>();
	}

    public void Update()
    {
        float i = Random.Range(0f, 1f);
        if(i < (1.0f / 60.0f * Time.deltaTime))
        {
            m_animator.SetBool(m_hashLeft, true);
        }
        else if (i < (2.0f / 60.0f * Time.deltaTime))
        {
            m_animator.SetBool(m_hashRight, true);
        }
        else
        {
            m_animator.SetBool(m_hashLeft, false);
            m_animator.SetBool(m_hashRight, false);
        }
	}
}
