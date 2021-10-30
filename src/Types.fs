namespace RunHelpers

type ProcessResult =
    | Ok
    | Error of exitCode: int * msg: string list

module ProcessResult =
    /// Prints error messages and returns exit code
    let wrapUp pRes =
        match pRes with
        | Ok -> 0
        | Error (x, msg) ->
            // We print every msg the error has
            List.iter (printfn "%s") msg
            x
