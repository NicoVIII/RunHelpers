namespace RunHelpers

type JobResult =
    | Ok
    | Error of msg: string list

type Job = Async<JobResult>

module Job =
    let errorExitCode = 1

    let ok = async { return Ok }

    let error msgList = async { return Error(msgList) }

    let allowFailure (job: Job) : Job =
        async {
            let! _ = job
            return Ok
        }

    let create f : Job = async { return f () }

    /// Prints error messages and returns exit code
    let execute job =
        let result = Async.RunSynchronously job

        match result with
        | Ok -> 0
        | Error msg ->
            // We print every msg the error has
            List.iter (printfn "%s") msg
            errorExitCode

    open Fake.Core

    /// Creates a job from a FAKE CreateProcess instance
    let fromCreateProcess (proc: CreateProcess<ProcessResult<unit>>) : Job =
        async {
            // TODO: move to allow customization like [parallel]-Prefix
            // explicit \n to improve parallel behaviour
            printfn "> %s" proc.CommandLine

            match (Proc.run proc).ExitCode with
            | 0 -> return Ok
            | _ -> return Error([ "Execution failed!" ])
        }
