namespace FsWeb.Controllers

open System.Collections.Generic
open System.Web
open System.Web.Mvc
open System.Net.Http
open System.Web.Http
open System.Linq
open Raven.Client
open FsWeb.Models

type ContactsController() =
    inherit RavenApiController()

    // GET /api/contacts
    member x.Get() = 
        x.RavenSession.Query<Contact>().ToListAsync()

    // POST /api/contacts
    member x.Post ([<FromBody>] contact:Contact) = 
        x.RavenSession.StoreAsync(contact)