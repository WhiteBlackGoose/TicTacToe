module TableSize

let N = 3

let NLow = N - 1

let WinRow = 3

let AllMoves = Seq.allPairs (seq { 0..NLow }) (seq { 0..NLow })
