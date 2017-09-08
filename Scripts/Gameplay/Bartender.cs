using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class Bartender : NetworkBehaviour
{
    public void OnTriggerEnter(Collider other)
    {
        GameObject gObject = other.gameObject;
        if (gObject.tag == "Player")
        {
            PlayerBeer playerBeer = gObject.GetComponent<PlayerBeer>();
            if(playerBeer.HasToGetMoney())
            {
                playerBeer.SetHasToGetMoneyServer(false);
                if(GameManager.IsMulti())
                {
                    RoundManager.IncreaseTokens(gObject.GetComponent<CharacterManager>());
                }
                else if(GameManager.IsSolo())
                {
                    SoloManager.IncreasePlayerScore();
                }
            }
        }
    }
}
