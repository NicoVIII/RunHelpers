namespace RunHelpers

open System
open System.IO

/// Contains helpers to setup watch commands
module Watch =
    /// Helpertype for multiple FileSystemWatchers
    type FileSystemWatcherList = {
        watchers: FileSystemWatcher list
    } with

        interface IDisposable with
            member this.Dispose() =
                this.watchers |> List.iter (fun watcher -> watcher.Dispose())

    module FileSystemWatcherList =
        let create watchers = { watchers = watchers }

        let combine watcherLists =
            watcherLists |> List.collect (fun lst -> lst.watchers) |> create

    type WatcherOptions = {
        includeSubdirectories: bool
        excludeFolders: string list
    }

    module WatcherOptions =
        let create () = {
            includeSubdirectories = true
            excludeFolders = [ "bin"; "obj" ]
        }

        let excludeFolders folders options = {
            options with
                excludeFolders = List.append options.excludeFolders folders |> List.distinct
        }

        let excludeFolder folder = excludeFolders [ folder ]

        let withoutSubdirectories options = {
            options with
                includeSubdirectories = false
        }

    let setupWatcher options folders onChange =
        let mutable working = false
        let mutable changedWhileWorking = false

        let disable (watcher: FileSystemWatcher) = watcher.EnableRaisingEvents <- false
        let enable (watcher: FileSystemWatcher) = watcher.EnableRaisingEvents <- true

        let watchers =
            folders
            |> List.map (fun folder ->
                let watcher = new FileSystemWatcher(folder)
                watcher.IncludeSubdirectories <- options.includeSubdirectories
                watcher)

        // Does the work necessary after a change
        let rec work () =
            working <- true
            onChange () |> ignore

            if changedWhileWorking then
                changedWhileWorking <- false
                printfn "- Do it again, there were changes while running"
                work ()
            else
                printfn "- Waiting for changes... (enter to exit)"
                working <- false

        // Handles events, debounces them and filters them
        let handler (args: FileSystemEventArgs) =
            let filtered = options.excludeFolders |> List.exists (args.FullPath.Contains)

            if not filtered then
                List.iter disable watchers

                let working = async { if working then changedWhileWorking <- true else work () }

                let debouncer =
                    async {
                        do! Async.Sleep(500)
                        List.iter enable watchers
                    }

                [ working; debouncer ]
                |> Async.Parallel
                |> Async.Ignore
                |> Async.RunSynchronously

        // Register handler
        List.iter
            (fun (watcher: FileSystemWatcher) ->
                watcher.Changed.Add handler
                watcher.Created.Add handler
                watcher.Deleted.Add handler)
            watchers

        // Enable Watchers
        List.iter enable watchers

        FileSystemWatcherList.create watchers
