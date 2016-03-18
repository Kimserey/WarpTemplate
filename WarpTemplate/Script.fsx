#I "../packages/"
#load "WebSharper.Warp/tools/reference-nover.fsx"

open WebSharper
open WebSharper.JavaScript
open WebSharper.Sitelets
open WebSharper.UI.Next
open WebSharper.UI.Next.Html
open WebSharper.UI.Next.Client

[<JavaScript>]
module Client =

    let main() =
        let rv: Var<Var<int> list> = Var.Create []

        div [ Doc.Button "reset" 
                  [ attr.``class`` "btn btn-default" ] 
                  (fun () -> 
                    Var.Set rv [])
              
              Doc.Button "add" 
                  [ attr.``class`` "btn btn-default" ] 
                  (fun () -> Var.Set rv (rv.Value @ [ Var.Create (List.length rv.Value ) ]))
              
              rv.View
              |> View.Map (fun l -> div [ text (sprintf "Number of items: %i" (List.length l)) ])
              |> Doc.EmbedView

              text "List:"; br[]
              rv.View 
              |> View.Map (List.map View.FromVar >> View.Sequence) 
              |> View.Join
              |> View.Map (fun l -> l |> Seq.map (sprintf "%i " >> text) |> div)
              |> Doc.EmbedView ]

module Server =

    type Page = { Body: Doc list }

    let template =
        Content.Template<Page>(__SOURCE_DIRECTORY__ + "/index.html")
            .With("body", fun x -> x.Body)
    
    let site =
        Application.SinglePage (fun _ ->
            Content.WithTemplate template
                { Body = [ client <@ Client.main() @> ] })

do Warp.RunAndWaitForInput Server.site |> ignore