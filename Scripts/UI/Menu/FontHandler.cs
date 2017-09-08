using UnityEngine;
using System.Collections;

public class FontHandler : MonoBehaviour
{
    [SerializeField] private Texture[] m_numbers;

    static private FontHandler s_singleton;

    public void Start()
    {
        s_singleton = this;
    }

    static public Texture[] Numbers()
    {
        return s_singleton.m_numbers;
    }

    static public Texture Number(int number)
    {
        return s_singleton.m_numbers[number];
    }
}
