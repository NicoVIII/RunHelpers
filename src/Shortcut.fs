namespace RunHelpers

[<AutoOpen>]
module BasicShortcuts =
    open Fake.Core

    let inline cmd rawCmd =
        CreateProcess.fromRawCommand rawCmd >> Job.fromCreateProcess

module Shortcuts =
    let inline dotnet args = cmd "dotnet" args

    // dotnet tooling
    let inline fable args = dotnet [ "fable"; yield! args ]
    let inline paket args = dotnet [ "paket"; yield! args ]

    // Node package manager
    let inline npm args = cmd "npm" args
    let inline pnpm args = cmd "pnpm" args
