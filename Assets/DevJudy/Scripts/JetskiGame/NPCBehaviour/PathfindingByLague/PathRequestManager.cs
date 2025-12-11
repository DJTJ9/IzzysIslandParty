using System;
using System.Collections.Generic;
using UnityEngine;

namespace Pathfinding
{
    public class PathRequestManager : MonoBehaviour
    {
        private static PathRequestManager instance;
        private Pathfinding pathfinding;

        private Queue<SPathRequest> requestQueue = new Queue<SPathRequest>();
        private SPathRequest currentRequest;

        private bool processingPath = false;

        private void Awake()
        {
            instance = this;
            pathfinding = GetComponent<Pathfinding>();
        }

        public static void RequestPath(Vector3 _start, Vector3 _end, Action<Vector3[], bool> _callback)
        {
            SPathRequest request = new SPathRequest(_start, _end, _callback);
            instance.requestQueue.Enqueue(request);

            instance.TryProcessNext();
        }

        public void FinishedProcessingPath(Vector3[] _path, bool _success)
        {
            currentRequest.Callback(_path, _success);
            processingPath = false;

            TryProcessNext();
        }

        private void TryProcessNext()
        {
            if (!processingPath && requestQueue.Count > 0)
            {
                currentRequest = requestQueue.Dequeue();
                processingPath = true;

                pathfinding.StartFindPath(currentRequest.PathStart, currentRequest.PathEnd);
            }
        }

        struct SPathRequest
        {
            public Vector3 PathStart;
            public Vector3 PathEnd;
            public Action<Vector3[], bool> Callback;

            public SPathRequest(Vector3 _pathStart, Vector3 _pathEnd, Action<Vector3[], bool> _callback)
            {
                PathStart = _pathStart;
                PathEnd = _pathEnd;
                Callback = _callback;
            }
        }
    }
}