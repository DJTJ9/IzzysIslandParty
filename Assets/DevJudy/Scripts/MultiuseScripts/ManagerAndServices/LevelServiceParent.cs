using UnityEngine;
using UnityEngine.Events;

namespace MultiuseScripts
{
    public abstract class LevelServiceParent : MonoBehaviour
    {
       [SerializeField] protected UnityEvent OnLevelStart;
       [SerializeField] protected UnityEvent OnLevelEnd;
        
        public virtual void StartLevel(){ }

        public virtual void EndLevel() { }
    }
}