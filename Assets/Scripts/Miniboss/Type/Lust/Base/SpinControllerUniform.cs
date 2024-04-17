using UnityEngine;
using Unity.Netcode;

namespace Enemy
{
    public class SpinControllerUniform : NetworkBehaviour
    {
        [SerializeField] public float spinRate;
        [SerializeField] public bool isSpinningClockwise;

        private NetworkVariable<bool> _isSpinningClockwise = new NetworkVariable<bool>();
        private float spinDirection = 1.0f;

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            if (!IsServer) return;
            _isSpinningClockwise.Value = isSpinningClockwise;
        }

        public void FixedUpdate()
        {
            transform.Rotate(0f, spinDirection * spinRate, 0, Space.Self);
        }

        private void ChangeSpinDirection(bool _, bool current)
        {
            if (current)
            {
                spinDirection = 1.0f;
                return;
            }
            spinDirection = -1.0f;
        }
    }
}
