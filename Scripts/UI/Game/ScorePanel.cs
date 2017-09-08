using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ScorePanel : MonoBehaviour
{
    [SerializeField] private RawImage m_scoreImage;
    private Animator m_animator;
    private int m_hashIsUp = Animator.StringToHash("isUp");

    public void Start()
    {
        m_animator = GetComponent<Animator>();
    }

    public void SetIsUp(bool value)
    {
        m_animator.SetBool(m_hashIsUp, value);
    }

    private IEnumerator SetNumberCoroutine(Texture number)
    {
        SetIsUp(true);
        yield return new WaitForSeconds(1.25f);
        m_scoreImage.texture = number;
        SetIsUp(false);
    }

    public void SetNumber(int number)
    {
        if (FontHandler.Numbers()[number] == m_scoreImage.texture)
            return;
        StartCoroutine(SetNumberCoroutine(FontHandler.Numbers()[number]));
    }
}
