open System.Text
open RabbitMQ.Client

let getMessage (args: string[]) : string =
    if args.Length > 0 then
        args |> Array.reduce (fun acc a -> $"{acc} {a}")
    else
        "Hello World!"

[<EntryPoint>]
let main (args: string[]) : int =
    // RabbitMQ default ports:
    // - Message broker = 5672
    // - Front-end      = 15672
    let factory = new ConnectionFactory()
    factory.HostName <- "localhost"

    // The connection abstracts the socket connection, and takes care of protocol version
    // negotiation and authentication and so on for us.
    let connection = factory.CreateConnection()

    // The channel is where most of the API for getting things done resides.
    let channel = connection.CreateModel()

    let arguments = Seq.empty |> dict

    // Declaring a queue is idempotent - it will only be created if it doesn't exist already.
    channel.QueueDeclare(
        queue = "task_queue",
        durable = true,
        exclusive = false,
        autoDelete = false,
        arguments = arguments
    )
    |> ignore

    let message = getMessage args
    let body = Encoding.UTF8.GetBytes message

    let properties = channel.CreateBasicProperties()
    properties.Persistent <- true

    channel.BasicPublish(exchange = "", routingKey = "task_queue", basicProperties = properties, body = body)

    printfn $"[x] Sent {message}"
    printfn $"Press [enter] to exit."
    System.Console.ReadLine() |> ignore

    0
