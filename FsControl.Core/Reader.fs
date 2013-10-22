﻿namespace FsControl.Core.Types

open FsControl.Core.Abstractions
open FsControl.Core.Abstractions.Applicative

type Reader<'R,'A> = Reader of ('R->'A)

[<RequireQualifiedAccess>]
module Reader =
    let run (Reader x) = x
    let map   f (Reader m) = Reader(f << m) :Reader<'r,_>    
    let local f (Reader m) = Reader(m << f) :Reader<'r,_>
    let ask() = Reader id

type Reader<'R,'A> with
    static member instance (Functor.Map  , Reader m:Reader<'r,'a>, _) = fun (f:_->'b) -> Reader(fun r -> f (m r))
    static member instance (Monad.Return, _:Reader<'r,'a>            ) = fun a -> Reader(fun _ -> a)                     :Reader<'r,'a>
    static member instance (Monad.Bind  ,   Reader m, _:Reader<'r,'b>) = fun k -> Reader(fun r -> Reader.run(k (m r)) r) :Reader<'r,'b>
    static member instance (Applicative.Pure, _:Reader<'r,'a>) = fun (x:'a) -> DefaultImpl.PureFromMonad x :Reader<'r,_>
    static member instance (Applicative.Ap, f:Reader<'r,_>, x:Reader<'r,'a>, _:Reader<'r,'b>) = fun () -> DefaultImpl.ApFromMonad f x :Reader<'r,'b>