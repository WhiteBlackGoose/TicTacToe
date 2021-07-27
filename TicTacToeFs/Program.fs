open View
open PlayerUser
open PlayerBot
open States

let view = { Position = { X = 0; Y = 0; } }
let board = emptyStateTable

let rec nextMove nextMove1 nextMove2 (board : StateTable) =
    paint view (fun x y -> board.States.[x, y]) true
    match nextMove1 board with
    | Some newBoard ->
        if won newBoard X then
            printf "X won!"
        else if won newBoard O then
            printf "O won!"
        else if isFull newBoard then
            printf "Draw."
        else
            nextMove nextMove2 nextMove1 board
    | None ->
        printf "Can't make a move"

nextMove (userNextMove view X) (botNextMove O) board