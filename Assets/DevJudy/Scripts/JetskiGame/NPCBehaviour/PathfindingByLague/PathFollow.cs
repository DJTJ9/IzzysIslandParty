using System.Collections;
using Pathfinding;
using UnityEngine;

public class PathFollow : MonoBehaviour
{
    // For some reason, if the target is the finishLine, and not the child, it behaves very oddly
    [SerializeField] private Transform target;
    [SerializeField] private float speed = 5f;
    private int targetIndex;

    private Vector3[] path;

    [SerializeField] private bool recalculatePath;
    private bool targetReached = false;

    private void Start()
    {
        if (target == null)
            Debug.LogError("No target assigned on " + gameObject.name);

        //RequestPath();
    }

    public void StartRequestPath()
    {
        RequestPath();
    }

    [ContextMenu("RequestPath")]
    private void RequestPath()
    {
        PathRequestManager.RequestPath(transform.position, target.position, OnPathFound);
    }

    private void OnPathFound(Vector3[] _path, bool _foundPath)
    {
        if (_foundPath)
        {
            path = _path;

            StopCoroutine(FollowPath());
            StartCoroutine(FollowPath());
        }
    }

    private void FixedUpdate()
    {
        if (recalculatePath && !targetReached)
            StartRequestPath();
    }

    private IEnumerator FollowPath()
    {
        Vector3 currentPathPoint = path[0];

        while (true)
        {
            Debug.Log(" currentPath: " + currentPathPoint + " vs " + target.position);
            // Is this whats killing it?
            if (Mathf.Approximately(transform.position.x, currentPathPoint.x) && Mathf.Approximately(transform.position.z, currentPathPoint.z))
            {
                Debug.Log("In if");
                targetIndex++;

                currentPathPoint = path[targetIndex];

                if (targetIndex >= path.Length)
                {
                    Debug.Log("Im hereeeeeeeeeee");
                    yield break;
                }
            }

            transform.position = Vector3.MoveTowards(transform.position,
                new Vector3(currentPathPoint.x, transform.position.y, currentPathPoint.z), speed * Time.deltaTime);

            Debug.Log("In ze coroutine ");

            yield return null;
        }

        Debug.Log("Target reached");
        targetReached = true;
    }

    public void OnDrawGizmos()
    {
        if (path != null && target != null)
        {
            Gizmos.color = Color.green;

            Gizmos.DrawSphere(target.position, 5f);

            Gizmos.color = Color.black;

            for (int i = targetIndex; i < path.Length; i++)
            {
                Gizmos.DrawCube(path[i], Vector3.one);

                if (i == targetIndex)
                {
                    Gizmos.DrawLine(transform.position, path[i]);
                }
                else
                {
                    Gizmos.DrawLine(path[i - 1], path[i]);
                }
            }
        }
    }
}
