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

    let (<*>) f x = View.Apply f x

    let main() =
        let rv: ListModel<int, int> = ListModel.Create id []

        let modify n n' =
            let x = rv.LensInto id (fun _ a -> a) n
            x.Set n'

        let key = Var.Create 0
        let replace = Var.Create 0

        div [ Doc.Button "reset" [ attr.``class`` "btn btn-default" ] rv.Clear
              
              Doc.Button "add"   [ attr.``class`` "btn btn-default" ] (fun () -> rv.Add rv.Length)
              
              Doc.IntInputUnchecked [] key
              Doc.IntInputUnchecked [] replace

              View.Const (fun key replace -> modify key replace; Doc.Empty)
              <*> key.View
              <*> replace.View
              |> Doc.EmbedView

              rv.LengthAsView
              |> Doc.BindView (fun i -> div [ text (sprintf "Number of items: %i" i) ])

              text "List:"; br[]
              div [ rv.View 
                    |> Doc.BindSeqCached (sprintf "%i " >> text) ] ]

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