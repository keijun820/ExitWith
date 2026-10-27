using UnityEngine;

public class PutMirror : MonoBehaviour
{
    [SerializeField] Transform cam;
    MirrorPlace lookingMirrorPlace;
    bool mirrorPlaced;

    void Start()
    {
        mirrorPlaced = false;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    // Update is called once per frame
    void Update()
    {
        if (mirrorPlaced) rayMirror();
        else rayMirrorPlace();
    }

    void rayMirrorPlace()
    {
        RaycastHit hit;
        if (Physics.Raycast(cam.position, cam.forward, out hit, 2.5f))
        {
            if (hit.collider.gameObject.layer != LayerMask.NameToLayer("MirrorPlace"))
            {
                if (lookingMirrorPlace != null) lookingMirrorPlace.noLook();
                lookingMirrorPlace = null;
                return;
            }
            lookingMirrorPlace = hit.collider.gameObject.GetComponent<MirrorPlace>();
            lookingMirrorPlace.isLooking(hit.point);
            if (Input.GetKeyDown(KeyCode.E))
            {
                lookingMirrorPlace.placeMirror();
                mirrorPlaced = true;
            }
        }
        else
        {
            if (lookingMirrorPlace != null) lookingMirrorPlace.noLook();
            lookingMirrorPlace = null;
        }
    }

    void rayMirror()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, 1.5f))
        {
            if (hit.collider.gameObject.layer != LayerMask.NameToLayer("Mirror")) return;
            if (Input.GetKeyDown(KeyCode.E)) lookingMirrorPlace.deleteMirror();
        }
    }

}
