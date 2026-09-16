using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.UI.Image;

public class PutMirror : MonoBehaviour
{
    [SerializeField] GameObject _MirrorObject;
    [SerializeField] Material[] mats;
    GameObject lookedPlace;

    // Update is called once per frame
    void Update()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, 1.5f))
        {
            GameObject targetObject = hit.collider.gameObject;
            if(lookedPlace != null && targetObject != lookedPlace)
            {
                lookedPlace.GetComponent<MeshRenderer>().material = mats[0];
                lookedPlace = null;
            }
            if(targetObject.layer == LayerMask.NameToLayer("MirrorPlace")) lookedPlace = targetObject;
            lookedPlace.GetComponent<MeshRenderer>().material = mats[1];
        }
        else if(lookedPlace != null)
        {
            lookedPlace.GetComponent<MeshRenderer>().material = mats[0];
            lookedPlace = null;
        }

        if(lookedPlace != null && Input.GetMouseButtonDown(0))
        {

        }
    }
}
