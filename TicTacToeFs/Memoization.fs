module Memoization

open System.Collections.Generic

let memoize f =
    let cache = Dictionary<_, _>()
    fun c ->
        match cache.TryGetValue (c) with
        | (true, res) -> res
        | _ -> 
            let res = f c
            cache.[c] <- res
            res