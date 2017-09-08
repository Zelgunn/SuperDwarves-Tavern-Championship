using UnityEngine;
using System.Collections;

public class FireLight : MonoBehaviour
{
    private Light m_light;
    [SerializeField]
    private int m_intensityDir = 1;
    [SerializeField]
    private float m_minIntensity = 2.0f;
    [SerializeField]
    private float m_maxIntensity = 3.0f;
    [SerializeField]
    private float m_flickerIntensityPerSec = 0.5f;
    [SerializeField]
    private float m_flickerPerSec = 20;

    private float m_target;

	void Start ()
	{
        m_light = GetComponent<Light>();
        m_light.intensity = m_maxIntensity;

        StartCoroutine(Flicker());
	}

    public IEnumerator Flicker()
    {
        switch(m_intensityDir)
        {
            case 1:
                if (m_light.intensity >= m_target)
                {
                    m_intensityDir = -1;
                    m_target = Random.Range(m_minIntensity, m_target);
                }
                break;
            default:
                if (m_light.intensity <= m_target)
                {
                    m_intensityDir = 1;
                    m_target = Random.Range(m_target, m_maxIntensity);
                }
                break;
        }

        float flicker = Random.Range(0, m_flickerIntensityPerSec * 2) * m_intensityDir / m_flickerPerSec;
        m_light.intensity += flicker;

        if (m_flickerPerSec <= 0) m_flickerPerSec = 1;

        yield return new WaitForSeconds(1/m_flickerPerSec);
        StartCoroutine(Flicker());
    }
}