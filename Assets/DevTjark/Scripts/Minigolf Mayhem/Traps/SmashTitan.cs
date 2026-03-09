using Audio;
using enums;
using Sirenix.OdinInspector;
using UnityEngine;

public class SmashTitan : MonoBehaviour
{
    [FoldoutGroup("Settings", expanded: true)]
    [SerializeField] private float m_impactForce = 100f;
    
    [SerializeField] private Transform impulseTarget;
    [SerializeField] private BoxCollider triggerZone;
    [SerializeField] private ParticleSystem dustParticles;

    /// <summary>
    /// Detects when the player enters the trigger zone and applies an impulse force
    /// to push the player in the direction of the impulse target.
    /// </summary>
    /// <param name="_other">The collider that entered the trigger zone.</param>
    private void OnTriggerEnter(Collider _other)
    {
        if (!_other.CompareTag("Player")) return;
        
        if (_other.TryGetComponent<Rigidbody>(out var _rb))
        {
            var impulseDirection = (impulseTarget.position - _rb.transform.position).normalized;
            _rb.AddForce(impulseDirection * m_impactForce * Time.fixedDeltaTime, ForceMode.Impulse);
        }
    }
    
    /// <summary>
    /// Enables the trigger zone to detect the player's presence.
    /// </summary>
    public void EnableTriggerZone()  => triggerZone.enabled = true;
    
    /// <summary>
    /// Disables the trigger zone, stopping detection of the player's presence.
    /// </summary>
    public void DisableTriggerZone() => triggerZone.enabled = false;
    
    /// <summary>
    /// Starts the dust particle effect.
    /// </summary>
    public void PlayDustParticles() => dustParticles.Play();
    
    /// <summary>
    /// Stops the dust particle effect.
    /// </summary>
    public void StopDustParticles() => dustParticles.Stop();

    /// <summary>
    /// Plays the sound associated with the smash action.
    /// </summary>
    public void PlaySmashSound()
    {
        AudioService.Instance.PlaySimpleSound(AudioCollection.Instance.LevelSoundsDictionary.LevelAudios["TitanSmash"], EAudioType.SFX);
    }
}
