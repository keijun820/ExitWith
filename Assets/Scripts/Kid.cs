using System.IO;
using UnityEngine;
using UnityEngine.AI;

public class Kid : MonoBehaviour
{
    [SerializeField] NavMeshAgent navAgent;
    [SerializeField] Transform player;
    NavMeshPath path;
    bool isFollowing;

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
            if(IsStopPosition()) navAgent.speed = 0f;
            else navAgent.speed = 5f;
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
