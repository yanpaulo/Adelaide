open Adelaide

[<EntryPoint>]
let main argv = 
    printfn "%A" argv
    let form = new System.Windows.Forms.Form()
    adelaideF2 |> ignore
    System.Windows.Forms.Application.Run(form)
    0 // retornar um código de saída inteiro
