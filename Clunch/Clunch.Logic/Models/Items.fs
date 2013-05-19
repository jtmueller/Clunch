namespace Clunch.Models

open System

type ItemStats =
    { Value: float }

type ItemInstance =
    { InstanceId: Guid
      TypeId: string
      Name: string
      Description: string
      Stats: ItemStats }

// TODO: Types of items (weapons, armor, consumables, etc)

type ItemType =
    { Id: string
      Name: string
      Description: string
      Stats: ItemStats }
    // TODO: some things anyone can create, others only admins can
    member x.CreateInstance() =
        { InstanceId = Guid.NewGuid()
          TypeId = x.Id
          Name = x.Name
          Description = x.Description
          Stats = x.Stats }
