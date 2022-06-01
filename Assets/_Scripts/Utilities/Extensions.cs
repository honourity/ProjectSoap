using System;
using UnityEngine;

public static class Extensions
{
   public static Transform FindDeep(this Transform rootTransform, string childTransformName)
   {
      foreach (Transform child in rootTransform)
      {
         if (child.name == childTransformName)
            return child;
         var result = child.FindDeep(childTransformName);
         if (result != null)
            return result;
      }
      return null;
   }

   public static TEnum ToCardinalDirection<TEnum>(this float angle)
   {
      var directions = Enum.GetValues(typeof(TEnum)).Length;
      var slice = 360f / directions;
      var rotate = angle + slice / 2;
      var result = (Int32)Math.Truncate(rotate / slice);
      return (TEnum)(System.Object)(result % directions);
   }

   public static TEnum ToCardinalDirection<TEnum>(this Vector2 vector)
   {
      var angle = Vector3.SignedAngle(Vector3.up, vector, Vector3.back);
      if (angle < 0) angle += 360f;

      return angle.ToCardinalDirection<TEnum>();
   }

   public static float ToAngle(this Enums.Direction direction)
   {
      return (int)direction * 45f;
   }

   public static float Map(this float value, float fromMin, float fromMax, float toMin, float toMax)
   {
      var fromRange = (fromMax - fromMin);
      var toRange = (toMax - toMin);
      var mappedValue = (((value - fromMin) * toRange) / fromRange) + toMin;
      return mappedValue;
   }

   public static Vector2 ToVector(this Enums.Direction direction)
   {
      return new Vector2(Mathf.Sin(direction.ToAngle() * Mathf.Deg2Rad), Mathf.Cos(direction.ToAngle() * Mathf.Deg2Rad));
   }

   public static Enums.Direction SnapTo4Way(this Enums.Direction direction)
   {
      if (direction > Enums.Direction.NORTH && direction < Enums.Direction.SOUTH)
      {
         return Enums.Direction.EAST;
      }
      else if (direction > Enums.Direction.SOUTH)
      {
         return Enums.Direction.WEST;
      }
      else
      {
         return direction;
      }
   }
}