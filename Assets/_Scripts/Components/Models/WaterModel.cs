using System;

public class WaterModel : IInventoryItem
{
   public Guid Id { get; private set; }

   public float Volume { get; set; }

   public WaterModel(WaterModel water) : this(water.Id, water.Volume) { }

   public WaterModel(Guid id, float volume)
   {
      Id = id;
      Volume = volume;
   }

   public void Added()
   {
      Messaging.SendMessage(Enums.MessageType.INVENTORY_WATER_ADDED, this);
   }

   public void Removed()
   {
      Messaging.SendMessage(Enums.MessageType.INVENTORY_WATER_REMOVED, this);
   }

   public void AddVolume(float volume)
   {
      Volume += volume;
   }

   /// <summary>
   /// </summary>
   /// <param name="volume">amount of water to remove</param>
   /// <returns>quantity of water which was removed successfuly</returns>
   public float RemoveWater(float volume)
   {
      var volumeRemoved = volume;

      Volume -= volume;

      if (volume < 0)
      {
         volumeRemoved += Volume;
         Volume = 0f;
      }

      return volumeRemoved;
   }

   public bool Equals(IInventoryItem other)
   {
      return (Id == other.Id);
   }
}
