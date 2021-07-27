module Bot

open TableSize
open States
open Memoization


let rec notLoseIfMakeThisMove (table : StateTable) (x : int) (y : int) (token : State) =
    (table, x, y, token) |> memoize (fun (table, x, y, token) ->
        let newTable = stateTableWith table x y token

        won newTable token
        || isFull newTable
        || AllMoves
            |> Seq.exists (fun (x, y) ->
                newTable.States.[x, y] = Empty
                && winIfMakeThisMove newTable x y (nextTurn token)
            )
            |> not
    )


and winIfMakeThisMove (table : StateTable) (x : int) (y : int) (token : State) =
    (table, x, y, token) |> memoize (fun (table, x, y, token) ->
        let newTable = stateTableWith table x y token

        not (isFull newTable)
        && (won newTable token
            || AllMoves
            |> Seq.exists (fun (x, y) ->
                newTable.States.[x, y] = Empty
                && winIfMakeThisMove newTable x y (nextTurn token)
            )
            |> not)
    )


let botNextMove (table : StateTable) token =
    let moveLeadToSelector strategy (x, y) =
        if table.States.[x, y] = token && strategy table x y token then
            Some(x, y)
        else
            None

    match Seq.tryPick (moveLeadToSelector winIfMakeThisMove) AllMoves with
    | Some(x, y) -> Some(x, y)
    | None -> Seq.tryPick (moveLeadToSelector notLoseIfMakeThisMove) AllMoves
