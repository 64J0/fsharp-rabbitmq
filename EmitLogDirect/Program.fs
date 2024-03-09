open System.Text
open RabbitMQ.Client

let getMessage (args: string[]) : string =
    if args.Length > 1 then
        args |> Array.skip 1 |> Array.reduce (fun acc a -> $"{acc} {a}")
    else
        "Hello World!"

[<EntryPoint>]
let main (args: string[]) : int =
    let factory = new ConnectionFactory()
    factory.HostName <- "localhost"

    let connection = factory.CreateConnection()
    let channel = connection.CreateModel()

    // publishing to a non-existing exchange is forbidden
    channel.ExchangeDeclare(exchange = "direct_logs", ``type`` = ExchangeType.Direct)

    // we don't declare the queue here

    let message = getMessage args
    let severity = if args.Length > 0 then args[0] else "info"
    let body = Encoding.UTF8.GetBytes message

    channel.BasicPublish(exchange = "direct_logs", routingKey = severity, basicProperties = null, body = body)
    printfn $"[x] Sent '{severity}'.'{message}'"

    printfn $"Press [enter] to exit."
    System.Console.ReadLine() |> ignore

    0
