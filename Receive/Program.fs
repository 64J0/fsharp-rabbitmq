open System
open System.Text
open RabbitMQ.Client
open RabbitMQ.Client.Events

let factory = new ConnectionFactory()
factory.HostName <- "localhost"

let connection = factory.CreateConnection()
let channel = connection.CreateModel()

let arguments = Seq.empty |> dict

// Declaring a queue is idempotent - it will only be created if it doesn't exist already.
channel.QueueDeclare(queue = "hello", durable = false, exclusive = false, autoDelete = false, arguments = arguments)
|> ignore

printfn "[*] Waiting for messages."

let consumer = new EventingBasicConsumer(channel)

consumer.Received.AddHandler(fun _model ea ->
    let body = ea.Body.ToArray()
    let message = Encoding.UTF8.GetString(body)
    printfn $"[x] Received {message}")

channel.BasicConsume(queue = "hello", autoAck = true, consumer = consumer)
|> ignore

printfn "Press [enter] to exit."
Console.ReadLine() |> ignore
