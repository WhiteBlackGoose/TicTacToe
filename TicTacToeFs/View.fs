module View

open System
open TableSize

[<Struct>]
type Position =
    { X : int;
      Y : int; }

type NetViewState =
    { Position : Position; }

let paint (view : NetViewState) tableGetter showCursor =
    let read (x : int) (y : int) =
        if showCursor && { X = x; Y = y } = view.Position then
            "@"
        else
            match tableGetter x y with
            | State.X -> "X"
            | State.O -> "Y"
            | _ -> " "

    Console.Clear ()
    for x in 0..NLow do
        printf "|"
        for y in 0..NLow do
            printf $"|{read(x, y)}"


let rec getUserMoveInteractive view table =
    let readKey = Console.ReadKey ().Key
    if readKey is ConsoleKey.DownArrow then
        paint view table