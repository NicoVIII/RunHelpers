open System

open RunHelpers
open RunHelpers.Watch

[<RequireQualifiedAccess>]
module Config =
    let project = "./src/RunHelpers.fsproj"

module Task =
    let restore () =
        job {
            Template.DotNet.toolRestore ()
            Template.DotNet.restore Config.project
        }

    let build = Template.DotNet.build Config.project

    let watch () =
        let options =
            WatcherOptions.create ()
            |> WatcherOptions.excludeFolders [ "bin"
                                               "obj" ]

        use watcher =
            setupWatcher options [ "src" ] (fun () -> build Template.DotNet.Config.Debug)

        printfn "Waiting for changes... (enter for exit)"
        Console.ReadLine() |> ignore
        Ok

    let pack = Template.Paket.pack Config.project

module Command =
    let restore () = Task.restore ()

    let subbuild () = Task.build Template.DotNet.Config.Debug

    let build () =
        job {
            restore ()
            subbuild ()
        }

    let subwatch () = Task.watch ()

    let watch () =
        job {
            restore ()
            subwatch ()
        }

    let pack version =
        job {
            restore ()
            Task.build Template.DotNet.Config.Release
            Task.pack version
        }

[<EntryPoint>]
let main args =
    args
    |> List.ofArray
    |> function
        | [ "restore" ] -> Command.restore ()
        | [ "subbuild" ] -> Command.subbuild ()
        | []
        | [ "build" ] -> Command.build ()
        | [ "subwatch" ] -> Command.subwatch ()
        | [ "watch" ] -> Command.watch ()
        | [ "pack"; version ] -> Command.pack version
        // Missing args cases
        | [ "pack" ] ->
            let msg = [ "Usage: dotnet run pack <version>" ]
            Error(1, msg)
        // Default error case
        | _ ->
            let msg =
                [ "Usage: dotnet run [<command>]"
                  "Look up available commands in run.fs" ]

            Error(1, msg)
    |> ProcessResult.wrapUp
