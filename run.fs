open System

open RunHelpers
open RunHelpers.Templates
open RunHelpers.Watch

[<RequireQualifiedAccess>]
module Config =
    let project = "./src/RunHelpers.fsproj"
    let packDir = "./pack"

module Task =
    let restore () = DotNet.restoreWithTools Config.project
    let build = DotNet.build Config.project

    let watch () =
        let options =
            WatcherOptions.create ()
            |> WatcherOptions.excludeFolders [ "bin"
                                               "obj" ]

        use _ = setupWatcher options [ "src" ] (fun () -> build Debug)

        printfn "Waiting for changes... (enter for exit)"
        Console.ReadLine() |> ignore
        Ok

    let pack version =
        DotNet.pack Config.packDir Config.project version

[<EntryPoint>]
let main args =
    args
    |> List.ofArray
    |> function
        | [ "restore" ] -> Task.restore ()
        | [ "subbuild" ] -> Task.build Debug
        | []
        | [ "build" ] ->
            job {
                Task.restore ()
                Task.build Debug
            }
        | [ "subwatch" ] -> Task.watch ()
        | [ "watch" ] ->
            job {
                Task.restore ()
                Task.watch ()
            }
        | [ "pack"; version ] ->
            job {
                Task.restore ()
                Task.build Release
                Task.pack version
            }
        // Missing args cases
        | [ "pack" ] ->
            let msg = [ "Usage: dotnet run pack <version>" ]
            Error(1, msg)
        // Default error case
        | _ ->
            let msg =
                [
                    "Usage: dotnet run [<command>]"
                    "Look up available commands in run.fs"
                ]

            Error(1, msg)
    |> ProcessResult.wrapUp
