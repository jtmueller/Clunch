namespace FsWeb.Models

// Web API model binding expects private backing fields that match the JSON. Can we make this more flexible?

type Contact() = 
    let mutable id = ""
    let mutable firstName = ""
    let mutable lastName = ""
    let mutable phone = ""
    member x.Id with get() = id and set v = id <- v
    member x.FirstName with get() = firstName and set v = firstName <- v
    member x.LastName with get() = lastName and set v = lastName <- v
    member x.Phone with get() = phone and set v = phone <- v

