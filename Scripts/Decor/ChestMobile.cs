using UnityEngine;
using System.Collections;

public class ChestMobile : MonoBehaviour
{
    private float m_angleDelta = 0;
    private bool m_opening = false;
    private bool m_closing = false;

    public void Start()
    {
        transform.localEulerAngles = new Vector3(295, 0, 0);
    }
    
    public IEnumerator OpenChestCoroutine()
    {
        m_opening = true;
        m_closing = false;

        while ((m_angleDelta < 65) && m_opening)
        {
            m_angleDelta += Time.deltaTime * 120;
            transform.localEulerAngles = new Vector3(295 + m_angleDelta, 0, 0);
            yield return null;
        }

        m_opening = false;
    }

    public IEnumerator CloseChestCoroutine()
    {
        m_opening = false;
        m_closing = true;

        while ((m_angleDelta > 0) && m_closing)
        {
            m_angleDelta -= Time.deltaTime * 120;
            transform.localEulerAngles = new Vector3(295 + m_angleDelta, 0, 0);
            yield return null;
        }

        m_closing = false;
    }
}
