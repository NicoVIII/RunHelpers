namespace RunHelpers

[<RequireQualifiedAccess>]
module Template =
    open BasicShortcuts

    [<RequireQualifiedAccess>]
    module DotNet =
        let restore project = dotnet [ "restore"; project ]

        let build project =
            dotnet [ "build"
                     project
                     "--no-restore" ]

        let run project = dotnet [ "run"; "--project"; project ]

    [<RequireQualifiedAccess>]
    module Npm =
        let install () = npm [ "install" ]

    [<RequireQualifiedAccess>]
    module Pnpm =
        let install () = pnpm [ "install" ]
