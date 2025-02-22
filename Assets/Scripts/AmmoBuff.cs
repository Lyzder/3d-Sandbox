using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoBuff : MonoBehaviour
{
    private PlayerController playerController;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")){
            playerController = other.gameObject.GetComponent<PlayerController>();
            playerController.SetBuff();
            Destroy(gameObject);
        }
    }
}
