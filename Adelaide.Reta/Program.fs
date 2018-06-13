open Adelaide.Module
open FSharp.Charting
open MathNet.Numerics.LinearAlgebra

[<EntryPoint>]
let main argv = 
    let vx x = vector ([1.0; x])
    let form = new System.Windows.Forms.Form()
    let resultado = realizaXY

    let pointData = 
        resultado.Treinamento |>
        Seq.map (fun par -> (par.X |> Seq.last , par.Y))

    let lineData = 
        [0.0 .. 20.0] |>
        Seq.map (fun x -> (x, saida resultado.W (vx x)))

    let point = Chart.Point pointData
    let line = Chart.Line lineData

    let combine = Chart.Combine [point; line]
    combine.ShowChart() |> ignore
    System.Windows.Forms.Application.Run(form)
    0 // retornar um código de saída inteiro
