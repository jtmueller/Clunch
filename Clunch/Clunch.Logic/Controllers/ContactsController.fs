namespace Clunch.Controllers

open System.Collections.Generic
open System.Web
open System.Web.Mvc
open System.Net.Http
open System.Web.Http
open System.Linq
open Raven.Client
open Raven.Abstractions.Commands
open Clunch.Models

type ContactsController(session) =
    inherit RavenApiController(session)

    // GET /api/contacts
    member x.Get() = 
        (query {
            for contact in session.Query<Contact>() do
            sortBy contact.FirstName 
            select contact           
        }).ToListAsync()

    // POST /api/contacts
    member x.Post ([<FromBody>] contact:Contact) = 
        session.StoreAsync(contact)

    // DELETE
    member x.Delete ([<FromBody>] id:string) =
        session.Advanced.Defer(DeleteCommandData(Key=id))
