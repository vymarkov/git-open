module Extension

open Fable.Core
open Fable.Core.JsInterop
open Fable.Import
open Fable.Import.vscode

[<Emit("$0.match(/\w+/g)")>]
let wordsIn (str : string) : obj = jsNative

let formatCount : int -> string = importMember "./formatter"

let activate (context : ExtensionContext) =
  let sb = window.createStatusBarItem StatusBarAlignment.Left

  let wordCount (d : TextDocument) =
    match wordsIn (d.getText ()) with
    | null -> 0
    | m -> unbox m?length

  let showIfMdElseHide () =
    match window.activeTextEditor with
    | Some e ->
      let d = e.document
      if d.languageId = "markdown" then
        sb.text <- d |> wordCount |> formatCount
        sb.show ()
      else
        sb.hide ()
    | _ -> sb.hide ()

  showIfMdElseHide ()

  let disposables : Disposable [] = [||]
  window.onDidChangeActiveTextEditor $ (showIfMdElseHide, (), disposables) |> ignore
  window.onDidChangeTextEditorSelection $ (showIfMdElseHide, (), disposables) |> ignore
