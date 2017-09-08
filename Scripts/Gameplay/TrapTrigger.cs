using UnityEngine;
using System.Collections;

public class TrapTrigger : MonoBehaviour
{
    public void OnTriggerEnter(Collider other)
    {
        GameObject gObject = other.gameObject;
        if (gObject.tag == "Player")
        {
            PlayerNetworkSkills playerNetworkSkills = gObject.GetComponent<PlayerNetworkSkills>();
            playerNetworkSkills.SetCloseToTriggerTrap(true);
        }
    }

    public void OnTriggerExit(Collider other)
    {
        GameObject gObject = other.gameObject;
        if (gObject.tag == "Player")
        {
            PlayerNetworkSkills playerNetworkSkills = gObject.GetComponent<PlayerNetworkSkills>();
            playerNetworkSkills.SetCloseToTriggerTrap(false);
        }
    }
}
