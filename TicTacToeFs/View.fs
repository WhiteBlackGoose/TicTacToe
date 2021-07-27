module View

open System
open TableSize
open States

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
            | X -> "X"
            | O -> "O"
            | _ -> " "

    Console.Clear ()
    for x in 0..NLow do
        printf "|"
        for y in 0..NLow do
            printf $"{read x y}|"
        printf "\n"


let rec getUserMoveInteractive (view : NetViewState) (table : StateTable) =
    let limitPosition (x : int, y : int) =
        let limitAxis = function
            | tooLow when tooLow < 0 -> 0
            | tooHigh when tooHigh > NLow -> NLow
            | rest -> rest
        { X = limitAxis x; Y = limitAxis y }

    paint view (fun x y -> table.States.[x, y]) true

    let cx, cy = view.Position.X, view.Position.Y

    let key = (Console.ReadKey ()).Key

    let potentiallyNewPos = 
        match key with
        | ConsoleKey.DownArrow -> Some(cx + 1, cy)
        | ConsoleKey.UpArrow -> Some(cx - 1, cy)
        | ConsoleKey.RightArrow -> Some(cx, cy + 1)
        | ConsoleKey.LeftArrow -> Some(cx, cy - 1)
        | _ -> None

    match potentiallyNewPos with
    | Some(x, y) -> getUserMoveInteractive { Position = limitPosition (x, y) } table
    | None -> 
        match key with
        | ConsoleKey.Spacebar | ConsoleKey.Enter -> (cx, cy)
        | _ -> getUserMoveInteractive view table