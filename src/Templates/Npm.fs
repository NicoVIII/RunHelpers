namespace RunHelpers.Templates

open RunHelpers.Shortcuts

[<RequireQualifiedAccess>]
module Npm =
    let install () = npm [ "install" ]
