namespace RunHelpers.Templates

open RunHelpers
open RunHelpers.Shortcuts

type DotNetConfig =
    | Debug
    | Release

[<RequireQualifiedAccess>]
module DotNetConfig =
    let toString =
        function
        | Debug -> "Debug"
        | Release -> "Release"

type DotNetOS =
    | WindowsX64
    | LinuxX64
// macOS single file publishing is weird, that's why it's missing atm

[<RequireQualifiedAccess>]
module DotNetOS =
    let toString =
        function
        | WindowsX64 -> "win-x64"
        | LinuxX64 -> "linux-x64"

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
        dotnet [ "build"; project; "-c"; DotNetConfig.toString config; "--no-restore" ]

    let run project = dotnet [ "run"; "--project"; project ]

    let pack outDir project version =
        dotnet [
            "pack"
            "-c"
            DotNetConfig.toString Release
            "-o"
            outDir
            $"/p:Version=%s{version}"
            project
        ]

    let publishSelfContained outDir project os =
        dotnet [
            "publish"
            project
            "-r"
            DotNetOS.toString os
            "-v"
            "minimal"
            "-c"
            DotNetConfig.toString Release
            "-o"
            outDir
            "--self-contained"
            "/p:PublishSingleFile=true"
            "/p:PublishTrimmed=true"
            "/p:EnableCompressionInSingleFile=true"
            "/p:IncludeNativeLibrariesForSelfExtract=true"
            "/p:DebugType=None"
        ]
