open Adelaide.Module
open FSharp.Charting

[<EntryPoint>]
let main argv = 
    let form = new System.Windows.Forms.Form()
    let (pointData, lineData) = adelaideF2
    let point = Chart.Point pointData
    let line = Chart.Line lineData

    let combine = Chart.Combine [point; line]
    combine.ShowChart() |> ignore
    System.Windows.Forms.Application.Run(form)
    0 // retornar um código de saída inteiro
