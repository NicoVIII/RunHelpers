namespace RunHelpers

[<AutoOpen>]
module Builder =
    open FakeHelpers

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
