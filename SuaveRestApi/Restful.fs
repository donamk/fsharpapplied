namespace SuaveRestApi.Rest

open Newtonsoft.Json
open Newtonsoft.Json.Serialization
open Suave
open Suave.Operators
open Suave.Successful
open Suave.Filters

[<AutoOpen>]
module Restful =
    open Suave.RequestErrors

    type RestResource<'a> = {
        GetAll : unit -> 'a seq
        GetById : int -> 'a option
        Create : 'a -> 'a
        Update : 'a -> 'a option
        UpdateById : int -> 'a -> 'a option
        Delete : int -> unit
        IsExist: int -> bool
    }

    let JSON v =
        let settings =  new JsonSerializerSettings()
        settings.ContractResolver <- new CamelCasePropertyNamesContractResolver()

        JsonConvert.SerializeObject(v, settings)
        |> OK >=> Writers.setMimeType "application/json; charset=utf-8"

    let fromJson<'a> json =
        JsonConvert.DeserializeObject(json, typeof<'a>) :?> 'a

    let getResourceFromReq<'a> (req : HttpRequest) =
        let getString rawForm =
            System.Text.Encoding.UTF8.GetString(rawForm)
        req.rawForm |> getString |> fromJson<'a>

    
    let rest resourceName resource =
        let resourcePath = "/" + resourceName
        let resourceIdPath =
            let path = resourcePath + "/%d"
            new PrintfFormat<(int -> string), unit, string, string, int>(path)
        let getAll = warbler (fun _ -> resource.GetAll () |> JSON)
        let badRequest = BAD_REQUEST "Resource not found"
        let handleResource requestError = function
            | Some r -> r |> JSON
            | _ -> requestError
        let deleteResourceById id =
            resource.Delete id
            NO_CONTENT
        let getResourceById =
            resource.GetById >> handleResource (NOT_FOUND "Resource not found") 
        let updateResourceById id =
            request (getResourceFromReq >> (resource.UpdateById id) >> handleResource badRequest)
        let isResourceExists id =
            if resource.IsExist id then
                OK ""
            else
                NOT_FOUND ""


        choose [
            path resourcePath >=> choose [
                GET >=> getAll
                POST >=> request (getResourceFromReq >> resource.Create >> JSON)
                PUT >=> request (getResourceFromReq >>
                                    resource.Update >> handleResource badRequest)
            ]
            DELETE >=> pathScan resourceIdPath deleteResourceById
            GET >=> pathScan resourceIdPath getResourceById
            PUT >=> pathScan resourceIdPath updateResourceById
            HEAD >=> pathScan resourceIdPath isResourceExists
        ]