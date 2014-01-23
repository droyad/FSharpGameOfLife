namespace GameOfLife
open System


type Location(x:int, y:int) = 
    member this.X = x
    member this.Y = y
    override this.Equals(o) =
        match o with
        | :? Location as sc -> this.X = sc.X && this.Y = sc.Y
        | _ -> false
    override this.GetHashCode() =
        x.GetHashCode()
    interface System.IComparable with
        member this.CompareTo(o) =
            match o with
            | :? Location as sc -> match (compare this.X sc.X) with
                | 0 -> compare this.Y sc.Y
                | _ as r -> r 
            | _ -> -1

type Generation(n:int, aliveCells:Set<Location>) =
    member this.Number = n
    member this.AliveCells = aliveCells
    member this.MinX = match aliveCells.Count with 
        | 0 -> -2
        | _ -> (aliveCells |> Set.map (fun f -> f.X) |> Set.minElement)-2
    member this.MinY =  match aliveCells.Count with 
        | 0 -> -2
        | _ -> (aliveCells |> Set.map (fun f -> f.Y) |> Set.minElement)-2
    member this.MaxX =  match aliveCells.Count with 
        | 0 -> +2
        | _ -> (aliveCells |> Set.map (fun f -> f.X) |> Set.maxElement)+2
    member this.MaxY =  match aliveCells.Count with 
        | 0 -> +2
        | _ -> (aliveCells |> Set.map (fun f -> f.Y) |> Set.maxElement)+2
    static member ofList cells = Generation(0, set (cells |> List.map (fun (x,y) -> Location(x,y))))


module Format = 
    let one (gen:Generation) = 
            let charListToString list = new String(List.toArray list)
            let containsLocationAt set (x,y) = Set.contains (new Location(x,y)) set
            let isAliveAt l = containsLocationAt gen.AliveCells l
            let formatForLocation p = match isAliveAt p with
                | true -> '#'
                | false -> ' '
            let formatRow y = "║" + ([gen.MinX..gen.MaxX] |> List.map (fun x -> formatForLocation (x,y)) |> charListToString) + "║"
            let infoLine = "Gen " + gen.Number.ToString()
            let firstLine = "╔" + (String.replicate (gen.MaxX - gen.MinX+1) "═") + "╗"
            let lastLine = "╚" + (String.replicate (gen.MaxX - gen.MinX+1) "═") + "╝"
            let middleLines = [gen.MinY..gen.MaxY] |> List.map formatRow
            let formatLines = List.toArray (infoLine :: firstLine :: middleLines @ [lastLine])
        
            String.Join ("\r\n", formatLines)

    let all (gens:Generation list) = String.Join ("\r\n\r\n", (gens |> List.map one))


module Life =
    let tick1 (gen:Generation) =
        let surroundingPoints (x,y) = set [
            new Location(x-1,y-1); new Location(x,y-1);new Location(x+1,y-1);
            new Location(x-1,y);                   new Location(x+1,y);
            new Location(x-1,y+1); new Location(x,y+1);new Location(x+1,y+1)
            ]
        let countSurrounding (l:Location) = surroundingPoints (l.X, l.Y) |> Set.intersect gen.AliveCells |> Set.count
        let hasLifeAt l = Set.contains  l gen.AliveCells
        let willHaveLife l = match (hasLifeAt l, countSurrounding l) with
            | (true, 2) -> true
            | (_,3) -> true
            | _ -> false
        let allPossibleLocations = 
            let eachRow x = 
                [gen.MinY..gen.MaxY] |> List.map (fun y -> new Location(x,y))
            [gen.MinX..gen.MaxX] |> Seq.collect eachRow |> List.ofSeq
        let createGeneration cells = new Generation(gen.Number+1, Set.ofSeq cells)
        allPossibleLocations |> List.filter willHaveLife |> createGeneration

    let rec tickN prevGen n =  
        match n with
        | 0 -> List.rev prevGen
        | n -> tickN ((tick1 (List.head prevGen)) :: prevGen)  (n-1)


module StartCultures =
    let Blinker = Generation.ofList [(0,0); (1,0); (2,0)]
    let GliderGun = Generation.ofList [(2, 6); (2, 7); (3, 6); (3, 7); (12, 6); (12, 7); (12, 8); (13, 5); (13, 9); (14, 4); (14, 10); (15, 4); (15, 10); (16, 7); (17, 5); (17, 9); (18, 6); (18, 7); (18, 8); (19, 7); (22, 4); (22, 5); (22, 6); (23, 4); (23, 5); (23, 6); (24, 3); (24, 7); (26, 2); (26, 3); (26, 7); (26, 8); (36, 4); (36, 5); (37, 4); (37, 5)]
