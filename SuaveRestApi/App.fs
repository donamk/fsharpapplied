open SuaveRestApi.Rest
open SuaveRestApi.Db
open Suave

[<EntryPoint>]
let main argv =
    let personWebPart = rest "people" {
        GetAll = PersonDb.getPeople
        GetById = PersonDb.getPerson
        Create = PersonDb.createPerson
        Update = PersonDb.updatePerson
        Delete = PersonDb.deletePerson
        UpdateById = PersonDb.updatePersonById
        IsExist = PersonDb.isPersonExist
    }

    startWebServer defaultConfig personWebPart
    0