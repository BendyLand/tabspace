open System
open Utils

let handleConversion (file: string) (mode: Mode) (spacesPerTab: int): string =
    match mode with
    | ToTabs -> convertSpacesToTabs file spacesPerTab
    | ToSpaces -> convertTabsToSpaces file spacesPerTab
    | StripOnly -> stripTrailingWhitespace file
    | _ ->
        printfn "You shouldn't be here"
        exit 2

let runProgram (args: string[]): int =
    let flags, nonFlags = splitArgs args
    let filename = extractFilename nonFlags
    let spacesPerTab = extractSpacesPerTab flags
    try
        let text = readFile filename
        let mode = detectMode flags
        match mode with
        | NeedsInferring ->
            let mode = inferMode text
            let result = handleConversion text mode spacesPerTab
            writeFile filename result
        | _ ->
            let result = handleConversion text mode spacesPerTab
            writeFile filename result
        printfn "File written!"
        0
    with
        | _ ->
            printfn "File not found"
            1
        
[<EntryPoint>]
let main args =
    match Array.length args with
    | 0 ->
        printfn "Usage: tabspace <file> [flags]"
        1
    | _ ->
        runProgram args

