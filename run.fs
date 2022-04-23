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

    let watch () =
        let options =
            WatcherOptions.create ()
            |> WatcherOptions.excludeFolders [ "bin"
                                               "obj" ]

        use _ = setupWatcher options [ "src" ] (fun () -> build Debug)

        printfn "Waiting for changes... (enter for exit)"
        Console.ReadLine() |> ignore
        Job.ok

    let pack version =
        DotNet.pack Config.packDir Config.project version

    [<RequireQualifiedAccess>]
    module Docs =
        let build () =
            Shell.cp "./README.md" "docs/index.md"

            dotnet [ "fsdocs"; "build"; "--clean" ]

        let watch () =
            Shell.deleteDir "./tmp"

            Shell.cp "./README.md" "docs/index.md"

            dotnet [ "fsdocs"; "watch" ]

    [<RequireQualifiedAccess>]
    module Template =
        let test () =
            Shell.mkdir Config.templateTest
            Shell.cleanDir Config.templateTest

            job {
                parallelJob {
                    for template in Directory.EnumerateDirectories(Config.templates) do
                        match Path.GetFileName template with
                        | "bin"
                        | "obj" -> ()
                        | _ ->
                            job {
                                // Build
                                DotNet.restore template
                                DotNet.build template Debug

                                // Install
                                dotnet [ "new"
                                         "--uninstall"
                                         template ]
                                |> Job.allowFailure

                                dotnet [ "new"; "--install"; template ]

                                // Use
                                let templateName = Path.GetFileName template

                                dotnet [ "new"
                                         templateName
                                         "-o"
                                         $"{Config.templateTest}/{templateName}" ]
                            }
                }

                DotNet.restore Config.templates
                DotNet.build Config.templates Debug
            }

        let pack version =
            DotNet.pack Config.packDir Config.templates version

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
        | [ "test-templates" ] -> Task.Template.test ()
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
        | [ "pack-templates"; version ] -> Task.Template.pack version
        | [ "build-docs" ] ->
            job {
                Task.restore ()
                Task.build Debug
                Task.Docs.build ()
            }
        | [ "docs" ] ->
            job {
                Task.restore ()
                Task.build Debug
                Task.Docs.watch ()
            }
        // Missing args cases
        | [ "pack" ] ->
            let msg = [ "Usage: dotnet run pack <version>" ]
            Job.error 1 msg
        | [ "pack-templates" ] ->
            let msg =
                [
                    "Usage: dotnet run pack-templates <version>"
                ]

            Job.error 1 msg
        // Default error case
        | _ ->
            let msg =
                [
                    "Usage: dotnet run [<command>]"
                    "Look up available commands in run.fs"
                ]

            Job.error 1 msg
    |> Job.execute
