namespace Newtonsoft.Json.Converters

open System
open System.Collections.Generic
open Microsoft.FSharp.Reflection
open Newtonsoft.Json

type OptionConverter() =
    inherit JsonConverter()
    
    override x.CanConvert(t) = 
        t.IsGenericType && t.GetGenericTypeDefinition() = typedefof<option<_>>
 
    override x.WriteJson(writer, value, serializer) =
        let value = 
            if value = null then null
            else 
                let _,fields = FSharpValue.GetUnionFields(value, value.GetType())
                fields.[0]  
        serializer.Serialize(writer, value)
 
    override x.ReadJson(reader, t, existingValue, serializer) =        
        let innerType = t.GetGenericArguments().[0]
        let innerType = 
            if innerType.IsValueType then (typedefof<Nullable<_>>).MakeGenericType([|innerType|])
            else innerType        
        let value = serializer.Deserialize(reader, innerType)
        let cases = FSharpType.GetUnionCases(t)
        if value = null then FSharpValue.MakeUnion(cases.[0], [||])
        else FSharpValue.MakeUnion(cases.[1], [|value|])

type ListConverter() =
    inherit JsonConverter()
    
    override x.CanConvert(t:Type) = 
        t.IsGenericType && t.GetGenericTypeDefinition() = typedefof<list<_>>
 
    override x.WriteJson(writer, value, serializer) =
        let list = value :?> System.Collections.IEnumerable |> Seq.cast
        serializer.Serialize(writer, list)
 
    override x.ReadJson(reader, t, _, serializer) = 
        let itemType = t.GetGenericArguments().[0]
        let collectionType = typedefof<IEnumerable<_>>.MakeGenericType(itemType)
        let collection = serializer.Deserialize(reader, collectionType) :?> IEnumerable<_>        
        let listType = typedefof<list<_>>.MakeGenericType(itemType)
        let cases = FSharpType.GetUnionCases(listType)
        let rec make = function
            | [] -> FSharpValue.MakeUnion(cases.[0], [||])
            | head::tail -> FSharpValue.MakeUnion(cases.[1], [| head; (make tail); |])                    
        make (collection |> Seq.toList)

type TupleArrayConverter() =
    inherit JsonConverter()
    
    override x.CanConvert(t:Type) = 
        FSharpType.IsTuple(t)
 
    override x.WriteJson(writer, value, serializer) =
        let values = FSharpValue.GetTupleFields(value)
        serializer.Serialize(writer, values)
 
    override x.ReadJson(reader, t, _, serializer) =
        let advance = reader.Read >> ignore
        let deserialize t = serializer.Deserialize(reader, t)
        let itemTypes = FSharpType.GetTupleElements(t)
 
        let readElements() =
            let rec read index acc =
                match reader.TokenType with
                | JsonToken.EndArray -> 
                    List.rev acc
                | _ ->
                    let value = deserialize(itemTypes.[index])
                    advance()
                    read (index + 1) (value :: acc)
            advance()
            read 0 List.empty
 
        match reader.TokenType with
        | JsonToken.StartArray ->
            let values = readElements()
            FSharpValue.MakeTuple(values |> List.toArray, t)
        | _ -> failwith "invalid token"

type UnionTypeConverter() =
    inherit JsonConverter()
 
    let doRead (reader: JsonReader) = 
        reader.Read() |> ignore
 
    override x.CanConvert(typ:Type) =
        let result = 
            ((typ.GetInterface(typeof<System.Collections.IEnumerable>.FullName) = null) 
            && FSharpType.IsUnion typ)
        result
 
    override x.WriteJson(writer: JsonWriter, value: obj, serializer: JsonSerializer) =
        let t = value.GetType()
        let write (name : string) (fields : obj[]) = 
            writer.WriteStartObject()
            writer.WritePropertyName("case")
            writer.WriteValue(name)  
            writer.WritePropertyName("values")
            serializer.Serialize(writer, fields)
            writer.WriteEndObject()   
 
        let (info, fields) = FSharpValue.GetUnionFields(value, t)
        write info.Name fields
 
    override x.ReadJson(reader: JsonReader, objectType: Type, existingValue: obj, serializer: JsonSerializer) =      
         let cases = FSharpType.GetUnionCases(objectType)
         if reader.TokenType <> JsonToken.Null  
         then
            doRead reader
            doRead reader
            let case = cases |> Array.find (fun x -> x.Name = if reader.Value = null then "None" else reader.Value.ToString())
            doRead reader
            doRead reader
            doRead reader
            let fields =  [| 
                for field in case.GetFields() do
                    let result = serializer.Deserialize(reader, field.PropertyType)
                    reader.Read() |> ignore
                    yield result
            |] 
            let result = FSharpValue.MakeUnion(case, fields)
            while reader.TokenType <> JsonToken.EndObject do
                doRead reader         
            result
         else
            FSharpValue.MakeUnion(cases.[0], [||]) 

