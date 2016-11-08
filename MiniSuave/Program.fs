module MiniSuave

open Suave.Http
open Suave.Console
open Suave.Successful
open Suave.Combinators
open Suave.Filters

[<EntryPoint>]
let main argv = 
    let request = 
        { Type = Suave.Http.GET
          Route = "" }
    
    let response = 
        { Content = ""
          StatusCode = 200 }
    
    let context = 
        { Request = request
          Response = response }
    
    //    execute context (OK "Hello Suave!")
    //    executeInLoop context (GET >=> Path "/hello" >=> OK "Hello Suave!")
    let app = 
        Choose [ GET >=> Path "/hello" >=> OK "Hello GET"
                 POST >=> Path "/hello" >=> OK "Hello POST"
                 Path "/foo" >=> Choose [ GET >=> OK "Foo GET"
                                          POST >=> OK "Foo POST" ] ]
    
    executeInLoop context app
    0
