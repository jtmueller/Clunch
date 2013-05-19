namespace Clunch.Models

[<CLIMutable>]
type CharRef =
    { Id: string
      FirstName: string
      LastName: string }

[<CLIMutable>]
type User =
    { Id: string
      Username: string
      Email: string
      PWHash: byte[]
      PWSalt: byte[]
      IsAdmin: bool
      Characters: CharRef list }
