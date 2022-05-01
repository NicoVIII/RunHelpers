namespace RunHelpers.Templates

open RunHelpers.Shortcuts

[<RequireQualifiedAccess>]
module Pnpm =
    let install () = pnpm [ "install" ]
