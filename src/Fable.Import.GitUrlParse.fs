// ts2fable 0.7.1
module rec GitUrlParse
open System
open Fable.Core
open Fable.Core.JS

let [<Import("*","git-url-parse")>] gitUrlParse: GitUrlParse.IExports = jsNative

type [<AllowNullLiteral>] IExports =
    abstract gitUrlParse: url: string -> GitUrlParse.GitUrl

module GitUrlParse =

    type [<AllowNullLiteral>] IExports =
        abstract stringify: url: GitUrl * ?``type``: string -> string

    type [<AllowNullLiteral>] GitUrl =
        /// An array with the url protocols (usually it has one element).
        abstract protocols: ResizeArray<string> with get, set
        abstract port: float option with get, set
        /// The url domain (including subdomains).
        abstract resource: string with get, set
        /// The authentication user (usually for ssh urls).
        abstract user: string with get, set
        abstract pathname: string with get, set
        abstract hash: string with get, set
        abstract search: string with get, set
        abstract href: string with get, set
        abstract protocol: string with get, set
        /// The oauth token (could appear in the https urls).
        abstract token: string with get, set
        /// The Git provider (e.g. `"github.com"`).
        abstract source: string with get, set
        /// The repository owner.
        abstract owner: string with get, set
        /// The repository name.
        abstract name: string with get, set
        /// The repository ref (e.g., "master" or "dev").
        abstract ref: string with get, set
        /// A filepath relative to the repository root.
        abstract filepath: string with get, set
        /// The type of filepath in the url ("blob" or "tree").
        abstract filepathtype: string with get, set
        /// The owner and name values in the `owner/name` format.
        abstract full_name: string with get, set
        /// The organization the owner belongs to. This is CloudForge specific.
        abstract organization: string with get, set
        /// Whether to add the `.git` suffix or not.
        abstract git_suffix: bool option with get, set
        abstract toString: ?``type``: string -> string
