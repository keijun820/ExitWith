using Unity.VisualScripting;
using UnityEngine;

public class MirrorPlace : MonoBehaviour
{
    [SerializeField] GameObject _realMirror;
    [SerializeField] GameObject _fakeMirror;

    [SerializeField] MirrorPlace oppoPlace;
    GameObject fakeMirror;
    GameObject placedMirror;

    [SerializeField] bool axisZ;

    public void isLooking(Vector3 lookPoint, bool stop = false)
    {
        float difference;
        if (axisZ) difference = lookPoint.z - transform.position.z;            
        else difference = lookPoint.x - transform.position.x;

        float radius = transform.localScale.x / 2;
        if (difference > radius - 1) difference = radius - 1;
        else if (difference < -radius + 1) difference = -radius + 1;

        if (fakeMirror == null) fakeMirror = Instantiate(_fakeMirror);
        Vector3 v = Vector3.right;
        if (axisZ) v = Vector3.forward;
        
        fakeMirror.transform.position = transform.position + difference * v;
        fakeMirror.transform.localEulerAngles = new Vector3(0, transform.localEulerAngles.y, 0);

        if (stop) return;
        oppoPlace.isLooking(oppoPlace.transform.position + lookPoint - transform.position, true);
    }

    public void noLook()
    {
        Destroy(fakeMirror);
        fakeMirror = null;
    }

    public GameObject placeMirror(GameObject oppoMirror = null)
    {
        placedMirror = Instantiate(_realMirror);
        placedMirror.transform.position = fakeMirror.transform.position;
        placedMirror.transform.localEulerAngles = fakeMirror.transform.localEulerAngles;
        Destroy(fakeMirror);
        fakeMirror = null;

        if (oppoMirror != null)
        {
            placedMirror.GetComponentInChildren<Mirror>().oppoMirror = oppoMirror;
            return placedMirror;
        }
        placedMirror.GetComponentInChildren<Mirror>().oppoMirror = oppoPlace.placeMirror(placedMirror);
        return placedMirror;
    }

    public void deleteMirror()
    {
        
    }
}
