module TableSize

let N = 4

let NLow = N - 1

let WinRow = 4

let AllMoves = Seq.allPairs (seq { 0..NLow }) (seq { 0..NLow })