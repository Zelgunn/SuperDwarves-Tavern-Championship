using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class IngameMenu : MonoBehaviour
{
    [SerializeField] private Button m_exitButton;

    private bool m_menuShown = false;

	public void Start ()
    {
	
	}

    public void Update ()
    {
	    if(Input.GetButtonDown("Cancel"))
        {
            ShowMenu(!m_menuShown);
        }
	}

    public void ShowMenu(bool showMenu)
    {
        m_menuShown = showMenu;

        m_exitButton.gameObject.SetActive(showMenu);
    }

    public void OnExitButton()
    {
        ShowMenu(false);
        GameManager.Singleton().ExitGame();
    }
}