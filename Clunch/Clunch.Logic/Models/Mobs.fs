namespace Clunch.Models

open System

[<CLIMutable>]
type MobStats =
    { HitPoints: int }

[<CLIMutable>]
type Character =
    { Id: string
      FirstName: string
      LastName: string
      Stats: MobStats }

[<CLIMutable>]
type NPC =
    { Id: string
      FirstName: string
      LastName: string
      Stats: MobStats }
    // TODO: string properties to hold LUA scripts for events/behavior?

type CreatureInstance =
    { InstanceId: Guid
      TypeId: string
      Name: string
      Description: string
      Stats: MobStats }

type CreatureType =
    { Id: string
      Name: string
      Description: string
      Stats: MobStats }
    // TODO: string properties to hold LUA scripts for events/behavior?
    member x.CreateInstance() =
        { InstanceId = Guid.NewGuid()
          TypeId = x.Id
          Name = x.Name
          Description = x.Description
          Stats = x.Stats }

type Mob =
    | Character of Character
    | NPC of NPC
    | Creature of CreatureInstance