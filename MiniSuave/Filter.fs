namespace Suave

module Filters =
    open Http

    let iff condition context =
        if condition context then
            context |> Some |> async.Return
        else
            None |> async.Return

    let GET = iff (fun context -> context.Request.Type = GET)
    let POST = iff (fun context -> context.Request.Type = POST)
    let Path path = iff (fun context -> context.Request.Route = path)

    let rec Choose webparts context = async {
        match webparts with
        | [] -> return None
        | x :: xs ->
            let! result = x context
            match result with
            | Some x -> return Some x
            | None -> return! Choose xs context
    }