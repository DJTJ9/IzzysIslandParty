using System;
using System.Collections;
using enums;
using Juice;
using Player;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Pathfinding
{
    public class JetskiNPCBehaviour : Controller
    {
        private const float minPathUpdateTime = 0.2f;
        private const float pathUpdateThreshold = 25f;
        private const float squareMoveThreshold = pathUpdateThreshold * pathUpdateThreshold;
        
        private SO_PlayerRacingGames player;
        private Rigidbody rb;

        [SerializeField] private IconHandler iconHandler;
        [SerializeField] private Transform target;
        
        [Header("Movement variables: ")]
        [SerializeField] private float speed = 20f;
        [SerializeField] private float turnSpeed = 2.5f;
        [SerializeField] private float turnDistance = 10f;
        [SerializeField] private float stoppingDistance = 2f;
        private Vector3 previousPosition;
        private Vector3 goingBackwardsFromPosition;
        
        [Header("Obstacle Check:")]
        [SerializeField] private float obstacleCheckSize;
        [SerializeField] private Vector3 obstacleCheckPosition;
        [SerializeField] private LayerMask obstacleLayerMask;

        private SmoothPath path;
        int pathIndex = 0;

        private Coroutine followPathRoutine;
        
        private int timeDeductionMinutes;
        private int timeDeductionSeconds;

        private bool wentFarEnoughBack = false;
        private bool canFollowPath = false;
        private bool finishLineCrossed = false;

        [SerializeField] private bool debug;

        private void Start()
        {
            rb = GetComponent<Rigidbody>();

            previousPosition = new Vector3(rb.position.x * -2, rb.position.y, rb.position.z * -2);
        }

        public override void OnNPCJoined(SO_PlayerRacingGames _player)
        {
            player = _player;

            player.Time = String.Empty;
            player.TimeValue = 0;
            player.PlayerScore.Value = 0;
        }

        public void SetTarget(Transform _target)
        {
            target = _target;
        }

        public void SetPlacement(int _placement)
        {
            player.PlayerScore.Value = _placement;
        }

        public void SetTime(string _time)
        {
            player.Time = _time;
        }

        public void CanFollowPath()
        {
            canFollowPath = true;

            StartCoroutine(UpdatePath());
        }

        private void FixedUpdate()
        {
            if (followPathRoutine != null && !finishLineCrossed)
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

                followPathRoutine = StartCoroutine(FollowPath());
            }
        }

        private IEnumerator UpdatePath()
        {
            if (Time.timeSinceLevelLoad < 0.3f)
                yield return new WaitForSeconds(0.3f);

            PathRequestManager.RequestPath(transform.position, target.position, OnPathFound);

            Vector3 formerTargetPos = Vector3.zero;

            while (canFollowPath && !finishLineCrossed)
            {
                yield return new WaitForSeconds(minPathUpdateTime);

                if ((target.position - formerTargetPos).sqrMagnitude >= squareMoveThreshold ||
                    (transform.position - previousPosition).sqrMagnitude >= squareMoveThreshold)
                {
                    PathRequestManager.RequestPath(transform.position, target.position, OnPathFound);

                    formerTargetPos = target.position;
                    previousPosition = transform.position;
                }
            }

            yield return null;
        }

        private IEnumerator FollowPath()
        {
            bool followingPath = true;
            pathIndex = 0;

            float speedPercent = 1f;

            while (!canFollowPath)
                yield return new WaitForFixedUpdate();

            while (followingPath && !finishLineCrossed)
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

                if (!followingPath)
                    break;

                if (pathIndex >= path.SlowDownIndex && stoppingDistance > 0)
                {
                    speedPercent = Mathf.Clamp01(path.TurnBoundaries[path.FinishLineIndex].DistanceFromPoint(pos2D) / stoppingDistance);

                    if (speedPercent < 0.01f)
                        followingPath = false;
                }

                if ((previousPosition - transform.position).sqrMagnitude < 0.01f)
                {
                    Collider[] colliders = Physics.OverlapSphere(transform.position + obstacleCheckPosition, obstacleCheckSize, obstacleLayerMask);

                    if (colliders.Length > 1)
                    {
                        for (int i = 0; i < colliders.Length; i++)
                        {
                            if (!colliders[i].gameObject.CompareTag("Obstacle"))
                                continue;
                            
                            if (debug)
                                Debug.Log(gameObject.name + " is going backwards");
                            
                            rb.AddForce((speed * speedPercent) * (transform.forward * -1), ForceMode.Force);
                        }
                    }
                }
                else
                    rb.AddForce((speed * speedPercent) * transform.forward, ForceMode.Force);

                previousPosition = transform.position;

                yield return new WaitForFixedUpdate();
            }

            yield return null;
        }

        public void OnObstacleCleared()
        {
            int randomEmote = Random.Range(0, 2);

            if (randomEmote == 0)
                iconHandler.DisplayIcon(EEmotion.Love);
            else
                iconHandler.DisplayIcon(EEmotion.Happy);
        }

        public void OnObstacleMissed(float _timeDeduction, out int _timeDeductionMinutes, out int _timeDeductionSeconds)
        {
            int randomEmote = Random.Range(0, 2);

            switch (randomEmote)
            {
                case 0:
                    iconHandler.DisplayIcon(EEmotion.Sad);
                    break;
                default:
                    iconHandler.DisplayIcon(EEmotion.Embarrassed);
                    break;
            }

            int currentDeduction = (int)(timeDeductionSeconds + _timeDeduction);

            if (currentDeduction >= 60)
            {
                timeDeductionMinutes++;
                timeDeductionSeconds = (currentDeduction % 60);
            }
            else
                timeDeductionSeconds = currentDeduction;

            _timeDeductionMinutes = timeDeductionMinutes;
            _timeDeductionSeconds = timeDeductionSeconds;
        }

        public void OnFinishLineCrossed()
        {
            finishLineCrossed = true;
        }

        public void GetFinalTimeDeduction(out int _minutes, out int _seconds)
        {
            _minutes = timeDeductionMinutes;
            _seconds = timeDeductionSeconds;
        }

        public void OnDrawGizmos()
        {
            if (path != null)
                path.DrawWithGizmos();
        }
    }
}