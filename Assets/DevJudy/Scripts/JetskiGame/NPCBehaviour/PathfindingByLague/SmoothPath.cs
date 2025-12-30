using UnityEngine;

namespace Pathfinding
{
    public class SmoothPath
    {
        public readonly Vector3[] LookPoints;
        public readonly SLine[] TurnBoundaries;
        public readonly int FinishLineIndex;
        public readonly int SlowDownIndex;

        public SmoothPath(Vector3[] _wayPoints, Vector3 _startPos, float _turnDistance, float _stoppingDistance)
        {
            LookPoints = _wayPoints;
            TurnBoundaries = new SLine[LookPoints.Length];
            FinishLineIndex = TurnBoundaries.Length - 1;

            Vector2 previousPoint = Vector3ToVector2(_startPos);

            for (int i = 0; i < LookPoints.Length; i++)
            {
                Vector2 currentPoint = Vector3ToVector2(LookPoints[i]);
                Vector2 dirToCurrentPoint = (currentPoint - previousPoint).normalized;
                Vector2 turnBoundaryPoint = (i == FinishLineIndex) ? currentPoint : currentPoint - dirToCurrentPoint * _turnDistance;

                TurnBoundaries[i] = new SLine(turnBoundaryPoint, previousPoint - dirToCurrentPoint * _turnDistance);
                previousPoint = turnBoundaryPoint;
            }

            float distanceFromEndPoint = 0f;

            for (int i = LookPoints.Length - 1; i > 0; i--)
            {
                distanceFromEndPoint += Vector3.Distance(LookPoints[i], LookPoints[i - 1]);

                if (distanceFromEndPoint > _stoppingDistance)
                {
                    SlowDownIndex = i;
                    break;
                }
            }
        }

        private Vector2 Vector3ToVector2(Vector3 _v3)
        {
            return new Vector2(_v3.x, _v3.z);
        }

        public void DrawWithGizmos()
        {
            Gizmos.color = Color.black;

            foreach (Vector3 p in LookPoints)
            {
                Gizmos.DrawCube(p + Vector3.up, Vector3.one);
            }

            Gizmos.color = Color.red;

            foreach (SLine l in TurnBoundaries)
            {
                l.DrawWithGizmos(10f);
            }
        }
    }
}
