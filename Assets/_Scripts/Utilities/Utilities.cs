using UnityEngine;

public static class Utilities
{

   /// <summary>
   /// 
   /// </summary>
   /// <param name="line1start">Start position of Line Segment 1</param>
   /// <param name="line1end">End position of Line Segment 1</param>
   /// <param name="line2start">Start position of Line Segment 2</param>
   /// <param name="line2end">End position of Line Segment 2</param>
   /// <param name="intersection">The point of intersection between both line segments, if it exists. Passed as ref</param>
   /// <returns>True if intersection found. False if no intersection</returns>
   public static bool LineIntersection(Vector2 line1start, Vector2 line1end, Vector2 line2start, Vector2 line2end, ref Vector2 intersection)
   {
      float Ax, Bx, Cx, Ay, By, Cy, d, e, f, num/*,offset*/;
      float x1lo, x1hi, y1lo, y1hi;

      Ax = line1end.x - line1start.x;
      Bx = line2start.x - line2end.x;

      // X bound box test/
      if (Ax < 0)
      {
         x1lo = line1end.x; x1hi = line1start.x;
      }
      else
      {
         x1hi = line1end.x; x1lo = line1start.x;
      }

      if (Bx > 0)
      {
         if (x1hi < line2end.x || line2start.x < x1lo) return false;
      }
      else
      {
         if (x1hi < line2start.x || line2end.x < x1lo) return false;
      }

      Ay = line1end.y - line1start.y;
      By = line2start.y - line2end.y;

      // Y bound box test//
      if (Ay < 0)
      {
         y1lo = line1end.y; y1hi = line1start.y;
      }
      else
      {
         y1hi = line1end.y; y1lo = line1start.y;
      }

      if (By > 0)
      {
         if (y1hi < line2end.y || line2start.y < y1lo) return false;
      }
      else
      {
         if (y1hi < line2start.y || line2end.y < y1lo) return false;
      }

      Cx = line1start.x - line2start.x;
      Cy = line1start.y - line2start.y;
      d = By * Cx - Bx * Cy;  // alpha numerator//
      f = Ay * Bx - Ax * By;  // both denominator//

      // alpha tests//
      if (f > 0)
      {
         if (d < 0 || d > f) return false;
      }
      else
      {
         if (d > 0 || d < f) return false;
      }

      e = Ax * Cy - Ay * Cx;  // beta numerator//

      // beta tests //
      if (f > 0)
      {
         if (e < 0 || e > f) return false;
      }
      else
      {
         if (e > 0 || e < f) return false;
      }

      // check if they are parallel

      if (f == 0) return false;

      // compute intersection coordinates //
      num = d * Ax; // numerator //

      //    offset = same_sign(num,f) ? f*0.5f : -f*0.5f;   // round direction //
      //    intersection.x = p1.x + (num+offset) / f;
      intersection.x = line1start.x + num / f;
      num = d * Ay;

      //    offset = same_sign(num,f) ? f*0.5f : -f*0.5f;
      //    intersection.y = p1.y + (num+offset) / f;
      intersection.y = line1start.y + num / f;

      return true;
   }
}
