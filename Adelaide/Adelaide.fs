module Adelaide.Module

open System
open MathNet.Numerics.Random
open MathNet.Numerics.Statistics
open MathNet.Numerics.LinearAlgebra

let square x = Math.Pow(x, 2.0)

type Par = { X: float Vector; Y: float }
type Resultado = { MSE: float; RMSE: float; W: float Vector; Treinamento: Par seq;  }

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
    List.map (fun par -> erro par w |> square) |>
    List.sum

let squareRootErro t w =
    squareErro t w |>
    Math.Sqrt

let wn t = 
    (t: Par list) |> ignore
    let maxEpoca = 10000
    let rec wn1 t w epoca =
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
        if epoca < maxEpoca && se1 < se0 then wn1 t w1 (epoca + 1) else w1

    let w = vector(Random.doubles t.Head.X.Count)
    wn1 t w 0


let adalineXY range =
    let vx x = vector([1.0; x])
    let treinamento = 
        range |>
        List.map (fun x -> { X = vx x; Y = f2 x |> ruido 2.0 })

    let w1 = wn (List.ofSeq treinamento)
    

    { MSE = squareErro treinamento w1 ; RMSE = squareRootErro treinamento w1; Treinamento = treinamento; W = w1 }

let adalineXYZ range =
    let vx x = vector([1.0; x; x])
    let treinamento = 
        range |>
        List.map (fun x -> { X = vx x; Y = f3 x x |> ruido 10.0 })

    let w1 = wn (List.ofSeq treinamento)

    { MSE = squareErro treinamento w1 ; RMSE = squareRootErro treinamento w1; Treinamento = treinamento; W = w1 }

let realizaXY =
    let range = [0.0 .. 20.0]
    let realizacoes = 
        [0 .. 20] |>
        List.map (fun _ -> adalineXY range)
    let desvio = realizacoes |> List.map(fun r -> r.RMSE) |> Statistics.StandardDeviation
    printfn "\nArtificial 1:\nDesvio padrão: %f" desvio
    
    let min = realizacoes |> List.minBy (fun r -> r.RMSE)
    printfn "%A" min
    min
    

let realizaXYZ =
    let range = [0.0 .. 20.0]
    let realizacoes = 
        [0 .. 20] |>
        List.map (fun _ -> adalineXYZ range)
    
    let desvio = realizacoes |> List.map(fun r -> r.RMSE) |> Statistics.StandardDeviation
    printfn "\nArtificial 2:\nDesvio padrão: %f" desvio
    
    let min = realizacoes |> List.minBy (fun r -> r.RMSE)
    printfn "%A" min
    min

//#load "FSharp.Charting.fsx"
//Chart.Point [ for x in 1.0 .. 10.0 -> (x, f2 x) ]