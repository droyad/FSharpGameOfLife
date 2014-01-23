open System
open GameOfLife

[<EntryPoint>]
let main argv = 
    let rec tickAndPrint g = 
        let gen = Life.tick1 g
        Console.Clear()
        printfn "%s" (Format.one gen)
       // Threading.Thread.Sleep(100)
        tickAndPrint gen

    tickAndPrint StartCultures.BlockLayer2
    Console.ReadLine()
    0 // return an integer exit code
