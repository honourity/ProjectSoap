namespace Enums
{
   public enum MessageType
   {
      PATH_CHANGED,

      //HygieneComponent
      HYGIENE_CHANGED,

      //int
      SHOWER_STARTED,

      //int
      SHOWER_STARTED_PLAYER,

      //int
      SHOWER_FINISHED,

      NPC_CLEANED_BY_SOAP,

      //SoapModel
      INVENTORY_SOAP_ADDED,

      //SoapModel
      INVENTORY_SOAP_REMOVED,

      //WaterModel
      INVENTORY_WATER_ADDED,

      //WaterModel
      INVENTORY_WATER_REMOVED,

      STAMINA_USE_FAILED,

      GAME_STARTED,

      GAME_PAUSED,

      GAME_RESUMED,

      GAME_RESET,

      GAME_OVER,

      //CollectableSoapController
      SOAP_COLLECTABLE_DESTROYED
   }

   public enum MoveType
   {
      NONE = 0,
      IDLE_LAND = 1,
      WALK_LAND = 2,
      RUN_LAND = 3,
      DASH_LAND = 4,
      IDLE_WATER = 5,
      WALK_WATER = 6,
      RUN_WATER = 7,
      DASH_WATER = 8,
      ATTACK = 9,
      FALL = 10,
      GRAB = 11,
      GRABBED = 12,
      STUNNED = 13,
      CLEANING = 14,
   }

   public enum Direction
   {
      NORTH = 0,
      NORTH_EAST = 1,
      EAST = 2,
      SOUTH_EAST = 3,
      SOUTH = 4,
      SOUTH_WEST = 5,
      WEST = 6,
      NORTH_WEST = 7,
   }

   public enum Size
   {
      NONE = 0,
      SMALL = 1,
      MEDIUM = 2,
      LARGE = 3,
   }

   public enum AIType
   {
      SomeDude,
      Drippy,
      Janitor,
      Zombie,
      SomeOtherAI
   }
}
