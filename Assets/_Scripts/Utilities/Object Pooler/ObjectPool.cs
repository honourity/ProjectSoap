using System;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool<T>
{
   public List<ObjectPoolContainer<T>> List;
   private Dictionary<T, ObjectPoolContainer<T>> lookup;
   private Func<T> factoryFunc;
   private int lastIndex = 0;

   public ObjectPool(Func<T> factoryFunc, int initialSize)
   {
      this.factoryFunc = factoryFunc;

      List = new List<ObjectPoolContainer<T>>(initialSize);
      lookup = new Dictionary<T, ObjectPoolContainer<T>>(initialSize);

      Warm(initialSize);
   }

   private void Warm(int capacity)
   {
      for (int i = 0; i < capacity; i++)
      {
         CreateContainer();
      }
   }

   private ObjectPoolContainer<T> CreateContainer()
   {
      var container = new ObjectPoolContainer<T>();
      container.Item = factoryFunc();
      List.Add(container);
      return container;
   }

   public T GetItem()
   {
      ObjectPoolContainer<T> container = null;
      for (int i = 0; i < List.Count; i++)
      {
         lastIndex++;
         if (lastIndex > List.Count - 1) lastIndex = 0;

         if (List[lastIndex].Used)
         {
            continue;
         }
         else
         {
            container = List[lastIndex];
            break;
         }
      }

      if (container == null)
      {
         container = CreateContainer();
      }

      container.Consume();
      lookup.Add(container.Item, container);
      return container.Item;
   }

   public void Release(object item)
   {
      Release((T)item);
   }

   public void Release(T item)
   {
      if (lookup.ContainsKey(item))
      {
         var container = lookup[item];
         container.Release();
         lookup.Remove(item);
      }
      else
      {
         Debug.LogWarning("This object pool does not contain the item provided: " + item);
      }
   }

   public int Count
   {
      get { return List.Count; }
   }

   public int CountUsedItems
   {
      get { return lookup.Count; }
   }
}
