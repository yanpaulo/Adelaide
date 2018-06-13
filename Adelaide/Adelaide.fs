module Adelaide.Module

open System
open MathNet.Numerics.Random
open MathNet.Numerics.LinearAlgebra

// Saiba mais sobre F# em http://fsharp.org
// Veja o projeto 'F# Tutorial' para obter mais ajuda.

type Par = { X: float Vector; Y: float }

let noise rate x = 
    x + rate * (Random.shared.NextDouble() - 0.5)

let f2 x =
    x

let f3 x y =
    x + y

let saida w x =
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
    let range = [ 0.0 .. 20.0 ]
    let treinamento = 
        range |>
        List.map (fun x -> { X = vx x; Y = f2 x |> noise 2.0 })

    let w1 = wn (List.ofSeq treinamento)

    let pointData = 
        treinamento |> 
        List.map (fun par -> ((par.X |> Seq.last), par.Y))


    let lineData = 
        range |>
        List.map (fun x -> (x, saida w1 (vx x)))
    (pointData, lineData)

let adelaideF3 =
    let vx x = vector([1.0; x; x])
    let range = [ 0.0 .. 20.0 ]
    let treinamento = 
        range |>
        List.map (fun x -> { X = vx x; Y = f3 x x |> noise 20.0 })

    let w1 = wn (List.ofSeq treinamento)

    
    let lineData = 
        range |>
        List.map (fun x -> (x, saida w1 (vx x)))
    (Seq.ofList treinamento, w1)



//#load "FSharp.Charting.fsx"
//Chart.Point [ for x in 1.0 .. 10.0 -> (x, f2 x) ]