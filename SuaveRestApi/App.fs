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

    let albumWebPart = rest "albums" {
        GetAll = MusicStoreDb.getAlbums
        GetById = MusicStoreDb.getAlbumById
        Create = MusicStoreDb.createAlbum
        Update = MusicStoreDb.updateAlbum
        UpdateById = MusicStoreDb.updateAlbumById
        Delete = MusicStoreDb.deleteAlbum
        IsExist = MusicStoreDb.isAlbumExists
    }

    startWebServer defaultConfig (choose [personWebPart; albumWebPart])
    0