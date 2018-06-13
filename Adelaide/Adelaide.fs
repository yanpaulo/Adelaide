module Adelaide.Module

open System
open MathNet.Numerics.Random
open MathNet.Numerics.LinearAlgebra

let square x = Math.Pow(x, 2.0)

type Par = { X: float Vector; Y: float }

let ruido rate x = 
    x + rate * (Random.shared.NextDouble() - 0.5)

let f2 x =
    x

let f3 x y =
    x - y

let saida w x =
    w .* x |> Vector.sum

let erro par w =
    par.Y - (saida w par.X)

let squareErro t w =
    t |>
    List.map (fun t -> t.Y - saida w t.X |> square) |>
    List.sum

let squareRootErro t w =
    squareErro t w |>
    Math.Sqrt

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
        let se0 = squareErro t w
        let e = erro t.Head w
        let w1 = wn2 t w e
        let se1 = squareErro t w1
        if se1 < se0 then wn1 t w1 else w1

    let w = vector(Random.doubles t.Head.X.Count)
    wn1 t w


let adalineXY =
    let vx x = vector([1.0; x])
    let range = [ 0.0 .. 20.0 ]
    let treinamento = 
        range |>
        List.map (fun x -> { X = vx x; Y = f2 x |> ruido 2.0 })

    let w1 = wn (List.ofSeq treinamento)

    let pointData = 
        treinamento |> 
        List.map (fun par -> ((par.X |> Seq.last), par.Y))


    let lineData = 
        range |>
        List.map (fun x -> (x, saida w1 (vx x)))
    (pointData, lineData)

let adalineXYZ =
    let vx x = vector([1.0; x; x])
    let range = [ 0.0 .. 20.0 ]
    let treinamento = 
        range |>
        List.map (fun x -> { X = vx x; Y = f3 x x |> ruido 20.0 })

    let w1 = wn (List.ofSeq treinamento)

    (Seq.ofList treinamento, w1)



//#load "FSharp.Charting.fsx"
//Chart.Point [ for x in 1.0 .. 10.0 -> (x, f2 x) ]