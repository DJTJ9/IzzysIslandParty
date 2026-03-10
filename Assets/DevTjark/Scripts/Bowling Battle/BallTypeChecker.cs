using UnityEngine;

public class BallTypeChecker : MonoBehaviour
{
    private BallType m_currentBallType;

    /// <summary>
    /// Detects when a collider enters the trigger zone.
    /// Updates the current ball type if a BowlingBallSwapper component is present in the colliding object.
    /// </summary>
    /// <param name="_other">The collider entering the trigger zone.</param>
    private void OnTriggerEnter(Collider _other)
    {
        var swapper = _other.GetComponentInChildren<BowlingBallSwapper>();

        if (swapper != null)
        {
            m_currentBallType = swapper.GetCurrentBallType();
        }
    }
    
    /// <summary>
    /// Retrieves the current ball type assigned to this object.
    /// </summary>
    /// <returns>The currently assigned BallType.</returns>
    public BallType GetCurrentBallType() => m_currentBallType;
}
