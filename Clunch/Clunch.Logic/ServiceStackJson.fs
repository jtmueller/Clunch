namespace Clunch

open System
open System.Net.Http.Formatting
open System.Net.Http.Headers
open System.Threading.Tasks
open System.Web.Http
open System.Text
open ServiceStack.Text

type ServiceStackTextFormatter() as this =
    inherit MediaTypeFormatter()

    do
        JsConfig.DateHandler <- JsonDateHandler.ISO8601
        this.SupportedMediaTypes.Add(MediaTypeHeaderValue("application/json"))
        this.SupportedEncodings.Add(UTF8Encoding(encoderShouldEmitUTF8Identifier=false, throwOnInvalidBytes=true))
        this.SupportedEncodings.Add(UnicodeEncoding(bigEndian=false, byteOrderMark=true, throwOnInvalidBytes=true))

    override x.CanReadType theType =
        if isNull theType then nullArg "theType"
        true

    override x.CanWriteType theType =
        if isNull theType then nullArg "theType"
        true

    override x.ReadFromStreamAsync(theType, readStream, content, formatterLogger) =
        Task<obj>.Factory.StartNew(fun () -> JsonSerializer.DeserializeFromStream(theType, readStream))

    override x.WriteToStreamAsync(theType, value, writeStream, content, context) =
        Task.Factory.StartNew(fun () -> JsonSerializer.SerializeToStream(value, theType, writeStream))

    static member Register (config:HttpConfiguration) =
        config.Formatters.RemoveAt 0
        config.Formatters.Insert(0, new ServiceStackTextFormatter())


type ServiceStackSerializer() =
    
    interface Microsoft.AspNet.SignalR.Json.IJsonSerializer with
        member x.Parse(json, theType) =
            JsonSerializer.DeserializeFromString(json, theType)

        member x.Serialize(value, writer) =
            JsonSerializer.SerializeToWriter(value, writer)

        