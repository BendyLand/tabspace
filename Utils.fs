module Utils

open System
open System.IO

type Mode = ToTabs | ToSpaces | StripOnly | NeedsInferring

let classifyFlag (s: string): Mode option =
    let s = s.ToLowerInvariant()
    if   s.Contains "tab"   then Some ToTabs
    elif s.Contains "space" then Some ToSpaces
    elif s.Contains "strip" then Some StripOnly
    else None

let detectMode (flags: string[]): Mode =
    flags
    |> Array.tryPick classifyFlag
    |> Option.defaultValue NeedsInferring

let extractSpacesPerTab (flags: string[]): int =
    flags
    |> Array.tryPick (fun f ->
        match Int32.TryParse(f.TrimStart '-') with
        | true, n when n > 0 -> Some n
        | _ -> None)
    |> Option.defaultValue 4

let readFile path = File.ReadAllText path

let writeFile path text = File.WriteAllText (path, text)

let isFlag (arg: string): bool =
    arg.[0] = '-'

let splitArgs (args: string[]): string[] * string[] =
    Array.partition isFlag args

let findFilename (args: string[]): string =
    let filtered = args |> Array.filter (fun x ->  x.Contains '.')
    match Array.length filtered with
    | 1 -> Array.head filtered
    | _ -> ""

let extractFilename (args: string[]): string =
    match Array.length args with
    | 1 -> Array.head args
    | _ -> findFilename args

let inferMode (file: string) : Mode =
    let lines = file.Split '\n'
    let tabCount, spaceCount =
        lines 
        |> Array.fold (fun (tabs, spaces) line ->
            if String.IsNullOrWhiteSpace line then tabs, spaces 
            else
                match line.[0] with
                | '\t' -> tabs + 1, spaces
                | ' '  -> tabs, spaces + 1
                | _    -> tabs, spaces 
        ) (0, 0)
    if tabCount > spaceCount then ToSpaces
    elif spaceCount > tabCount then ToTabs
    else ToTabs  

let countLeadingTabs (line: string): int =
    line 
    |> Seq.takeWhile (fun c -> c = '\t')
    |> Seq.length

let countLeadingSpaces (line: string): int =
    line 
    |> Seq.takeWhile (fun c -> c = ' ')
    |> Seq.length

let convertLine (line: string) (mode: Mode) (spacesPerTab: int): string =
    match mode with
    | ToSpaces ->
        let line = line.TrimEnd [|' '; '\t'|]
        let count = countLeadingTabs line
        let prefix = String(' ', count * spacesPerTab)
        String.concat "" [prefix; line.Trim '\t']
    | ToTabs ->
        let line = line.TrimEnd [|' '; '\t'|]
        let mutable count = countLeadingSpaces line / spacesPerTab
        if count = 0 then count <- 1
        else count <- count
        let prefix = String('\t', count)
        String.concat "" [prefix; line.TrimStart ' ']
    | StripOnly ->
        line.TrimEnd [|' '; '\t'|]
    | _ ->
        printfn "You shouldn't be here either"
        exit 2

let convertTabsToSpaces (file: string) (spacesPerTab: int): string =
    let lines = file.Split "\n"
    let result =
        lines
        |> Array.map (fun line ->
            if String.IsNullOrWhiteSpace line then line
            else
                match line.[0] with
                | '\t' -> convertLine line ToSpaces spacesPerTab
                | _ -> line
        )
    String.Join ("\n", result)

let convertSpacesToTabs (file: string) (spacesPerTab: int): string =
    let lines = file.Split "\n"
    let result =
        lines
        |> Array.map (fun line ->
            if String.IsNullOrWhiteSpace line then line
            else
                match line.[0] with
                | ' ' -> convertLine line ToTabs spacesPerTab
                | _ -> line
        )
    String.Join ("\n", result)

let stripTrailingWhitespace (file: string): string =
    let lines = file.Split "\n"
    let result =
        lines
        |> Array.map (fun line ->
            if String.IsNullOrWhiteSpace line then line
            else
                line.TrimEnd [|' '; '\t'|]
        )
    String.Join ("\n", result)