namespace Raven.Converters

open System
open System.Collections.Generic
open Microsoft.FSharp.Reflection
open Raven.Imports.Newtonsoft.Json

type OptionConverter() =
    inherit JsonConverter()
    
    override x.CanConvert(t) = 
        t.IsGenericType && t.GetGenericTypeDefinition() = typedefof<option<_>>
 
    override x.WriteJson(writer, value, serializer) =
        let value = 
            if value = null then null
            else 
                let _,fields = FSharpValue.GetUnionFields(value, value.GetType())
                fields.[0]  
        serializer.Serialize(writer, value)
 
    override x.ReadJson(reader, t, existingValue, serializer) =        
        let innerType = t.GetGenericArguments().[0]
        let innerType = 
            if innerType.IsValueType then (typedefof<Nullable<_>>).MakeGenericType([|innerType|])
            else innerType        
        let value = serializer.Deserialize(reader, innerType)
        let cases = FSharpType.GetUnionCases(t)
        if value = null then FSharpValue.MakeUnion(cases.[0], [||])
        else FSharpValue.MakeUnion(cases.[1], [|value|])

type ListConverter() =
    inherit JsonConverter()
    
    override x.CanConvert(t:Type) = 
        t.IsGenericType && t.GetGenericTypeDefinition() = typedefof<list<_>>
 
    override x.WriteJson(writer, value, serializer) =
        let list = value :?> System.Collections.IEnumerable |> Seq.cast
        serializer.Serialize(writer, list)
 
    override x.ReadJson(reader, t, _, serializer) = 
        let itemType = t.GetGenericArguments().[0]
        let collectionType = typedefof<IEnumerable<_>>.MakeGenericType(itemType)
        let collection = serializer.Deserialize(reader, collectionType) :?> IEnumerable<_>        
        let listType = typedefof<list<_>>.MakeGenericType(itemType)
        let cases = FSharpType.GetUnionCases(listType)
        let rec make = function
            | [] -> FSharpValue.MakeUnion(cases.[0], [||])
            | head::tail -> FSharpValue.MakeUnion(cases.[1], [| head; (make tail); |])                    
        make (collection |> Seq.toList)

type TupleArrayConverter() =
    inherit JsonConverter()
    
    override x.CanConvert(t:Type) = 
        FSharpType.IsTuple(t)
 
    override x.WriteJson(writer, value, serializer) =
        let values = FSharpValue.GetTupleFields(value)
        serializer.Serialize(writer, values)
 
    override x.ReadJson(reader, t, _, serializer) =
        let advance = reader.Read >> ignore
        let deserialize t = serializer.Deserialize(reader, t)
        let itemTypes = FSharpType.GetTupleElements(t)
 
        let readElements() =
            let rec read index acc =
                match reader.TokenType with
                | JsonToken.EndArray -> 
                    List.rev acc
                | _ ->
                    let value = deserialize(itemTypes.[index])
                    advance()
                    read (index + 1) (value :: acc)
            advance()
            read 0 List.empty
 
        match reader.TokenType with
        | JsonToken.StartArray ->
            let values = readElements()
            FSharpValue.MakeTuple(values |> List.toArray, t)
        | _ -> failwith "invalid token"

type UnionTypeConverter() =
    inherit JsonConverter()
 
    let doRead (reader: JsonReader) = 
        reader.Read() |> ignore
 
    override x.CanConvert(typ:Type) =
        let result = 
            ((typ.GetInterface(typeof<System.Collections.IEnumerable>.FullName) = null) 
            && FSharpType.IsUnion typ)
        result
 
    override x.WriteJson(writer: JsonWriter, value: obj, serializer: JsonSerializer) =
        let t = value.GetType()
        let write (name : string) (fields : obj[]) = 
            writer.WriteStartObject()
            writer.WritePropertyName("case")
            writer.WriteValue(name)  
            writer.WritePropertyName("values")
            serializer.Serialize(writer, fields)
            writer.WriteEndObject()   
 
        let (info, fields) = FSharpValue.GetUnionFields(value, t)
        write info.Name fields
 
    override x.ReadJson(reader: JsonReader, objectType: Type, existingValue: obj, serializer: JsonSerializer) =      
         let cases = FSharpType.GetUnionCases(objectType)
         if reader.TokenType <> JsonToken.Null  
         then
            doRead reader
            doRead reader
            let case = cases |> Array.find (fun x -> x.Name = if reader.Value = null then "None" else reader.Value.ToString())
            doRead reader
            doRead reader
            doRead reader
            let fields =  [| 
                for field in case.GetFields() do
                    let result = serializer.Deserialize(reader, field.PropertyType)
                    reader.Read() |> ignore
                    yield result
            |] 
            let result = FSharpValue.MakeUnion(case, fields)
            while reader.TokenType <> JsonToken.EndObject do
                doRead reader         
            result
         else
            FSharpValue.MakeUnion(cases.[0], [||]) 