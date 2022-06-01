using UnityEngine;

public class MovementComponent : MonoBehaviour
{
   public float WalkSpeed = 5f;
   public float RunSpeed = 15f;
   public float StunnedDuration = 1f;

   public float OriginalWalkSpeed { get; private set; }
   public float OriginalRunSpeed { get; private set; }
   public float OriginalStunnedDuration { get; private set; }

   private void Awake()
   {
      OriginalWalkSpeed = WalkSpeed;
      OriginalRunSpeed = RunSpeed;
      OriginalStunnedDuration = StunnedDuration;
   }
}
