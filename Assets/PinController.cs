using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PinController : MonoBehaviour
{
    public MapController mapController;
    int pinId { get; set; }
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.tag == "MeshPinColider")
        {
            mapController.OnPinEnterTrigger(this.gameObject, collision.gameObject);
        }
    }

    private void OnTriggerExit(Collider collision)
    {
        if (collision.tag == "MeshPinColider")
        {
            mapController.OnPinExitTrigger(this.gameObject, collision.gameObject);
        }
    }
}

