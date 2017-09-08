using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using System;

public class NumericDisplay : MonoBehaviour
{
    [SerializeField] private RawImage m_base;
    private List<RawImage> m_images;

    public void SetValue(int value)
    {
        int digitCount = 1;
        int tmp = value;

        while(tmp >= 10)
        {
            tmp /= 10;
            digitCount++;
        }

        if(m_images != null)
        {
            foreach (RawImage image in m_images)
            {
                if (image == m_base)
                    continue;

                GameObject.Destroy(image.gameObject);
            }
        }

        m_images = new List<RawImage>();
        m_images.Add(m_base);

        for (int i = 1; i < digitCount; i++)
        {
            RawImage image = GameObject.Instantiate(m_base);

            Vector3 pos = m_base.transform.position;
            pos.x += i * 30 * m_base.transform.lossyScale.x;
            image.transform.position = pos;

            image.transform.SetParent(transform);
            image.transform.localScale = m_base.transform.localScale;

            m_images.Add(image);
        }

        tmp = value;
        for(int i = digitCount - 1; i >= 0 ; i--)
        {
            m_images[i].texture = FontHandler.Number(tmp%10);
            tmp /= 10;
        }
    }
}
