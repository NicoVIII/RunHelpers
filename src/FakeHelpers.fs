namespace RunHelpers

open Fake.Core

module FakeHelpers =
    [<RequireQualifiedAccess>]
    module Proc =
        let combine res1 f2 =
            match res1 with
            | Ok -> f2 ()
            | Error (code, msg) -> Error(code, msg)

        /// Runs a process created with Fake and turns it into an ProcessResult
        /// Also prints the command
        let runAsJob errorCode (proc: CreateProcess<ProcessResult<unit>>) =
            printfn $"> %s{proc.CommandLine}"

            Proc.run proc
            |> (fun proc ->
                match proc.ExitCode with
                | 0 -> Ok
                | _ -> Error(errorCode, []))
