using System.IO;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;

public class KidController : MonoBehaviour
{
    [SerializeField] NavMeshAgent navAgent;
    [SerializeField] Transform player;
    [SerializeField] PlayerController playerController;
    NavMeshPath path;
    bool isFollowing;
    float walkingSpeed = 8f;
    float lookSpeed = 2.0f;
    float lookXLimit = 45.0f;
    Vector3 moveDirection = Vector3.zero;
    float rotationX = 0;

    [SerializeField] Camera kidCamera;
    [SerializeField] MeshRenderer mesh;

    string animState; //나중에 animation을 위한 코드

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        isFollowing = false;
        path = new NavMeshPath();
    }

    // Update is called once per frame
    void Update()
    {
        Follow();
        Control();
    }

    void Follow()
    {
        if (playerController.kidControlling)
        {
            isFollowing = false;
            navAgent.SetDestination(transform.position);
            navAgent.speed = 0f;
            return;
        }

        if (Input.GetKeyDown(KeyCode.Q)) isFollowing = !isFollowing;

        if (!isFollowing)
        {
            navAgent.SetDestination(transform.position);
            navAgent.speed = 0f;
            return;
        }
        Vector3 playerPosition = PlayerPos();
        if (CalculateNewPath(playerPosition))
        {
            navAgent.SetDestination(playerPosition);
            if (IsStopPosition()) navAgent.speed = 0f;
            else navAgent.speed = 5f;
        }
    }

    void Control()
    {
        mesh.enabled = !playerController.kidControlling;
        if (isFollowing) return;
        if (!playerController.kidControlling)
        {
            moveDirection.x = 0;
            moveDirection.z = 0;
            return;
        }
        // We are grounded, so recalculate move direction based on axis
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 right = transform.TransformDirection(Vector3.right);

        float curSpeedX = walkingSpeed * Input.GetAxis("Vertical");
        float curSpeedY = walkingSpeed * Input.GetAxis("Horizontal");
        moveDirection = (forward * curSpeedX) + (right * curSpeedY);

        navAgent.Move(moveDirection * Time.deltaTime);

        // Player and Camera rotation
        if (kidCamera != null)
        {
            rotationX += -Input.GetAxis("Mouse Y") * lookSpeed;
            rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
            kidCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
            transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookSpeed, 0);
        }
    }

    Vector3 PlayerPos()
    {
        NavMeshHit hit;
        float f = 0.5f;
        while (!NavMesh.SamplePosition(player.position + Vector3.down, out hit, f, NavMesh.AllAreas)) f += 0.1f;
        return hit.position;
    }

    bool CalculateNewPath(Vector3 target)
    {
        navAgent.CalculatePath(target, path);

        if (path.status != NavMeshPathStatus.PathComplete) return false;
        else return true;
    }

    bool IsStopPosition()
    {
        Vector3 direction = player.position - transform.position + Vector3.up * 0.5f;
        float distance = direction.magnitude;

        RaycastHit hit;

        if (Physics.Raycast(transform.position, direction.normalized, out hit, distance))
        {
            if (hit.transform == player && distance <= 5f)
            {
                return true;
            }
        }
        return false;
    }

    void Animate(string s)
    {
        if (animState == s) return;
        animState = s;
        //anim.SetTrigger(s);
    }
}
