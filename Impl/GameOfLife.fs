namespace GameOfLife
open System


type Generation(n:int, aliveCells:((int*int) list)) =
    member this.Number = n
    member this.AliveCells = aliveCells
    member this.MinX = match Seq.isEmpty aliveCells with 
        | true -> -2
        | _ -> (aliveCells |> List.map (fun (x,y) -> x) |> List.min)-2
    member this.MinY =  match aliveCells.Length with 
        | 0 -> -2
        | _ -> (aliveCells |> List.map (fun (x,y) -> y) |> List.min)-2
    member this.MaxX =  match aliveCells.Length with 
        | 0 -> +2
        | _ -> (aliveCells |> List.map (fun (x,y) -> x) |> List.max)+2
    member this.MaxY =  match aliveCells.Length with 
        | 0 -> +2
        | _ -> (aliveCells |> List.map (fun (x,y) -> y) |> List.max)+2

module Format = 
    let one (gen:Generation) = 
            let charListToString list = new String(List.toArray list)
            let formatForLocation p = 
                let isAliveAt (x,y) = gen.AliveCells |> List.exists (fun (x',y') -> x=x' && y=y')
                match isAliveAt p with
                | true -> '#'
                | false -> ' '
            let formatRow y = "║" + ([gen.MinX..gen.MaxX] |> List.map (fun x -> formatForLocation (x,y)) |> charListToString) + "║"
          
            let formatLines = 
                let infoLine = "Gen " + gen.Number.ToString()
                let firstLine = "╔" + (String.replicate (gen.MaxX - gen.MinX+1) "═") + "╗"
                let lastLine = "╚" + (String.replicate (gen.MaxX - gen.MinX+1) "═") + "╝"
                let middleLines = [gen.MinY..gen.MaxY] |> List.map formatRow
                List.toArray (infoLine :: firstLine :: middleLines @ [lastLine])
        
            String.Join ("\r\n", formatLines)

    let all (gens:Generation list) = String.Join ("\r\n\r\n", (gens |> List.map one))

module Life =
    let tick1 (gen:Generation) =
        let surroundingLocations (x,y) = [
            (x-1,y-1);(x,y-1);(1,y+1);
            (x-1,y); (x,y); (x+1,y);
            (x-1,y+1);(x,y+1);(1,y+1)
            ]
        let countSurrounding (x:int,y:int) = 
            let isNear (x',y') = match (x-x' |> Math.Abs, y-y' |> Math.Abs) with
                | (1,1) -> true
                | (0,1) -> true
                | (1,0) -> true
                | _ -> false
            gen.AliveCells |> Seq.filter isNear  |> Seq.length 
        let isAliveAt (x,y) = gen.AliveCells |> List.exists (fun (x',y') -> x=x' && y=y')
        let willHaveLife l = match (isAliveAt l, countSurrounding l) with
            | (true, 2) -> true
            | (_,3) -> true
            | _ -> false
        let locationsToScan = 
            gen.AliveCells |> List.map surroundingLocations |> Seq.collect id |> Seq.distinct
        let createGeneration cells = new Generation(gen.Number+1, List.ofSeq cells)
        locationsToScan |> Seq.filter willHaveLife |> createGeneration

    let rec tickN prevGen n =  
        match n with
        | 0 -> List.rev prevGen
        | n -> tickN ((tick1 (List.head prevGen)) :: prevGen)  (n-1)


module StartCultures =
    let Blinker = new Generation( 0, [(0,0); (1,0); (2,0)])
    let GliderGun =  new Generation( 0, [(2, 6); (2, 7); (3, 6); (3, 7); (12, 6); (12, 7); (12, 8); (13, 5); (13, 9); (14, 4); (14, 10); (15, 4); (15, 10); (16, 7); (17, 5); (17, 9); (18, 6); (18, 7); (18, 8); (19, 7); (22, 4); (22, 5); (22, 6); (23, 4); (23, 5); (23, 6); (24, 3); (24, 7); (26, 2); (26, 3); (26, 7); (26, 8); (36, 4); (36, 5); (37, 4); (37, 5)])
    let BlockLayer2 =  new Generation( 0, [
                        (0,0); (0,1); (0,2);        (0,4);
                        (1,0); 
                                             (2,3); (2,4);
                               (3,1); (3,2);        (3,4);
                        (4,0);        (4,2);        (4,4);
                        ])
