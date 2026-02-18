using System.Collections;
using UnityEngine;

namespace Pathfinding
{
    public class PathfindingUnit : Controller
    {
        private Rigidbody rb;
        
        private const float minPathUpdateTime = 0.2f;
        private const float pathUpdateThreshold = 0.5f;
        private const float squareMoveThreshold = pathUpdateThreshold * pathUpdateThreshold;

        [SerializeField] private Transform target;

        [SerializeField] private float speed = 20f;
        [SerializeField] private float turnSpeed = 2.5f;
        [SerializeField] private float turnDistance = 10f;
        [SerializeField] private float stoppingDistance = 2f;

        private SmoothPath path;
        int pathIndex = 0;

        private Coroutine followPathRoutine;

        private bool canFollowPath = false;

        private void Start()
        {
            rb = GetComponent<Rigidbody>();
        }

        public void SetTarget(Transform _target)
        {
            target = _target;
        }

        public void CanFollowPath()
        {
            canFollowPath = true;
            
            StartCoroutine(UpdatePath());
        }
        
        private void FixedUpdate()
        {
          if (followPathRoutine != null)
          {
              Quaternion targetRotation = Quaternion.LookRotation(path.LookPoints[pathIndex] - transform.position);
              transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, turnSpeed * Time.deltaTime);
          }
        }
        
        private void OnPathFound(Vector3[] _wayPoints, bool _foundPath)
        {
            if (_foundPath)
            {
                path = new SmoothPath(_wayPoints, transform.position, turnDistance, stoppingDistance);
                
                if (followPathRoutine != null)
                {
                    StopCoroutine(followPathRoutine);
                    followPathRoutine = null;
                }

                if (followPathRoutine == null)
                    followPathRoutine = StartCoroutine(FollowPath());
            }
        }
        
        private IEnumerator UpdatePath()
        {
            if (Time.timeSinceLevelLoad < 0.3f)
                yield return new WaitForSeconds(0.3f);

            PathRequestManager.RequestPath(transform.position, target.position, OnPathFound);

            Vector3 formerTargetPos = target.position;

            while (true)
            {
                yield return new WaitForSeconds(minPathUpdateTime);

                if ((target.position - formerTargetPos).sqrMagnitude >= squareMoveThreshold)
                {
                    PathRequestManager.RequestPath(transform.position, target.position, OnPathFound);
                    formerTargetPos = target.position;
                }
            }
        }

        private IEnumerator FollowPath()
        {
            bool followingPath = true;
            pathIndex = 0;

            float speedPercent = 1f;

            while (!canFollowPath)
                yield return new WaitForFixedUpdate();
            
            while (followingPath)
            {
                Vector2 pos2D = new Vector2(transform.position.x, transform.position.z);

                while (path.TurnBoundaries[pathIndex].HasCrossed(pos2D))
                {
                    if (pathIndex >= path.FinishLineIndex)
                    {
                        followingPath = false;
                        break;
                    }

                    pathIndex++;
                }

                if (followingPath)
                {
                    if (pathIndex >= path.SlowDownIndex && stoppingDistance > 0)
                    {
                        speedPercent = Mathf.Clamp01(path.TurnBoundaries[path.FinishLineIndex].DistanceFromPoint(pos2D) / stoppingDistance);

                        if (speedPercent < 0.01f)
                            followingPath = false;
                    }
                  
                    rb.AddForce((speed * speedPercent) * transform.forward, ForceMode.Force);
                }

                yield return new WaitForFixedUpdate();
            }

            yield return null;
        }

        public void OnDrawGizmos()
        {
            if (path != null)
                path.DrawWithGizmos();
        }
    }
}