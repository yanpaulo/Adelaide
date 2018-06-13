module Adelaide

open System
open FSharp.Charting
open MathNet.Numerics.Random
open MathNet.Numerics.LinearAlgebra

// Saiba mais sobre F# em http://fsharp.org
// Veja o projeto 'F# Tutorial' para obter mais ajuda.

type Par = { X: float Vector; Y: float }

let noise = Random.doubles 20 |> Array.map (fun n -> (n - 0.5) * 2.0)

let f2 x =
    x + noise.[int x]

let saida w x =
    (w: float Vector) |> ignore
    (x: float Vector) |> ignore
    w .* x |> Vector.sum

let erro par w =
    par.Y - (saida w par.X)

let square x = Math.Pow(x, 2.0)

let sqErro t w =
    t |>
    List.map (fun t -> t.Y - saida w t.X |> square) |>
    List.sum

let wn t = 
    (t: Par list) |> ignore
    let rec wn1 t w =
        let rec wn2 t w e =
            match t with
            | par :: tail  ->
                let w1 = w + 0.001 * e * par.X
                let e1 =  erro par w1
                wn2 tail w1 e1
            | _ -> w
        let se0 = sqErro t w
        let e = erro t.Head w
        let w1 = wn2 t w e
        let se1 = sqErro t w1
        if se1 < se0 then wn1 t w1 else w1

    let w = vector(Random.doubles t.Head.X.Count)
    wn1 t w

let adelaideF2 =
    let vx x = vector([1.0; x])
    let treinamento = 
        [ 0.0 .. 15.0 ] |>
        Seq.map (fun x -> { X = vx x; Y = f2 x })
    let w1 = wn (List.ofSeq treinamento)

    let point = Chart.Point [ for x in 1.0 .. 15.0 -> (x, f2 x) ]
    let line = Chart.Line [ for x in 1.0 .. 15.0 -> (x, saida w1 (vx x)) ]

    let combine = Chart.Combine [point; line]
    combine.ShowChart() |> ignore
    0

//#load "FSharp.Charting.fsx"
//Chart.Point [ for x in 1.0 .. 10.0 -> (x, f2 x) ]