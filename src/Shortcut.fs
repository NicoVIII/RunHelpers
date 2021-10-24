namespace RunHelpers

module BasicShortcuts =
    module Internal =
        open FakeExtensions

        let inline buildBasicCommand cmd = CreateProcess.create cmd >> Proc.run

    let dotnet args =
        Internal.buildBasicCommand "dotnet" args

    let npm args = Internal.buildBasicCommand "npm" args

    let pnpm args = Internal.buildBasicCommand "pnpm" args
