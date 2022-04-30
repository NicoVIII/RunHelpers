namespace RunHelpers

module BasicShortcuts =
    module Internal =
        open Fake.Core

        let inline basicCommand cmd =
            CreateProcess.fromRawCommand cmd
            >> Job.fromCreateProcess

    let dotnet args = Internal.basicCommand "dotnet" args

    let paket args =
        Internal.basicCommand "dotnet" [ "paket"; yield! args ]

    let npm args = Internal.basicCommand "npm" args

    let pnpm args = Internal.basicCommand "pnpm" args
