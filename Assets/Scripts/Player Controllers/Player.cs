using UnityEngine;
using Fusion;

namespace Starter.ThirdPersonCharacter
{
	public class Player : NetworkBehaviour
	{
		[SerializeField] private PlayerMovement movementController;
		[SerializeField] private PlayerInventory inventoryController;
		[SerializeField] private PlayerCombat combatController;
		[SerializeField] private PlayerHeath healthController;

		#region [Movement Values]
		public bool IsWalking => movementController != null ? movementController._isWalking : false;
		public bool IsRunning => movementController != null ? movementController._isRunning : false;
		public bool IsJumping => movementController != null ? movementController._isJumping : false;
		public bool IsFalling => movementController != null ? movementController._isFalling : false;
		public bool IsGrounded => movementController != null ? movementController._isGrounded : false;
		public float RealSpeed => movementController != null ? movementController._currentSpeed : 0f;
		public Vector3 CurrentPos => movementController != null ? movementController._currentPosition : Vector3.zero;
		public Vector3 CurrentForward => movementController != null ? movementController._currentForward : Vector3.zero;
		#endregion


		#region [Inventory Values]
		public int CurrentWeaponID => inventoryController != null ? inventoryController._currentWeaponID : -1;
		#endregion


		#region [Combat Values]

		#endregion


		#region [Heath Values]

		#endregion


		[Networked]
		private NetworkButtons _previousButtons { get; set; }




		public override void Spawned()
		{
			if (HasInputAuthority)
			{
				CameraManager cam = FindAnyObjectByType<CameraManager>();
				cam.TargetFollow = this.transform;
			}
		}


		public override void FixedUpdateNetwork()
		{
			var input = GetInput<GameplayInput>().GetValueOrDefault();
			movementController.Move(input, _previousButtons);
			_previousButtons = input.Buttons;
		}
	}
}
