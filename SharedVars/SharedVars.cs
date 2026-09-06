namespace SharedVars
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.IO.Pipes;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    public static class MinimizeMessenger
    {

        public static bool showFirmware = true;
        // Server (Wisej) side - Listen for incoming messages
        public static async Task StartListeningAsync(Action<string> messageHandler, CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                using (var server = new NamedPipeServerStream("MinimizePipe", PipeDirection.In, 100, PipeTransmissionMode.Byte, PipeOptions.Asynchronous))
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

        public static void firmwareToggle()
        {
            showFirmware = false;
            Console.WriteLine($"[MinimizeMessenger] Firmware visibility toggled to: {showFirmware}");
        }


        // Client (MAUI) side - Send messages to the server
        public static async Task SendMessageAsync(string message, int maxRetries = 10, int retryDelayMs = 200)
        {
            int attempt = 0;

            while (attempt < maxRetries)
            {
                Debug.WriteLine($"[MinimizeMessenger] Attempt {attempt + 1} of {maxRetries}"); // Add this line

                try
                {
                    using (var client = new NamedPipeClientStream(".", "MinimizePipe", PipeDirection.Out, PipeOptions.Asynchronous))
                    {
                        await client.ConnectAsync(1000); // Increased timeout to 1000ms

                        using (var writer = new StreamWriter(client))
                        {
                            await writer.WriteLineAsync(message);
                            Debug.WriteLine($"[MinimizeMessenger] Sent message: {message}");
                            await writer.FlushAsync();
                        }
                    }

                    // Success, exit loop
                    return;
                }
                catch (TimeoutException ex)
                {
                    attempt++;
                    Debug.WriteLine($"[MinimizeMessenger] Timeout connecting to server (attempt {attempt}/{maxRetries}): {ex.Message}");

                    if (attempt >= maxRetries)
                    {
                        Debug.WriteLine("[MinimizeMessenger] Giving up after max retries.");
                        return;
                    }

                    await Task.Delay(retryDelayMs);
                }
                catch (IOException ex)
                {
                    attempt++;
                    if (ex.Message.Contains("timed out"))
                    {
                        Debug.WriteLine($"[MinimizeMessenger] IO timeout connecting to server (attempt {attempt}/{maxRetries}): {ex.Message}");
                    }
                    else
                    {
                        Debug.WriteLine($"[MinimizeMessenger] IO error: {ex.Message}");
                    }

                    if (attempt >= maxRetries)
                    {
                        Debug.WriteLine("[MinimizeMessenger] Giving up after max retries.");
                        return;
                    }
                    await Task.Delay(retryDelayMs);
                }
                catch (Exception ex)
                {
                    attempt++;
                    Debug.WriteLine($"[MinimizeMessenger] Unexpected error: {ex.Message}");

                    if (attempt >= maxRetries)
                    {
                        Debug.WriteLine("[MinimizeMessenger] Giving up after max retries.");
                        return;
                    }
                    await Task.Delay(retryDelayMs);
                }
            }
        }

    }

    public static class PlayerAssignment
    {
        private const string RegistryMutexName = "Local\\OverjoyedRelease.PlayerAssignment";
        private static readonly object syncRoot = new object();
        private static int assignedPlayer = 1;
        private static bool hasAssignedPlayer;

        public static int ClaimNextAvailablePlayer(string configPath, bool multiMode)
        {
            if (string.IsNullOrWhiteSpace(configPath))
            {
                throw new ArgumentException("Config path is required.", nameof(configPath));
            }

            using (var registryMutex = new Mutex(false, RegistryMutexName))
            {
                WaitForRegistryMutex(registryMutex);

                try
                {
                    string playerPath = Path.Combine(configPath, "player.txt");
                    var players = ReadRegisteredPlayers(playerPath);

                    if (players.Count > 0 && !multiMode)
                    {
                        return 0;
                    }

                    for (int player = 1; player <= 4; player++)
                    {
                        if (players.Add(player))
                        {
                            WriteRegisteredPlayers(playerPath, players);
                            return player;
                        }
                    }

                    return 0;
                }
                finally
                {
                    try { registryMutex.ReleaseMutex(); } catch (ApplicationException) { }
                }
            }
        }

        public static void ReleasePlayer(string configPath, int player)
        {
            if (string.IsNullOrWhiteSpace(configPath) || player < 1 || player > 4)
            {
                return;
            }

            using (var registryMutex = new Mutex(false, RegistryMutexName))
            {
                WaitForRegistryMutex(registryMutex);

                try
                {
                    string playerPath = Path.Combine(configPath, "player.txt");
                    var players = ReadRegisteredPlayers(playerPath);

                    if (!players.Remove(player))
                    {
                        return;
                    }

                    WriteRegisteredPlayers(playerPath, players);
                }
                finally
                {
                    try { registryMutex.ReleaseMutex(); } catch (ApplicationException) { }
                }
            }
        }

        public static void SetAssignedPlayer(int player)
        {
            if (player < 1 || player > 4)
            {
                throw new ArgumentOutOfRangeException(nameof(player));
            }

            lock (syncRoot)
            {
                assignedPlayer = player;
                hasAssignedPlayer = true;
            }
        }

        public static bool TryGetAssignedPlayer(out int player)
        {
            lock (syncRoot)
            {
                player = assignedPlayer;
                return hasAssignedPlayer;
            }
        }

        public static int GetAssignedPlayerOrDefault(int defaultPlayer = 1)
        {
            lock (syncRoot)
            {
                return hasAssignedPlayer ? assignedPlayer : defaultPlayer;
            }
        }

        private static SortedSet<int> ReadRegisteredPlayers(string playerPath)
        {
            var players = new SortedSet<int>();

            if (!File.Exists(playerPath))
            {
                return players;
            }

            string content = File.ReadAllText(playerPath);

            for (int player = 1; player <= 4; player++)
            {
                if (content.IndexOf($"player{player}", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    players.Add(player);
                }
            }

            return players;
        }

        private static void WriteRegisteredPlayers(string playerPath, IEnumerable<int> players)
        {
            var orderedPlayers = players.OrderBy(player => player).ToArray();

            if (orderedPlayers.Length == 0)
            {
                File.WriteAllText(playerPath, string.Empty);
                return;
            }

            string content = string.Join(" ", orderedPlayers.Select(player => $"player{player}"));
            File.WriteAllText(playerPath, content);
        }

        private static void WaitForRegistryMutex(Mutex registryMutex)
        {
            try
            {
                registryMutex.WaitOne();
            }
            catch (AbandonedMutexException)
            {
            }
        }
    }
}
