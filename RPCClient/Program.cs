// See https://aka.ms/new-console-template for more information
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Grpc.Net.Client;
using SampleGrpc;

//Console.WriteLine("Hello, World!");

var channel = GrpcChannel.ForAddress("https://localhost:49153/");
var client = new Greeter.GreeterClient(channel);
var reply = await client.SayHelloAsync(new SampleGrpc.HelloRequest
{
    Name = "gRPC Demo"
});
Console.WriteLine("from server: " + reply);

try
{
    var cts = new CancellationTokenSource();
    using var streamingCall = client.GetTable(new Empty(), cancellationToken: cts.Token);

    await foreach (var tableRespnse in streamingCall.ResponseStream.ReadAllAsync(cancellationToken: cts.Token))
    {
        Console.WriteLine($"{tableRespnse.Timestamp.ToDateTime():s} | Num = {tableRespnse.Num}");
    }
}
catch (RpcException ex) when (ex.StatusCode == StatusCode.Cancelled)
{
    Console.WriteLine("Stream cancelled.");
}

Console.ReadLine();