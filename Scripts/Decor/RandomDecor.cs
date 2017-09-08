using UnityEngine;
using System.Collections;

public class RandomDecor : MonoBehaviour
{
    [SerializeField] private float m_randomChance = 0.5f;
	public void Start ()
    {
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(Random.Range(0f, 1f) > m_randomChance);
        }
	}

}
