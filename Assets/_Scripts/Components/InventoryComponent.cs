using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class InventoryComponent : MonoBehaviour
{
   private static System.Object _dataLock = new System.Object { };

   private IList<IInventoryItem> _items = new List<IInventoryItem>();

   public IList<IInventoryItem> GetItems()
   {
      return GetItemsOfType<IInventoryItem>();
   }

   public IList<T> GetItemsOfType<T>() where T : IInventoryItem 
   {
      var itemsOfType = new List<T>();

      lock (_dataLock)
      {
         var resultItems = _items.Where(item => item is T);

         foreach (var item in resultItems)
         {
            itemsOfType.Add((T)item);
         }
      }

      return itemsOfType;
   }

   public void AddItem(IInventoryItemInstance item)
   {
      AddItem(item?.ToInventoryItem());
   }

   public void AddItem(IInventoryItem item)
   {
      lock (_dataLock)
      {
         if (item?.Id == Guid.Empty) Debug.LogWarning("Tried to add an item which was not initialized properly. Item Id: " + item?.Id.ToString());
         if (_items.Contains(item)) Debug.LogWarning("Tried to add an item which is already in inventory! This should never happen. Item Id: " + item?.Id.ToString());

         if (!_items.Contains(item))
         {
            _items.Add(item);

            item?.Added();
         }
      }
   }

   public void RemoveItem(IInventoryItemInstance item)
   {
      RemoveItem(item?.ToInventoryItem());
   }

   public void RemoveItem(IInventoryItem item)
   {
      lock (_dataLock)
      {
         if (_items.Contains(item))
         {
            _items.Remove(item);

            item?.Removed();
         }
      }
   }
}
