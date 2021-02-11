module Extension

open Fable.Core
open Fable.Core.JsInterop
open Fable.Import.vscode
open Fable.Core.JS
open GitUrlParse
open Git

[<Emit("$0.match(/\w+/g)")>]
let wordsIn (str : string) : obj = jsNative

let formatCount (n : int) : string = importMember "./formatter"

let gitUrlParse (url : string) : GitUrlParse.GitUrl = importDefault "git-url-parse"

let getGitOriginURL (folder: Uri) =
    let gitExtension = extensions.getExtension<GitExtension>("vscode.git")

    match gitExtension with
    | e when isNullOrUndefined (e.exports) = false && e.exports.enabled = true ->
        let repo = gitExtension.exports.getAPI(1).repositories.Find(fun itm -> itm.rootUri.fsPath = folder.fsPath)
        match repo with
        | r when isNullOrUndefined(r) -> None
        | _ ->
            let urls = repo.state.remotes |> Seq.filter (fun itm -> itm.fetchUrl.IsSome) |> Seq.toArray
            match urls.Length with
            | len when len = 1 -> Some(urls.[0].fetchUrl)
            | _ -> None
    | e -> None

let extName = "gitopen"

let logger = window.createOutputChannel "GitOpen"

let formatIcon (url: string) =
    let gitUrl = gitUrlParse url
    match gitUrl.source with
    | s when s = "github.com" ->
        sprintf "$(github) %s" gitUrl.full_name
    | s when s = "gitlab.com" ->
        sprintf "$(octoface) %s" gitUrl.full_name
    | s ->
        sprintf "$(octoface) %s" gitUrl.full_name

let setSbTextByUsingWorkspaceFolder (sb: StatusBarItem, folder: WorkspaceFolder) =
    match getGitOriginURL folder.uri with
    | Some u ->
        sb.command <- sprintf "%s.openRepo" extName
        sb.text <- formatIcon(u.Value)
        sb.show ()
    | None -> sb.hide ()

let countWordsInDocument (d : TextDocument) =
  match wordsIn (d.getText ()) with
  // Don't match with `null` because that generates
  // more code
  | m when isNull m -> 0
  | m -> unbox m?length

let activate (context : ExtensionContext) =
  let sb = window.createStatusBarItem (StatusBarAlignment.Left, 1001.0)

  let getActiveWorkspaceFolder () =
    match workspace.workspaceFolders.Length with
    | len when len > 1 ->
        match window.activeTextEditor with
        | e when isNullOrUndefined e -> None
        | e ->
            match workspace.getWorkspaceFolder(e.document.uri) with
            | folder when isNullOrUndefined folder -> None
            | folder -> Some folder
     | len when len = 1 -> Some workspace.workspaceFolders.[0]
    | _ -> None


  let updateStatusBarItem () =
    match getActiveWorkspaceFolder() with
    | Some folder -> setSbTextByUsingWorkspaceFolder (sb, folder)
    | None -> sb.hide ()

  let openRepo () =
    match getActiveWorkspaceFolder() with
    | Some folder ->
        sprintf "Git url: %s" (getGitOriginURL(folder.uri).ToString()) |> logger.appendLine
        match getGitOriginURL folder.uri with
        | Some url ->
            let res = gitUrlParse(url.Value)
            res.protocol <- "https"
            res.git_suffix <- Some false
            env.openExternal (res.toString())
        | None -> ignore()
    | _ -> ignore()

  let disposables : Disposable [] = [||]

  commands.registerCommand (sprintf "%s.openRepo" extName, fun _ -> openRepo() |> unbox<obj>) |> context.subscriptions.Add
  window.onDidChangeActiveTextEditor $ (updateStatusBarItem, (), disposables) |> ignore

  updateStatusBarItem ()
