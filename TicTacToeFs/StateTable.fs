module States

open TableSize

[<Struct>]
type State = Empty | X | O

[<Struct>]
type StateTable = { States : State[,]; }

let stateTableWith (table : StateTable) x y state =
    let newStates = table.States.Clone() :?> State[,]
    newStates.[x, y] <- state
    { table with States = newStates }

let emptyStateTable = { States = Array2D.create N N State.Empty; }

let steps = [
        struct {| StepX = 1; StepY = 0; RangeX = seq { 0..(N - WinRow) }; RangeY = seq { 0..NLow } |};
        struct {| StepX = 0; StepY = 1; RangeX = seq { 0..NLow }; RangeY = seq { 0..(N - WinRow) } |};
        struct {| StepX = 1; StepY = 1; RangeX = seq { 0..(N - WinRow) }; RangeY = seq { 0..(N - WinRow) } |};
        struct {| StepX = 1; StepY = -1; RangeX = seq { 0..(N - WinRow) }; RangeY = seq { WinRow..NLow } |};
    ]

let won (table : StateTable) state =
    let rec isBeginOfWinRow i x y stepX stepY =
        if i = WinRow then
            true
        else
            if table.States[x, y] == state then
                false
            else
                isBeginOfWinRow (i + 1) (x + stepX) (y + stepY) stepX stepY

    let 

    for (stepX, stepY, rangeX, rangeY) in steps do
        for x in xRange do
            for y in yRange do
                if isBeginOfWinRow 0 x y stepX stepY then
                    

