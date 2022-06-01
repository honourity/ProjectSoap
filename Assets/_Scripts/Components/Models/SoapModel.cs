using System;

public class SoapModel : IInventoryItem
{
   public Guid Id { get; private set; }

   public Enums.Size Size;
   public float Hygiene;

   public SoapModel(SoapModel soap) : this(soap.Id, soap.Size, soap.Hygiene) { }

   public SoapModel(Guid id, Enums.Size size, float hygiene)
   {
      Id = id;
      Size = size;
      Hygiene = hygiene;
   }

   public void Added()
   {
      Messaging.SendMessage(Enums.MessageType.INVENTORY_SOAP_ADDED, this);
   }

   public void Removed()
   {
      Messaging.SendMessage(Enums.MessageType.INVENTORY_SOAP_REMOVED, this);
   }

   public bool Equals(IInventoryItem other)
   {
      return (Id == other.Id);
   }
}
