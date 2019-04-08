using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SharkShop : MonoBehaviour
{
    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Player")
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                Player player = other.GetComponent<Player>();
                if(player != null)
                {
                    if(player.hasCoin == true)
                    {
                        player.hasCoin = false;
                        UIManager uiManager = GameObject.Find("Canvas").GetComponent<UIManager>();
                        if(uiManager != null)
                        {
                            uiManager.RemoveCoin();
                        }

                        AudioSource audio = GetComponent<AudioSource>();
                        audio.Play();
                        player.EnableWeapon();
                    }
                    else
                    {
                        Debug.Log("Get Out Of Here");
                    }
                }
            }
        }
    }
}
