namespace Suave

module Http =
    type RequestType = GET | POST
    
    type Request = {
        Type : RequestType
        Route : string
    }

    type Response = {
        Content : string
        StatusCode : int
    }

    type Context = {
        Request : Request
        Response : Response
    }
    
    type WebPart = Context -> Async<Context option>

module Successful =
    open Http

    let OK content context =
        { context with Response = {Content = content; StatusCode = 200}}
        |> Some
        |> async.Return

module Console =
    open Http

    let execute inputContext webpart =
        async {
           let! outputContext = webpart inputContext
           match outputContext with
           | Some context ->
               printfn "--------------------"
               printfn "Code : %d" context.Response.StatusCode
               printfn "Output : %s" context.Response.Content
               printfn "--------------------"
           | None -> printfn "No Output" 
        } |> Async.RunSynchronously

    let parseRequest (input: System.String) =
        let parts = input.Split([|';'|])
        let rawType = parts.[0]
        let route = parts.[1]
        match rawType with
        | "GET" -> { Type = GET; Route = route }
        | "POST" -> { Type = POST; Route = route }
        | _ -> failwith "Invalid request"

    let executeInLoop inputContext webpart = 
        let mutable continueLooping = true
        while continueLooping do
            printf "Enter Input Route : "
            let input = System.Console.ReadLine()
            try
                if input = "exit" then
                    continueLooping <- false
                else
                    let context = { inputContext with Request = parseRequest input }
                    execute context webpart
            with
                | ex -> printf "Error: %s" ex.Message

module Combinators =
    // WebPart -> WebPart -> WebPart
    let compose first second context = async {
        let! firstContext = first context
        match firstContext with
        | None -> return None
        | Some context ->
            let! secondContext = second context
            return secondContext
    }

    let (>=>) = compose