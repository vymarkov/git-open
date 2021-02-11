// ts2fable 0.7.1
module rec Git
open System
open Fable.Core
open Fable.Core.JS
open Fable.Import.vscode

// type Uri = Uri
// type Event = Event
// type Disposable = Disposable
// type ProviderResult = ProviderResult

type [<AllowNullLiteral>] Git =
    abstract path: string

type [<AllowNullLiteral>] InputBox =
    abstract value: string with get, set

type RefType =
    obj

type [<AllowNullLiteral>] Ref =
    abstract ``type``: RefType
    abstract name: string option
    abstract commit: string option
    abstract remote: string option

type [<AllowNullLiteral>] UpstreamRef =
    abstract remote: string
    abstract name: string

type [<AllowNullLiteral>] Branch =
    inherit Ref
    abstract upstream: UpstreamRef option
    abstract ahead: float option
    abstract behind: float option

type [<AllowNullLiteral>] Commit =
    abstract hash: string
    abstract message: string
    abstract parents: ResizeArray<string>
    abstract authorDate: DateTime option
    abstract authorName: string option
    abstract authorEmail: string option
    abstract commitDate: DateTime option

type [<AllowNullLiteral>] Submodule =
    abstract name: string
    abstract path: string
    abstract url: string

type [<AllowNullLiteral>] Remote =
    abstract name: string
    abstract fetchUrl: string option
    abstract pushUrl: string option
    abstract isReadOnly: bool

type Status =
    obj

type [<AllowNullLiteral>] Change =
    /// Returns either `originalUri` or `renameUri`, depending
    /// on whether this change is a rename change. When
    /// in doubt always use `uri` over the other two alternatives.
    abstract uri: Uri
    abstract originalUri: Uri
    abstract renameUri: Uri option
    abstract status: Status

type [<AllowNullLiteral>] RepositoryState =
    abstract HEAD: Branch option
    abstract refs: ResizeArray<Ref>
    abstract remotes: ResizeArray<Remote>
    abstract submodules: ResizeArray<Submodule>
    abstract rebaseCommit: Commit option
    abstract mergeChanges: ResizeArray<Change>
    abstract indexChanges: ResizeArray<Change>
    abstract workingTreeChanges: ResizeArray<Change>
    abstract onDidChange: Event<unit>

type [<AllowNullLiteral>] RepositoryUIState =
    abstract selected: bool
    abstract onDidChange: Event<unit>

/// Log options.
type [<AllowNullLiteral>] LogOptions =
    /// Max number of log entries to retrieve. If not specified, the default is 32.
    abstract maxEntries: float option
    abstract path: string option

type [<AllowNullLiteral>] CommitOptions =
    abstract all: U2<bool, string> option with get, set
    abstract amend: bool option with get, set
    abstract signoff: bool option with get, set
    abstract signCommit: bool option with get, set
    abstract empty: bool option with get, set
    abstract noVerify: bool option with get, set

type [<AllowNullLiteral>] BranchQuery =
    abstract remote: bool option
    abstract pattern: string option
    abstract count: float option
    abstract contains: string option

