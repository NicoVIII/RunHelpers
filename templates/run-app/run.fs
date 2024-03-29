open Fake.IO

open RunHelpers
open RunHelpers.Templates

[<RequireQualifiedAccess>]
module Config =
    let projectName = "REPLACE"

    let mainProject = $"REPLACE.fsproj"

    let testProject = $"REPLACE.fsproj"

    let artifactName = "REPLACE"

    let publishPath = "./publish"

module Task =
    let restore () =
        DotNet.restoreWithTools Config.mainProject

    let build () = DotNet.build Config.mainProject Debug
    let run () = DotNet.run Config.mainProject
    let runTest () = DotNet.run Config.testProject

    let publish () =
        let publish = DotNet.publishSelfContained Config.publishPath Config.mainProject

        Shell.mkdir Config.publishPath
        Shell.cleanDir Config.publishPath

        job {
            publish LinuxX64

            Shell.mv
                $"%s{Config.publishPath}/%s{Config.projectName}"
                $"%s{Config.publishPath}/%s{Config.artifactName}-linux-x64"

            publish WindowsX64

            Shell.mv
                $"{Config.publishPath}/%s{Config.projectName}.exe"
                $"{Config.publishPath}/%s{Config.artifactName}-win-x64.exe"
        }

[<EntryPoint>]
let main args =
    args
    |> List.ofArray
    |> function
        | [ "restore" ] -> Task.restore ()
        | [ "build" ] ->
            job {
                Task.restore ()
                Task.build ()
            }
        | []
        | [ "run" ] ->
            job {
                Task.restore ()
                Task.run ()
            }
        | [ "test" ] ->
            job {
                Task.restore ()
                Task.runTest ()
            }
        | [ "publish" ] ->
            job {
                Task.restore ()
                Task.publish ()
            }
        | _ -> Job.error [ "Usage: dotnet run [<command>]"; "Look up available commands in run.fs" ]
    |> Job.execute
