namespace RunHelpers

[<AutoOpen>]
module Builder =
    type JobBuilder(combine) =
        member __.Combine(process1, process2) = combine process1 process2

        member __.Delay f = f ()

        member __.For(lst, f) =
            lst
            |> Seq.fold (fun f1 el -> combine f1 (f el)) Job.ok

        member __.Run f = f

        member __.Yield x : Job = x
        member __.Zero() : Job = Job.ok

    let job = JobBuilder(Job.combineSequential)
    let parallelJob = JobBuilder(Job.combineParallel Constant.errorExitCode)
