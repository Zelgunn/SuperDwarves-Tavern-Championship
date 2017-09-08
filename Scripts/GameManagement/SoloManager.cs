using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SoloManager : MonoBehaviour
{
    [SerializeField] private int m_winningRoundScore = 5;
    [Header("UI")]
    
    [SerializeField] private Counter m_counter;
    [SerializeField] private TokenCounter m_tokenCounter;
    [Header("Timer")]
    [SerializeField] private RawImage m_minutesLeft;
    [SerializeField] private RawImage m_minutesRight;
    [SerializeField] private RawImage m_secondsLeft;
    [SerializeField] private RawImage m_secondsRight;

    private ThirstyTable[] m_thirtyTables;
    private float m_startTime;
    private int m_playerScore = 0;

    static private SoloManager s_singleton;

    public void Start()
    {
        s_singleton = this;
    }

    public IEnumerator SoloLoop()
    {
        yield return StartCoroutine(InitSoloLoop());

        yield return StartCoroutine(MainSoloLoop());

        yield return StartCoroutine(EndSoloLoop());
    }

    private IEnumerator InitSoloLoop()
    {
        if (m_thirtyTables == null)
            m_thirtyTables = GameObject.FindObjectsOfType<ThirstyTable>();
        RoundManager.DisablePlayersControl();
        ResetValues();
        ResetTimer();
        RoundManager.RespawnPlayers();
        m_tokenCounter.SetMaxTokenCount(5);

        m_counter.gameObject.SetActive(true);
        for (int i = 4; i >= 0; i--)
        {
            m_counter.UpdateImage(i);
            yield return new WaitForSeconds(1);
        }
        m_counter.gameObject.SetActive(false);
        m_startTime = Time.time;
    }

    private IEnumerator MainSoloLoop()
    {
        RoundManager.EnablePlayersControl();
        while(m_playerScore < m_winningRoundScore)
        {
            UpdateTimer();
            yield return null;
        }
    }

    private IEnumerator EndSoloLoop()
    {
        ProfileInfo.SelectedProfileInfo().SetSoloTime((int)(Time.time - m_startTime));
        RoundManager.DisablePlayersControl();
        yield return null;
    }

    private void ResetTimer()
    {
        m_startTime = Time.time;
        UpdateTimer();
    }

    private void ResetValues()
    {
        m_playerScore = 0;
        foreach (ThirstyTable table in m_thirtyTables)
        {
            table.ResetBeer();
        }
    }

    private void UpdateTimer()
    {
        float elapsedTime = Time.time - m_startTime;
        int elapsedSeconds = ((int)elapsedTime)%60;
        int elapsedMins = (int)elapsedTime/60;

        int minutesLeftIdx = elapsedMins / 10;
        int minutesRightIdx = elapsedMins % 10;
        int secondsLeftIdx = elapsedSeconds / 10;
        int secondsRightIdx = elapsedSeconds % 10;


        m_minutesLeft.texture = FontHandler.Number(minutesLeftIdx);
        m_minutesRight.texture = FontHandler.Number(minutesRightIdx);
        m_secondsLeft.texture = FontHandler.Number(secondsLeftIdx);
        m_secondsRight.texture = FontHandler.Number(secondsRightIdx);
    }

    public void RemoveTokens()
    {
        m_tokenCounter.RemoveTokens();
    }

    static public void IncreasePlayerScore()
    {
        s_singleton.m_playerScore++;
        s_singleton.m_tokenCounter.SetTokenOnCount(s_singleton.m_playerScore);
    }
}
