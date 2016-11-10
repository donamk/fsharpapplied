namespace SuaveRestApi.Db

type Person = {
    Id : int
    Name : string
    Age : int
    Email : string
}

module Db =
    open System.Collections.Generic

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
