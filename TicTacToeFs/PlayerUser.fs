module PlayerUser

open View
open States

let userNextMove (view : NetViewState) (token : State) (table : StateTable) =
    let (x, y) = getUserMoveInteractive view table
    match table.States.[x, y] with
    | Empty -> Some (stateTableWith table x y token)
    | _ -> None
