using UnityEngine;

// This script was created by ditzel (GitHub) aka DitzelGames (YouTube) and is provided free of charge under an MIT License

//    Copyright (c) 2019 D. E.
//
//    Permission is hereby granted, free of charge, to any person obtaining a copy
//    of this software and associated documentation files (the "Software"), to deal
//    in the Software without restriction, including without limitation the rights
//    to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//    copies of the Software, and to permit persons to whom the Software is
//    furnished to do so, subject to the following conditions:
//
// 
//     The above copyright notice and this permission notice shall be included in all
//     copies or substantial portions of the Software.
// 
//     THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//     IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//     FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//     AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//     LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//     OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
//     SOFTWARE.

namespace Ditzelgames
{
    public static class PhysicsHelper
    {

        public static void ApplyForceToReachVelocity(Rigidbody _rigidbody, Vector3 _velocity, float _force = 1, ForceMode _mode = ForceMode.Force)
        {
            if (_force == 0 || _velocity.magnitude == 0)
                return;

            _velocity = _velocity + _velocity.normalized * 0.2f * _rigidbody.linearDamping;

            //force = 1 => need 1 s to reach velocity (if mass is 1) => force can be max 1 / Time.fixedDeltaTime
            _force = Mathf.Clamp(_force, -_rigidbody.mass / Time.fixedDeltaTime, _rigidbody.mass / Time.fixedDeltaTime);

            //dot product is a projection from rhs to lhs with a length of result / lhs.magnitude https://www.youtube.com/watch?v=h0NJK4mEIJU
            if (_rigidbody.linearVelocity.magnitude == 0)
            {
                _rigidbody.AddForce(_velocity * _force, _mode);
            }
            else
            {
                var velocityProjectedToTarget = (_velocity.normalized * Vector3.Dot(_velocity, _rigidbody.linearVelocity) / _velocity.magnitude);
                _rigidbody.AddForce((_velocity - velocityProjectedToTarget) * _force, _mode);
            }
        }

        public static void ApplyTorqueToReachRps(Rigidbody _rigidbody, Quaternion _rotation, float _rps, float _force = 1)
        {
            var radPerSecond = _rps * 2 * Mathf.PI + _rigidbody.angularDamping * 20;

            float angleInDegrees;
            Vector3 rotationAxis;
            _rotation.ToAngleAxis(out angleInDegrees, out rotationAxis);

            if (_force == 0 || rotationAxis == Vector3.zero)
                return;

            _rigidbody.maxAngularVelocity = Mathf.Max(_rigidbody.maxAngularVelocity, radPerSecond);

            _force = Mathf.Clamp(_force, -_rigidbody.mass * 2 * Mathf.PI / Time.fixedDeltaTime, _rigidbody.mass * 2 * Mathf.PI / Time.fixedDeltaTime);

            var currentSpeed = Vector3.Project(_rigidbody.angularVelocity, rotationAxis).magnitude;

            _rigidbody.AddTorque(rotationAxis * (radPerSecond - currentSpeed) * _force);
        }

        public static Vector3 QuaternionToAngularVelocity(Quaternion _rotation)
        {
            float angleInDegrees;
            Vector3 rotationAxis;
            _rotation.ToAngleAxis(out angleInDegrees, out rotationAxis);

            return rotationAxis * angleInDegrees * Mathf.Deg2Rad;
        }

        public static Quaternion AngularVelocityToQuaternion(Vector3 _angularVelocity)
        {
            var rotationAxis = (_angularVelocity * Mathf.Rad2Deg).normalized;
            float angleInDegrees = (_angularVelocity * Mathf.Rad2Deg).magnitude;

            return Quaternion.AngleAxis(angleInDegrees, rotationAxis);
        }

        public static Vector3 GetNormal(Vector3[] _points)
        {
            //https://www.ilikebigbits.com/2015_03_04_plane_from_points.html
            if (_points.Length < 3)
                return Vector3.up;

            var center = GetCenter(_points);

            float xx = 0f, xy = 0f, xz = 0f, yy = 0f, yz = 0f, zz = 0f;

            for (int i = 0; i < _points.Length; i++)
            {
                var r = _points[i] - center;
                xx += r.x * r.x;
                xy += r.x * r.y;
                xz += r.x * r.z;
                yy += r.y * r.y;
                yz += r.y * r.z;
                zz += r.z * r.z;
            }

            var detX = yy * zz - yz * yz;
            var detY = xx * zz - xz * xz;
            var detZ = xx * yy - xy * xy;

            if (detX > detY && detX > detZ)
                return new Vector3(detX, xz * yz - xy * zz, xy * yz - xz * yy).normalized;
            if (detY > detZ)
                return new Vector3(xz * yz - xy * zz, detY, xy * xz - yz * xx).normalized;
            else
                return new Vector3(xy * yz - xz * yy, xy * xz - yz * xx, detZ).normalized;

        }

        public static Vector3 GetCenter(Vector3[] _points)
        {
            var center = Vector3.zero;
            for (int i = 0; i < _points.Length; i++)
                center += _points[i] / _points.Length;
            return center;
        }
    }
}
