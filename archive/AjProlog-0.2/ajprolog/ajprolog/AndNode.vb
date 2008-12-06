' +---------------------------------------------------------------------+
' | ajprolog - A Prolog Interpreter                                     |
' +---------------------------------------------------------------------+
' | Copyright (c) 2003-2004 Angel J. Lopez. All rights reserved.        |
' +---------------------------------------------------------------------+
' | This source file is subject to the ajprolog Software License,       |
' | Version 1.0, that is bundled with this package in the file LICENSE. |
' | If you did not receive a copy of this file, you may read it online  |
' | at http://www.ajlopez.net/ajprolog/license.txt.                     |
' +---------------------------------------------------------------------+
'
'
Public Class AndNode
    Inherits PrimitiveNode

    Public Sub New(ByVal pm As PrologMachine, ByVal st As StructureObject)
        MyBase.New(pm, st)
    End Sub

    Public Sub New(ByVal pm As PrologMachine, ByVal obj As Primitive)
        MyBase.New(pm, obj)
    End Sub

    Public Overrides Function IsPushable() As Boolean
        Return False
    End Function
End Class
