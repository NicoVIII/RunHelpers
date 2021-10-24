namespace RunHelpers

open Fake.Core

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

module FakeExtensions =
    [<RequireQualifiedAccess>]
    module CreateProcess =
        let create cmd args = CreateProcess.fromRawCommand cmd args

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

[<AutoOpen>]
module Builder =
    open FakeExtensions

    type JobBuilder() =
        member __.Combine(res1, f2) = Proc.combine res1 f2

        member __.Delay f = f

        member __.For(lst, f) =
            lst
            |> Seq.fold (fun res1 el -> Proc.combine res1 (fun () -> f el)) Ok

        member __.Run f = f ()

        member __.Yield x = x
        member __.Zero() = Ok

    let job = JobBuilder()
