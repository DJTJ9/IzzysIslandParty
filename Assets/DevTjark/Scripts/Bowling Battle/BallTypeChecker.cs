using System;
using UnityEngine;

public class BallTypeChecker : MonoBehaviour
{
    private BallType m_currentBallType;

    private void OnTriggerEnter(Collider other)
    {
        var swapper = other.GetComponentInChildren<BowlingBallSwapper>();

        if (swapper != null)
        {
            m_currentBallType = swapper.GetCurrentBallType();
        }
    }
    
    public BallType GetCurrentBallType() => m_currentBallType;
}
