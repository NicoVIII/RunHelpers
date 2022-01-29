namespace RunHelpers.Templates

open RunHelpers
open RunHelpers.BasicShortcuts

type DotNetConfig =
    | Debug
    | Release

[<RequireQualifiedAccess>]
module DotNetConfig =
    let toString =
        function
        | Debug -> "Debug"
        | Release -> "Release"

[<RequireQualifiedAccess>]
module DotNet =
    let toolRestore () = dotnet [ "tool"; "restore" ]

    let restore project = dotnet [ "restore"; project ]

    let restoreWithTools project =
        job {
            toolRestore ()
            restore project
        }

    let build project config =
        dotnet [ "build"
                 project
                 "-c"
                 DotNetConfig.toString config
                 "--no-restore" ]

    let run project = dotnet [ "run"; "--project"; project ]
