using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PointAndClickAgent : MonoBehaviour
{
    public Camera cam;

    public NavMeshAgent agent;
    public LayerMask groundLayer;

    private Vector3 pos;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out var hit, 100f, groundLayer))
            {
                agent.SetDestination(hit.point);
                pos = hit.point;
            }
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(pos, 0.3f);
    }
}
