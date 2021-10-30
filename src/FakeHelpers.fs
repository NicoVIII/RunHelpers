namespace RunHelpers

open Fake.Core

module FakeHelpers =
    module Proc =
        let combine res1 f2 =
            match res1 with
            | Ok -> f2 ()
            | Error (code, msg) -> Error(code, msg)

        let run (proc: CreateProcess<ProcessResult<unit>>) =
            printfn $"> %s{proc.CommandLine}"

            Proc.run proc
            |> (fun proc ->
                match proc.ExitCode with
                | 0 -> Ok
                | _ -> Error(1, []))
