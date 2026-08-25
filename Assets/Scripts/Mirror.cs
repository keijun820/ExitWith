using UnityEngine;

public class Mirror : MonoBehaviour
{
    [SerializeField] Transform oppoMirrorPlace;
    [SerializeField] Transform myMirrorPlace;
    [SerializeField] GameObject _MirrorObject;
    public Mirror oppoMirror;
    public bool isWarfed;
    public float WarfTimer;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if(oppoMirror == null)
        {
            GameObject newMirror = Instantiate(_MirrorObject);
            newMirror.transform.position = oppoMirrorPlace.transform.position;
            newMirror.transform.eulerAngles = new Vector3(90, 0, -oppoMirrorPlace.eulerAngles.y);
            oppoMirror = newMirror.GetComponent<Mirror>();
            oppoMirror.oppoMirror = this;
            oppoMirror.oppoMirrorPlace = myMirrorPlace;
            oppoMirror.myMirrorPlace = oppoMirrorPlace;

            oppoMirrorPlace.GetComponent<MeshRenderer>().enabled = false;
        }
        isWarfed = false;
        WarfTimer = 0f;
    }

    void Update()
    {
        if(WarfTimer > 0)
        {
            WarfTimer -= Time.deltaTime;
            return;
        }
        Transform player = playerCheck();
        if (player == null)
        {
            isWarfed = false;
            return;
        }
        if (isWarfed) return;

        Vector3 offset = player.position - myMirrorPlace.position;

        float localX = Vector3.Dot(offset, myMirrorPlace.right);
        float localY = Vector3.Dot(offset, myMirrorPlace.up);
        float localZ = Vector3.Dot(offset, myMirrorPlace.forward);

        localX = -localX;
        localZ = -localZ;

        Vector3 newWorldPos = oppoMirrorPlace.position
                            + oppoMirrorPlace.right * localX
                            + oppoMirrorPlace.up * localY
                            + oppoMirrorPlace.forward * localZ;

        Quaternion relativeRot = Quaternion.Inverse(myMirrorPlace.rotation) * player.rotation;
        Quaternion halfTurn = Quaternion.Euler(0, 180, 0);
        relativeRot = halfTurn * relativeRot;
        Quaternion newWorldRot = oppoMirrorPlace.rotation * relativeRot;

        CharacterController cc = player.GetComponent<CharacterController>();
        cc.enabled = false;
        player.position = newWorldPos;
        player.rotation = newWorldRot;
        cc.enabled = true;

        oppoMirror.WarfTimer = 0.25f;
        WarfTimer = 0.25f;
        oppoMirror.isWarfed = true;
    }

    Transform playerCheck()
    {
        Collider[] cols = Physics.OverlapBox(transform.position, new Vector3(0.75f, 1.5f, 0.075f), Quaternion.Euler(0, -transform.eulerAngles.z, 0), 1 << LayerMask.NameToLayer("Player"));
        if (cols.Length > 0) return cols[0].transform;
        return null;
    }
}
