using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CentralPanel : MonoBehaviour
{
    private Animator m_animator;
    private int m_hashIsVisible = Animator.StringToHash("isVisible");

    private RawImage m_image;

    [SerializeField] private GameObject m_exitButton;
    [Header("Images Victoire/Défaite")]
    [SerializeField] private Texture m_victoryTexture;
    [SerializeField] private Texture m_defeatTexture;

	public void Start ()
    {
        m_animator = GetComponent<Animator>();
        m_image = GetComponent<RawImage>();
        m_exitButton.SetActive(false);
	}

    public void ShowMenu(bool show, bool showExit = false)
    {
        m_animator.SetBool(m_hashIsVisible, show);
        m_exitButton.SetActive(showExit);
    }

    public void SetVictorious(bool victorious)
    {
        if (victorious)
            m_image.texture = m_victoryTexture;
        else
            m_image.texture = m_defeatTexture;
    }
}
