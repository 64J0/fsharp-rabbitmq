open System.Text
open RabbitMQ.Client

let factory = new ConnectionFactory()
factory.HostName <- "localhost"

let connection = factory.CreateConnection()
let channel = connection.CreateModel()

let arguments = Seq.empty |> dict

// Declaring a queue is idempotent - it will only be created if it doesn't exist already.
channel.QueueDeclare(queue = "hello", durable = false, exclusive = false, autoDelete = false, arguments = arguments)
|> ignore

let message = "Hello World!"
let body = Encoding.UTF8.GetBytes message

let basicProperties = arguments
channel.BasicPublish(exchange = "", routingKey = "hello", basicProperties = null, body = body)

printfn $"[x] Sent {message}"
printfn $"Press [enter] to exit."
System.Console.ReadLine() |> ignore
