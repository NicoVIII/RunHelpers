namespace RunHelpers

[<RequireQualifiedAccess>]
module Template =
    open BasicShortcuts

    [<RequireQualifiedAccess>]
    module DotNet =
        type Config =
            | Debug
            | Release

        [<RequireQualifiedAccess>]
        module Config =
            let toString =
                function
                | Debug -> "Debug"
                | Release -> "Release"

        let toolRestore () = dotnet [ "tool"; "restore" ]

        let restore project = dotnet [ "restore"; project ]

        let build project config =
            dotnet [ "build"
                     project
                     "-c"
                     Config.toString config
                     "--no-restore" ]

        let run project = dotnet [ "run"; "--project"; project ]

    [<RequireQualifiedAccess>]
    module Paket =
        open System.IO

        let pack (project: string) version =
            let template =
                Path.GetDirectoryName project
                |> (fun folder -> Path.Combine(folder, "paket.template"))

            paket [ "pack"
                    "."
                    "--template"
                    template
                    "--version"
                    version ]

    [<RequireQualifiedAccess>]
    module Npm =
        let install () = npm [ "install" ]

    [<RequireQualifiedAccess>]
    module Pnpm =
        let install () = pnpm [ "install" ]
