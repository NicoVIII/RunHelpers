open System
open System.IO

open Fake.IO

open RunHelpers
open RunHelpers.BasicShortcuts
open RunHelpers.Templates
open RunHelpers.Watch

[<RequireQualifiedAccess>]
module Config =
    let project = "./src/RunHelpers.fsproj"
    let packDir = "./pack"
    let templates = "./templates"
    let templateTest = "./test-templates"

module Task =
    let restore () = DotNet.restoreWithTools Config.project
    let build config = DotNet.build Config.project config

    let testTemplates () =
        Shell.mkdir Config.templateTest
        Shell.cleanDir Config.templateTest

        job {
            for template in Directory.EnumerateDirectories Config.templates do
                // Build
                DotNet.build template Debug

                // Install
                dotnet [ "new"
                         "--uninstall"
                         template ]
                |> ignore

                dotnet [ "new"; "--install"; template ]

                // Use
                let templateName = Path.GetFileName template

                dotnet [ "new"
                         templateName
                         "-o"
                         $"{Config.templateTest}/{templateName}" ]
        }

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
        | []
        | [ "build" ] ->
            job {
                Task.restore ()
                Task.build Debug
            }
        | [ "test-templates" ] ->
            job {
                Task.restore ()
                Task.testTemplates ()
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
