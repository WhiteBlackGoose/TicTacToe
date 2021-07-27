module States

open TableSize

[<Struct>]
type State = Empty | X | O

let nextTurn = function
    | X -> O
    | O -> X
    | _ -> raise (System.Exception ())

[<Struct>]
type StateTable = { States : State[,]; }

let stateTableWith (table : StateTable) x y state =
    let newStates = table.States.Clone() :?> State[,]
    newStates.[x, y] <- state
    { table with States = newStates }

let emptyStateTable = { States = Array2D.create N N State.Empty; }

type WinRowShape =
    { StepX : int;
      StepY : int;
      RangeX : seq<int>;
      RangeY : seq<int>; }

let steps = [
        { StepX = 1; StepY = 0; RangeX = seq { 0..(N - WinRow) }; RangeY = seq { 0..NLow } };
        { StepX = 0; StepY = 1; RangeX = seq { 0..NLow }; RangeY = seq { 0..(N - WinRow) } };
        { StepX = 1; StepY = 1; RangeX = seq { 0..(N - WinRow) }; RangeY = seq { 0..(N - WinRow) } };
        { StepX = 1; StepY = -1; RangeX = seq { 0..(N - WinRow) }; RangeY = seq { (WinRow - 1)..NLow } };
    ]
        
let won (table : StateTable) state =
    let rec isBeginOfWinRow i x y stepX stepY =
        i = WinRow
        || table.States.[x, y] = state
           && isBeginOfWinRow (i + 1) (x + stepX) (y + stepY) stepX stepY
            

    let thisShapedWinRowExists stepX stepY rangeX rangeY =
        Seq.exists (fun (x, y) -> isBeginOfWinRow 0 x y stepX stepY) (Seq.allPairs rangeX rangeY)

    steps |>
    Seq.exists (fun step -> thisShapedWinRowExists step.StepX step.StepY step.RangeX step.RangeY)
    

let isFull (table : StateTable) =
    Seq.forall (fun c -> c <> Empty) (Seq.map (fun (x, y) -> table.States.[x, y]) AllMoves)
