using UnityEngine;
using Unity.Netcode;

public class DestroyOnExit : StateMachineBehaviour
{
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (animator.TryGetComponent<NetworkObject>(out var networkObject))
        {
            networkObject.Despawn(true);
        }
    }
}
