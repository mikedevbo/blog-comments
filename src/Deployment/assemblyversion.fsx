open System.Reflection
let args = fsi.CommandLineArgs

printfn "%s" ((args.[1] |> Assembly.LoadFile).FullName)