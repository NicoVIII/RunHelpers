namespace RunHelpers.Templates

open RunHelpers.Shortcuts

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