type [<AllowNullLiteral>] Repository =
    abstract rootUri: Uri
    abstract inputBox: InputBox
    abstract state: RepositoryState
    abstract ui: RepositoryUIState
    abstract getConfigs: unit -> Promise<ResizeArray<RepositoryGetConfigsPromise>>
    abstract getConfig: key: string -> Promise<string>
    abstract setConfig: key: string * value: string -> Promise<string>
    abstract getGlobalConfig: key: string -> Promise<string>
    abstract getObjectDetails: treeish: string * path: string -> Promise<RepositoryGetObjectDetailsPromise>
    abstract detectObjectType: ``object``: string -> Promise<RepositoryDetectObjectTypePromise>
    abstract buffer: ref: string * path: string -> Promise<Buffer>
    abstract show: ref: string * path: string -> Promise<string>
    abstract getCommit: ref: string -> Promise<Commit>
    abstract clean: paths: ResizeArray<string> -> Promise<unit>
    abstract apply: patch: string * ?reverse: bool -> Promise<unit>
    abstract diff: ?cached: bool -> Promise<string>
    abstract diffWithHEAD: unit -> Promise<ResizeArray<Change>>
    abstract diffWithHEAD: path: string -> Promise<string>
    abstract diffWith: ref: string -> Promise<ResizeArray<Change>>
    abstract diffWith: ref: string * path: string -> Promise<string>
    abstract diffIndexWithHEAD: unit -> Promise<ResizeArray<Change>>
    abstract diffIndexWithHEAD: path: string -> Promise<string>
    abstract diffIndexWith: ref: string -> Promise<ResizeArray<Change>>
    abstract diffIndexWith: ref: string * path: string -> Promise<string>
    abstract diffBlobs: object1: string * object2: string -> Promise<string>
    abstract diffBetween: ref1: string * ref2: string -> Promise<ResizeArray<Change>>
    abstract diffBetween: ref1: string * ref2: string * path: string -> Promise<string>
    abstract hashObject: data: string -> Promise<string>
    abstract createBranch: name: string * checkout: bool * ?ref: string -> Promise<unit>
    abstract deleteBranch: name: string * ?force: bool -> Promise<unit>
    abstract getBranch: name: string -> Promise<Branch>
    abstract getBranches: query: BranchQuery -> Promise<ResizeArray<Ref>>
    abstract setBranchUpstream: name: string * upstream: string -> Promise<unit>
    abstract getMergeBase: ref1: string * ref2: string -> Promise<string>
    abstract status: unit -> Promise<unit>
    abstract checkout: treeish: string -> Promise<unit>
    abstract addRemote: name: string * url: string -> Promise<unit>
    abstract removeRemote: name: string -> Promise<unit>
    abstract renameRemote: name: string * newName: string -> Promise<unit>
    abstract fetch: ?remote: string * ?ref: string * ?depth: float -> Promise<unit>
    abstract pull: ?unshallow: bool -> Promise<unit>
    abstract push: ?remoteName: string * ?branchName: string * ?setUpstream: bool -> Promise<unit>
    abstract blame: path: string -> Promise<string>
    abstract log: ?options: LogOptions -> Promise<ResizeArray<Commit>>
    abstract commit: message: string * ?opts: CommitOptions -> Promise<unit>

type [<AllowNullLiteral>] RemoteSource =
    abstract name: string
    abstract description: string option
    abstract url: U2<string, ResizeArray<string>>

type [<AllowNullLiteral>] RemoteSourceProvider =
    abstract name: string
    abstract icon: string option
    abstract supportsQuery: bool option
    abstract getRemoteSources: ?query: string -> ResizeArray<RemoteSource>
    abstract publishRepository: repository: Repository -> Promise<unit>

type [<AllowNullLiteral>] Credentials =
    abstract username: string
    abstract password: string

type [<AllowNullLiteral>] CredentialsProvider =
    abstract getCredentials: host: Uri -> Credentials

type [<AllowNullLiteral>] PushErrorHandler =
    abstract handlePushError: repository: Repository * remote: Remote * refspec: string * error: obj -> Promise<bool>

type [<StringEnum>] [<RequireQualifiedAccess>] APIState =
    | Uninitialized
    | Initialized

type [<AllowNullLiteral>] API =
    abstract state: APIState
    abstract onDidChangeState: Event<APIState>
    abstract git: Git
    abstract repositories: ResizeArray<Repository>
    abstract onDidOpenRepository: Event<Repository>
    abstract onDidCloseRepository: Event<Repository>
    abstract toGitUri: uri: Uri * ref: string -> Uri
    abstract getRepository: uri: Uri -> Repository option
    abstract init: root: Uri -> Promise<Repository option>
    abstract registerRemoteSourceProvider: provider: RemoteSourceProvider -> Disposable
    abstract registerCredentialsProvider: provider: CredentialsProvider -> Disposable
    abstract registerPushErrorHandler: handler: PushErrorHandler -> Disposable

