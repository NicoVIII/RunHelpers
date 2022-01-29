namespace RunHelpers.Templates

open RunHelpers.BasicShortcuts

[<RequireQualifiedAccess>]
module Pnpm =
    let install () = pnpm [ "install" ]
