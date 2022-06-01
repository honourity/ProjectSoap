using System;

public interface IInventoryItem : IEquatable<IInventoryItem>
{
   Guid Id { get; }

   /// <summary>
   /// Automatically used by Inventory
   /// </summary>
   void Added();

   /// <summary>
   /// See <see cref="IInventoryItem.Added"/>
   /// </summary>
   void Removed();
}
