open SuaveRestApi.Rest
open SuaveRestApi.Db
open Suave

[<EntryPoint>]
let main argv =
    let personWebPart = rest "people" {
        GetAll = Db.getPeople
        GetById = Db.getPerson
        Create = Db.createPerson
        Update = Db.updatePerson
        Delete = Db.deletePerson
        UpdateById = Db.updatePersonById
        IsExist = Db.isPersonExist
    }

    startWebServer defaultConfig personWebPart
    0