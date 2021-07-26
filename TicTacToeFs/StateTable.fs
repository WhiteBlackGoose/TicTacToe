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
