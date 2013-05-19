namespace Clunch.Models

type Direction =
    | North
    | South
    | East
    | West
    | Northwest
    | Northeast
    | Southwest
    | Southeast
    | Up
    | Down
    | Custom of string

    override x.ToString() =
        match x with
        | North -> "North"
        | South -> "South"
        | East -> "East"
        | West -> "West"
        | Northwest -> "Northwest"
        | Northeast -> "Northeast"
        | Southwest -> "Southwest"
        | Southeast -> "Southeast"
        | Up -> "Up"
        | Down -> "Down"
        | Custom dir -> dir

[<CLIMutable>]
type Exit =
    { ExitName : string
      Direction : Direction
      Description : string
      RoomId : string }

[<CLIMutable>]
type Room = 
    { Id : string
      RoomName : string
      /// A brief summary description.
      Brief : string option
      /// A full, detailed description of the room.
      Description : string option
      Exits: Exit list
      Items: ItemInstance list
      Mobs: Mob list }
      // TODO: string properties to hold LUA scripts for events?

[<AutoOpen>]
module RoomUtils =
    let private emptyRoom =
        { Id = ""
          RoomName = ""
          Brief = None
          Description = None
          Exits = []
          Items = []
          Mobs = [] }

    type Room with
        static member Empty = emptyRoom