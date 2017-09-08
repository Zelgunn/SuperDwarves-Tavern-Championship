using UnityEngine;
using System.Collections;

public class LavaFloor : MonoBehaviour
{
    public void OnTriggerEnter(Collider other)
    {
        GameObject gObject = other.gameObject;
        if (gObject.tag == "Player")
        {
            CharacterManager player = gObject.GetComponent<CharacterManager>();
            player.Respawn();
            player.DisableControl(true);

            StartCoroutine(FreeDeadCharacter(player));
        }
    }

    private IEnumerator FreeDeadCharacter(CharacterManager player)
    {
        int blink = 0;
        bool enableRenderers = false;

        while(blink < 10)
        {
            blink++;
            if(blink <= 5)
                yield return new WaitForSeconds(0.5f);
            else
                yield return new WaitForSeconds(0.25f);

            player.EnableRenderers(enableRenderers);
            enableRenderers = !enableRenderers;
        }
        
        player.EnableControl(true);
        player.EnableRenderers();
    }
}