type [<AllowNullLiteral>] GitExtension =
    abstract enabled: bool
    abstract onDidChangeEnablement: Event<bool>
    /// <summary>Returns a specific API version.
    ///
    /// Throws error if git extension is disabled. You can listed to the
    /// [GitExtension.onDidChangeEnablement](#GitExtension.onDidChangeEnablement) event
    /// to know when the extension becomes enabled/disabled.</summary>
    /// <param name="version">Version number.</param>
    abstract getAPI: version: obj -> API

type [<StringEnum>] [<RequireQualifiedAccess>] GitErrorCodes =
    | [<CompiledName "BadConfigFile">] BadConfigFile
    | [<CompiledName "AuthenticationFailed">] AuthenticationFailed
    | [<CompiledName "NoUserNameConfigured">] NoUserNameConfigured
    | [<CompiledName "NoUserEmailConfigured">] NoUserEmailConfigured
    | [<CompiledName "NoRemoteRepositorySpecified">] NoRemoteRepositorySpecified
    | [<CompiledName "NotAGitRepository">] NotAGitRepository
    | [<CompiledName "NotAtRepositoryRoot">] NotAtRepositoryRoot
    | [<CompiledName "Conflict">] Conflict
    | [<CompiledName "StashConflict">] StashConflict
    | [<CompiledName "UnmergedChanges">] UnmergedChanges
    | [<CompiledName "PushRejected">] PushRejected
    | [<CompiledName "RemoteConnectionError">] RemoteConnectionError
    | [<CompiledName "DirtyWorkTree">] DirtyWorkTree
    | [<CompiledName "CantOpenResource">] CantOpenResource
    | [<CompiledName "GitNotFound">] GitNotFound
    | [<CompiledName "CantCreatePipe">] CantCreatePipe
    | [<CompiledName "PermissionDenied">] PermissionDenied
    | [<CompiledName "CantAccessRemote">] CantAccessRemote
    | [<CompiledName "RepositoryNotFound">] RepositoryNotFound
    | [<CompiledName "RepositoryIsLocked">] RepositoryIsLocked
    | [<CompiledName "BranchNotFullyMerged">] BranchNotFullyMerged
    | [<CompiledName "NoRemoteReference">] NoRemoteReference
    | [<CompiledName "InvalidBranchName">] InvalidBranchName
    | [<CompiledName "BranchAlreadyExists">] BranchAlreadyExists
    | [<CompiledName "NoLocalChanges">] NoLocalChanges
    | [<CompiledName "NoStashFound">] NoStashFound
    | [<CompiledName "LocalChangesOverwritten">] LocalChangesOverwritten
    | [<CompiledName "NoUpstreamBranch">] NoUpstreamBranch
    | [<CompiledName "IsInSubmodule">] IsInSubmodule
    | [<CompiledName "WrongCase">] WrongCase
    | [<CompiledName "CantLockRef">] CantLockRef
    | [<CompiledName "CantRebaseMultipleBranches">] CantRebaseMultipleBranches
    | [<CompiledName "PatchDoesNotApply">] PatchDoesNotApply
    | [<CompiledName "NoPathFound">] NoPathFound
    | [<CompiledName "UnknownPath">] UnknownPath

type [<AllowNullLiteral>] RepositoryGetConfigsPromise =
    abstract key: string with get, set
    abstract value: string with get, set

type [<AllowNullLiteral>] RepositoryGetObjectDetailsPromise =
    abstract mode: string with get, set
    abstract ``object``: string with get, set
    abstract size: float with get, set

type [<AllowNullLiteral>] RepositoryDetectObjectTypePromise =
    abstract mimetype: string with get, set
    abstract encoding: string option with get, set
