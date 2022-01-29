namespace RunHelpers.Templates

open RunHelpers.BasicShortcuts

[<RequireQualifiedAccess>]
module Npm =
    let install () = npm [ "install" ]
