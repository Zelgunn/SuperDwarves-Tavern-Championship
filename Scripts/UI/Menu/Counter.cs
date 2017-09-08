using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Counter : MonoBehaviour
{
    [SerializeField] private Texture[] m_counterImages;
    [SerializeField] private Texture m_goImage;

    private RawImage m_image;
    private int m_currentIndex;

    public void Start()
    {
        m_currentIndex = m_counterImages.Length;
        m_image = GetComponent<RawImage>();
    }

    public void UpdateImage(float time)
    {
        if (time < 0)
            return;

        int index;
        if (time < 0.25f)
        {
            index = -1;
        }
        else
        {
            index = (int)time;
        }

        if (index != m_currentIndex)
        {
            if(m_image == null)
                m_image = GetComponent<RawImage>();

            if (m_image.enabled == false)
                m_image.enabled = true;

            if (index == -1)
            {
                m_image.texture = m_goImage;
            }
            else
            {
                m_image.texture = m_counterImages[index];
            }

            m_currentIndex = index;
        }
    }
    

    public void UpdateImage(int index)
    {
        if (index != m_currentIndex)
        {
            if(m_image == null)
                m_image = GetComponent<RawImage>();

            if (m_image.enabled == false)
                m_image.enabled = true;

            if(index < 0)
            {
                m_image.texture = m_goImage;
            }
            else
            {
                m_image.texture = m_counterImages[index];
            }

            m_currentIndex = index;
        }
    }

    public void TurnOff()
    {
        if (m_image.enabled == true)
            m_image.enabled = false;
    }
}
