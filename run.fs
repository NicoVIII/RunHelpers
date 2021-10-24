open RunHelpers

[<RequireQualifiedAccess>]
module Config =
    let project = "./src/RunHelpers.fsproj"

module Task =
    let restore () = Template.DotNet.restore Config.project

    let build () = Template.DotNet.build Config.project

module Command =
    let restore () = Task.restore ()

    let build () =
        job {
            restore ()
            Task.build ()
        }

[<EntryPoint>]
let main args =
    args
    |> List.ofArray
    |> function
        | [ "restore" ] -> Command.restore ()
        | []
        | [ "build" ] -> Command.build ()
        | _ ->
            let msg =
                [ "Usage: dotnet run [<command>]"
                  "Look up available commands in run.fs" ]

            Error(1, msg)
    |> ProcessResult.wrapUp
