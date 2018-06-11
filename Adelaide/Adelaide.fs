module Adelaide

open MathNet.Numerics.Random
open FSharp.Charting
open MathNet.Numerics.LinearAlgebra

// Saiba mais sobre F# em http://fsharp.org
// Veja o projeto 'F# Tutorial' para obter mais ajuda.

type Par = { X: float Vector; Y: float }

let noise x = x + 3.0*Random.shared.NextDouble() - 0.5

let f2 x =
    2.0*x + 1.0

let f3 x y = 
    2.0*x + 3.0*y + 1.0

let f2n = 
    f2 >> noise

let f3n x y = 
    f3 x y |> noise

let saida w x =
    (w: float Vector) |> ignore
    (x: float Vector) |> ignore
    w .* x |> Vector.sum

//#load "FSharp.Charting.fsx"
//Chart.Line [ for x in 1.0 .. 100.0 -> (x, x ** 2.0) ]