namespace RunHelpers

[<AutoOpen>]
module Builder =
    type Delayed<'a> = unit -> 'a
    type DelayedResult = Delayed<JobResult>

    [<AbstractClass>]
    type BaseJobBuilder() =
        member __.Zero() = Ok

    type SequentialJobBuilder() =
        inherit BaseJobBuilder()

        member __.Combine(jobResult: JobResult, job2: DelayedResult) =
            match jobResult with
            | Ok -> job2 ()
            | Error _ -> jobResult

        member __.Delay f : Delayed<'a> = f

        member this.For(collection: 'a seq, evaluation: 'a -> JobResult) =
            let delayedEvaluate value = (fun () -> evaluation value)

            collection
            |> Seq.fold
                (fun beforeResult value -> this.Combine(beforeResult, delayedEvaluate value))
                Ok

        member __.Run(f: DelayedResult) : Job = Job.create f

        member __.Yield x : JobResult = x
        member __.Yield(job: Job) : JobResult = job |> Async.RunSynchronously

    type ParallelJobBuilder() =
        inherit BaseJobBuilder()

        member __.Combine(job1: Job, job2: Job) = [ job2; job1 ]
        member __.Combine(jobList: Job list, job: Job) = job :: jobList
        member __.Delay f = f ()

        member this.For(collection: 'a seq, evaluation: 'a -> Job) =
            collection
            |> Seq.fold (fun (jobList: Job list) value -> this.Combine(jobList, evaluation value)) []

        member __.Run(jobList: Job seq) : Job =
            async {
                let! resultList = Async.Parallel jobList

                return
                    Array.fold
                        (fun combinedResult result ->
                            match combinedResult, result with
                            | Ok, result -> result
                            | combinedResult, Ok -> combinedResult
                            | Error msgList1, Error msgList2 ->
                                Error(List.append msgList1 msgList2))
                        Ok
                        resultList
            }

        member __.Yield job : Job = job

    let job = SequentialJobBuilder()
    let parallelJob = ParallelJobBuilder()
