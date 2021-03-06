﻿#if INTERACTIVE
#r @"..\..\bin\Akka.dll"
#r @"..\..\bin\Akka.FSharp.dll"
#endif

open Akka.FSharp
open Akka.Actor
open System

type Message =
    | Increment
    | Print


let system = ActorSystem.Create "example3"

let spawn_printer system name =
    spawn system name <|
        fun mailbox ->
            let rec loop () =
                actor {
                    let! msg = mailbox.Receive ()
                    printfn "From Child : %s" msg
                    return! loop ()
                }
            loop ()

let actor = spawn system "parent" <| fun mailbox ->

        // Spawning a Child
        let child = spawn_printer mailbox "child" 

        let rec loop () =
            actor {
                let! msg = mailbox.Receive ()
                printfn "From Parent : %s" msg
                child <! msg
                return! loop ()
            }
        loop ()
            

let child = system.ActorSelection("akka://example3/user/parent/child")

child <! "Hello from the child"

let parent = system.ActorSelection("akka://example3/user/parent")

parent <! "Hello from the parent"



    
