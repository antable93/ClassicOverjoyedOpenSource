namespace SharedVars
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.IO.Pipes;
    using System.Threading;
    using System.Threading.Tasks;

    public static class ActiveMessenger
    {
        // Server (Wisej) side - Listen for incoming messages
        public static async Task StartListeningAsyncActive(Action<string> messageHandler, CancellationToken cancellationToken)
        {
            try
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    using (var server = new NamedPipeServerStream("ActivePipe", PipeDirection.In, 100, PipeTransmissionMode.Byte, PipeOptions.Asynchronous))
                    {
                        Console.WriteLine("Waiting for pipe connection...");

                        await server.WaitForConnectionAsync(cancellationToken);

                        using (var reader = new StreamReader(server))
                        {
                            while (!cancellationToken.IsCancellationRequested && server.IsConnected)
                            {
                                var message = await reader.ReadLineAsync();
                                if (message != null)
                                {
                                    messageHandler(message);
                                }
                            }
                        }
                    }
                }
            }
            catch (OperationCanceledException)
            {
                // Graceful shutdown: do nothing or log if needed
            }
        }

        // Client (MAUI) side - Send messages to the 3server
        public static async Task SendMessageAsyncActive(string message, int maxRetries = 10, int retryDelayMs = 200)
        {
            int attempt = 0;

            while (attempt < maxRetries)
            {
                Debug.WriteLine($"[ActiveMessenger] Attempt {attempt + 1} of {maxRetries}"); // Add this line

                try
                {
                    using (var client = new NamedPipeClientStream(".", "ActivePipe", PipeDirection.Out, PipeOptions.Asynchronous))
                    {
                        await client.ConnectAsync(1000); // Increased timeout to 1000ms

                        using (var writer = new StreamWriter(client))
                        {
                            await writer.WriteLineAsync(message);
                            await writer.FlushAsync();
                        }
                    }

                    // Success, exit loop
                    return;
                }
                catch (TimeoutException ex)
                {
                    attempt++;
                    Debug.WriteLine($"[ActiveMessenger] Timeout connecting to server (attempt {attempt}/{maxRetries}): {ex.Message}");

                    if (attempt >= maxRetries)
                    {
                        Debug.WriteLine("[ActiveMessenger] Giving up after max retries.");
                        return;
                    }

                    await Task.Delay(retryDelayMs);
                }
                catch (IOException ex)
                {
                    attempt++;
                    if (ex.Message.Contains("timed out"))
                    {
                        Debug.WriteLine($"[ActiveMessenger] IO timeout connecting to server (attempt {attempt}/{maxRetries}): {ex.Message}");
                    }
                    else
                    {
                        Debug.WriteLine($"[ActiveMessenger] IO error: {ex.Message}");
                    }

                    if (attempt >= maxRetries)
                    {
                        Debug.WriteLine("[ActiveMessenger] Giving up after max retries.");
                        return;
                    }
                    await Task.Delay(retryDelayMs);
                }
                catch (Exception ex)
                {
                    attempt++;
                    Debug.WriteLine($"[ActiveMessenger] Unexpected error: {ex.Message}");

                    if (attempt >= maxRetries)
                    {
                        Debug.WriteLine("[ActiveMessenger] Giving up after max retries.");
                        return;
                    }
                    await Task.Delay(retryDelayMs);
                }
            }
        
        }

    }
}
