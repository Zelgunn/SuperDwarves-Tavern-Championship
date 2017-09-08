using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TimeNumericDisplay : MonoBehaviour
{
    [SerializeField] private RawImage m_minutesLeft;
    [SerializeField] private RawImage m_minutesRight;
    [SerializeField] private RawImage m_secondsLeft;
    [SerializeField] private RawImage m_secondsRight;

    public void SetTime(int seconds)
    {
        int mins = seconds / 60;
        seconds %= 60;

        m_minutesLeft.texture = FontHandler.Number(mins / 10);
        m_minutesRight.texture = FontHandler.Number(mins % 10);
        m_secondsLeft.texture = FontHandler.Number(seconds / 10);
        m_secondsRight.texture = FontHandler.Number(seconds % 10);
    }
}
