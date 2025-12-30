using UnityEngine;

namespace Pathfinding
{
    public struct SLine
    {
        private const float verticalLineGradient = 1e5f;

        private float gradient;
        private float gradientPerpendicular;
        private float yIntercept;

        private Vector2 pointOnLine1;
        private Vector2 pointOnLine2;

        private bool approachSide;

        public SLine(Vector2 _pointOnLine, Vector2 _pointPerpendicularToLine)
        {
            float deltaX = _pointOnLine.x - _pointPerpendicularToLine.x;
            float deltaY = _pointOnLine.y - _pointPerpendicularToLine.y;

            gradientPerpendicular = deltaX == 0 ? verticalLineGradient : deltaY / deltaX;

            if (gradientPerpendicular == 0)
                gradient = verticalLineGradient;
            else
                gradient = -1 / gradientPerpendicular;

            yIntercept = _pointPerpendicularToLine.y - gradient * _pointOnLine.x;

            pointOnLine1 = _pointOnLine;
            pointOnLine2 = _pointOnLine + new Vector2(1, gradient);

            approachSide = false;
            approachSide = GetSide(_pointPerpendicularToLine);
        }

        private bool GetSide(Vector2 _p)
        {
            return (_p.x - pointOnLine1.x) * (pointOnLine2.y - pointOnLine1.y) > (_p.y - pointOnLine1.y) * (pointOnLine2.x - pointOnLine1.x);
        }

        public bool HasCrossed(Vector2 _p)
        {
            return GetSide(_p) != approachSide;
        }

        public float DistanceFromPoint(Vector2 _p)
        {
            float yInterceptPerpendicualr = _p.y - gradientPerpendicular * _p.x;

            float intersectX = (yInterceptPerpendicualr - yIntercept) / (gradient - gradientPerpendicular);
            float intersectY = gradient * intersectX + yIntercept;

            return Vector2.Distance(_p, new Vector2(intersectX, intersectY));
        }


        public void DrawWithGizmos(float _length)
        {
            Vector3 lineDir = new Vector3(1f, 0f, gradient).normalized;
            Vector3 lineCenter = new Vector3(pointOnLine1.x, 0f, pointOnLine1.y) + Vector3.up;

            Gizmos.DrawLine(lineCenter - lineDir * _length / 2f, lineCenter + lineDir * _length / 2f);
        }
    }
}