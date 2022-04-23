namespace RunHelpers

type JobResult =
    | Ok
    | Error of exitCode: int * msg: string list

type Job = Async<JobResult>

module Job =
    let ok = async { return Ok }

    let error exitCode msgList =
        async { return Error(exitCode, msgList) }

    let allowFailure (job: Job) : Job =
        async {
            let! _ = job
            return Ok
        }

    /// Combines jobs sequentially. If the first job suceeds, the second is run,
    /// otherwise it returns a Error JobResult
    let combineSequential job1 job2 : Job =
        async {
            let! result1 = job1

            match result1 with
            | Ok -> return! job2
            | Error (code, msg) -> return Error(code, msg)
        }

    let combineParallelMany multiErrorExitCode jobList =
        async {
            let! results = Async.Parallel jobList

            return
                Array.fold
                    (fun totalResult jobResult ->
                        match totalResult, jobResult with
                        | Ok, Ok -> Ok
                        | Error (exitCode, msgList), Ok
                        | Ok, Error (exitCode, msgList) -> Error(exitCode, msgList)
                        | Error (_, msgList1), Error (_, msgList2) ->
                            Error(multiErrorExitCode, List.append msgList1 msgList2))
                    Ok
                    results
        }

    let combineParallel multiErrorExitCode job1 job2 : Job =
        combineParallelMany multiErrorExitCode [ job1; job2 ]

    /// Prints error messages and returns exit code
    let execute job =
        let result = Async.RunSynchronously job

        match result with
        | Ok -> 0
        | Error (x, msg) ->
            // We print every msg the error has
            List.iter (printfn "%s") msg
            x

    open Fake.Core

    /// Creates a job from a FAKE CreateProcess instance
    let fromCreateProcess errorCode (proc: CreateProcess<ProcessResult<unit>>) : Job =
        async {
            printfn $"> %s{proc.CommandLine}"

            match (Proc.run proc).ExitCode with
            | 0 -> return Ok
            | _ -> return Error(errorCode, [])
        }
