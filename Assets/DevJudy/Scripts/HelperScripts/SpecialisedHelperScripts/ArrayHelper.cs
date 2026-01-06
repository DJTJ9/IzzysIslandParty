using UnityEngine;

namespace HelperScripts
{
    public class ArrayHelper
    {
        public static T[] AddToArray<T>(T[] _arrayToAddTo, T _objectToAdd)
        {
            bool success = false;
            for (int i = 0; i < _arrayToAddTo.Length; i++)
            {
                if (_arrayToAddTo[i] == null)
                {
                    _arrayToAddTo[i] = _objectToAdd;
                    success = true;
                }
            }

            if (!success)
                Debug.LogError("Array: " + _arrayToAddTo + " has no space left for " + _objectToAdd);

            return _arrayToAddTo;
        }

        // !! Add this maybe
        public static T[] RemoveFromArray<T>(T[] _arrayToRemoveFrom, T _objectToRemove)
        {
            bool success = false;

            for (int i = 0; i < _arrayToRemoveFrom.Length; i++)
            {
                if (_arrayToRemoveFrom[i].Equals(_objectToRemove))
                {
                    success = true;
                    _arrayToRemoveFrom[i] = default(T);
                }
            }
            
            if (!success)
                Debug.LogError("Could not remove " + _objectToRemove + " from " + _arrayToRemoveFrom);
            
            return _arrayToRemoveFrom;
        }

        public static T[] ResizeArray<T>(T[] _arrayToResize, int _neededSize)
        {
            T[] newArray = new T[_neededSize];

            for (int i = 0; i < _neededSize; i++)
            {
                newArray[i] = _arrayToResize[i];
            }

            return newArray;
        }
    }
}