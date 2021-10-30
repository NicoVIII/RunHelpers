open RunHelpers

[<RequireQualifiedAccess>]
module Config =
    let project = "./src/RunHelpers.fsproj"

module Task =
    let restore () = Template.DotNet.restore Config.project

    let build = Template.DotNet.build Config.project

    let pack = Template.Paket.pack Config.project

module Command =
    let restore () = Task.restore ()

    let build () =
        job {
            restore ()
            Task.build Template.DotNet.Config.Debug
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
        | []
        | [ "build" ] -> Command.build ()
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
