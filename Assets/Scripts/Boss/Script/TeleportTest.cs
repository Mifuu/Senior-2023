using UnityEngine;
using Unity.Netcode;

public class TeleportTest : NetworkBehaviour
{
    [SerializeField] private Transform teleportExitLocation;

    public void OnTriggerEnter(Collider collider)
    {
        if (!IsServer) return;
        if (!collider.CompareTag("Player")) return;
        if (collider.TryGetComponent<CharacterController>(out var charController))
        {
            charController.enabled = false; // If it's not disabled, char controller will not allow position change.
            collider.transform.position = teleportExitLocation.position;
            charController.enabled = true;
        }
        else
            Debug.LogError("Teleportation failed");
    }
}
