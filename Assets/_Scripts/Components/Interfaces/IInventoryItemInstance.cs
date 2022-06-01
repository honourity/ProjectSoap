public interface IInventoryItemInstance
{
   /// <summary>
   /// Convert from a GameObject instance representing an InventoryItem, to an actual InventoryItem
   /// </summary>
   /// <returns></returns>
   IInventoryItem ToInventoryItem();
}