open System.Text
open RabbitMQ.Client

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
channel.QueueDeclare(queue = "hello", durable = false, exclusive = false, autoDelete = false, arguments = arguments)
|> ignore

let message = "Hello World!"
let body = Encoding.UTF8.GetBytes message

let basicProperties = arguments
channel.BasicPublish(exchange = "", routingKey = "hello", basicProperties = null, body = body)

printfn $"[x] Sent {message}"
printfn $"Press [enter] to exit."
System.Console.ReadLine() |> ignore
