using UnityEngine;

public class Mirror : MonoBehaviour
{
    public GameObject oppoMirror;
    [HideInInspector] public bool isWarfed;
    [HideInInspector] public float warfTimer;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        isWarfed = false;
        warfTimer = 0f;
    }

    void Update()
    {
        if(warfTimer > 0)
        {
            warfTimer -= Time.deltaTime;
            return;
        }
        Transform player = playerCheck();
        if (player == null)
        {
            isWarfed = false;
            return;
        }
        if (isWarfed) return;

        Vector3 offset = player.position - transform.parent.position;

        float localX = Vector3.Dot(offset, transform.parent.right);
        float localY = Vector3.Dot(offset, transform.parent.up);
        float localZ = Vector3.Dot(offset, transform.parent.forward);

        localX = -localX;
        localZ = -localZ;

        Vector3 newWorldPos = oppoMirror.transform.position
                            + oppoMirror.transform.right * localX
                            + oppoMirror.transform.up * localY
                            + oppoMirror.transform.forward * localZ;

        Quaternion relativeRot = Quaternion.Inverse(transform.parent.rotation) * player.rotation;
        Quaternion halfTurn = Quaternion.Euler(0, 180, 0);
        relativeRot = halfTurn * relativeRot;
        Quaternion newWorldRot = oppoMirror.transform.rotation * relativeRot;

        CharacterController cc = player.GetComponent<CharacterController>();
        cc.enabled = false;
        player.position = newWorldPos;
        player.rotation = newWorldRot;
        cc.enabled = true;

        oppoMirror.GetComponentInChildren<Mirror>().warfTimer = 0.25f;
        warfTimer = 0.25f;
        oppoMirror.GetComponentInChildren<Mirror>().isWarfed = true;
    }

    Transform playerCheck()
    {
        Collider[] cols = Physics.OverlapBox(transform.parent.position, new Vector3(0.75f, 1.5f, 0.075f), Quaternion.Euler(0, -transform.parent.eulerAngles.z, 0), 1 << LayerMask.NameToLayer("Player"));
        if (cols.Length > 0) return cols[0].transform;
        return null;
    }
}
