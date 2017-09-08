using UnityEngine;
using System.Collections;

public class BeerReserve : MonoBehaviour
{
    public void OnTriggerEnter(Collider other)
    {
        GameObject gObject = other.gameObject;
        if (gObject.tag == "Player")
        {
            PlayerBeer playerBeer = gObject.GetComponent<PlayerBeer>();
            if (playerBeer.IsEmpty() && !playerBeer.HasToGetMoney())
            {
                playerBeer.RefillBeer();
            }
        }
    }
}
