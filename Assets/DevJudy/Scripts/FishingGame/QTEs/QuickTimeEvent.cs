using UnityEngine;

namespace FishingGame.QuickTimeEvents
{
    public class QuickTimeEvent : MonoBehaviour
    {
        public bool QTERunning { get; protected set; }
        public bool QTEFinishedSuccessfully { get; protected set; }

        public virtual void StartQTE()
        {
            
        }
    }
}

