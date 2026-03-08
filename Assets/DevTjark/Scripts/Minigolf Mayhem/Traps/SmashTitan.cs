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

    private void OnTriggerEnter(Collider _other)
    {
        if (!_other.CompareTag("Player")) return;
        
        if (_other.TryGetComponent<Rigidbody>(out var _rb))
        {
            var impulseDirection = (impulseTarget.position - _rb.transform.position).normalized;
            _rb.AddForce(impulseDirection * m_impactForce * Time.fixedDeltaTime, ForceMode.Impulse);
        }
    }
    
    public void EnableTriggerZone()  => triggerZone.enabled = true;
    public void DisableTriggerZone() => triggerZone.enabled = false;
    
    public void PlayDustParticles() => dustParticles.Play();
    
    public void StopDustParticles() => dustParticles.Stop();

    public void PlaySmashSound()
    {
        AudioService.Instance.PlaySimpleSound(AudioCollection.Instance.LevelSoundsDictionary.LevelAudios["TitanSmash"], EAudioType.SFX);
    }
}
