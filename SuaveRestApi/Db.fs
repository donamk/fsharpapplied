namespace SuaveRestApi.Db

module PersonDb =
    open System.Collections.Generic

    type Person = {
        Id : int
        Name : string
        Age : int
        Email : string
    }

    let private peopleStorage = new Dictionary<int, Person>()
    let getPeople () =
        peopleStorage.Values :> seq<Person>
    
    let createPerson person =
        let id = peopleStorage.Values.Count + 1
        let newPerson = { person with Id = id }
        peopleStorage.Add(id, newPerson)
        newPerson

    let updatePersonById personId personToBeUpdated =
        if peopleStorage.ContainsKey(personId) then
            let updatePerson = { personToBeUpdated with Id = personId }
            peopleStorage.[personId] <- updatePerson
            Some updatePerson
        else
            None
    
    let updatePerson personToBeUpdated =
        updatePersonById personToBeUpdated.Id personToBeUpdated

    let deletePerson personId =
        if peopleStorage.ContainsKey(personId) then
            peopleStorage.Remove(personId) |> ignore

    let getPerson personId =
        if peopleStorage.ContainsKey(personId) then
            Some peopleStorage.[personId]
        else
            None

    let isPersonExist personId =
        peopleStorage.ContainsKey(personId)

module MusicStoreDb =
    open FSharp.Data.Sql
    
    type Album = {
        AlbumId : int
        ArtistId : int
        GenreId : int
        Title : string
        Price : decimal
    }
    
    let [<Literal>] private DbVendor = Common.DatabaseProviderTypes.POSTGRESQL
    let [<Literal>] private ResolutionPath = @"D:\code\FSharpApplied\packages\Npgsql.3.1.8\lib\net451\Npgsql.dll"
    let [<Literal>] ConnectionString = "Host=localhost;Database=musicstore;Username=postgres;Password=postgres"
    let [<Literal>] IndividualAmount = 1000
    let [<Literal>] UseOptionTypes  = true
    
    type private Sql = SqlDataProvider<DbVendor, ConnectionString, "", ResolutionPath, IndividualAmount, UseOptionTypes, CaseSensitivityChange = Common.CaseSensitivityChange.ORIGINAL>
    type DbContext = Sql.dataContext
    type AlbumEntity = DbContext.``public.albumsEntity``

    let private getContext() = Sql.GetDataContext()    
    let firstOrNone s = s |> Seq.tryFind (fun _ -> true)

    let mapToAlbum (albumEntity : AlbumEntity) = {
        AlbumId = albumEntity.Albumid
        ArtistId = albumEntity.Artistid
        GenreId = albumEntity.Genreid
        Title = albumEntity.Title
        Price = albumEntity.Price
    }

    let getAlbums () = 
        getContext().Public.Albums
        |> Seq.map mapToAlbum

    let getAlbumEntityById (ctx : DbContext) id = 
        query { 
            for album in ctx.Public.Albums do
            where (album.Albumid = id)
            select album
        } |> firstOrNone

    let getAlbumById id =
        getAlbumEntityById (getContext()) id |> Option.map mapToAlbum
//
    let createAlbum album =
        let ctx = getContext()
        let album = ctx.Public.Albums.Create(album.ArtistId, album.GenreId, album.Price, album.Title)
        ctx.SubmitUpdates()
        album |> mapToAlbum
//
//    let updateAlbumById id album =
//        let ctx = getContext()
//        let albumEntity = getAlbumEntityById ctx album.AlbumId
//        match albumEntity with
//        | None -> None
//        | Some a ->
//            a.ArtistId <- album.AlbumId
//            a.GenreId <- album.GenreId
//            a.Price <- album.Price
//            a.Title <- album.Title
//            ctx.SubmitUpdates()
//            Some album
//        
//    let updateAlbum album =
//        updateAlbumById album.AlbumId album
//
//
//    let deleteAlbum id =
//        let ctx = getContext()
//        let albumEntity = getAlbumEntityById ctx id
//        match albumEntity with
//        | None -> ()
//        | Some a ->
//            a.Delete()
//            ctx.SubmitUpdates()
//
//    let isAlbumExists id =
//        let ctx = getContext()
//        let albumEntity = getAlbumEntityById ctx id
//        match albumEntity with
//        | None -> false
//        | Some _ -> true
//    
//
