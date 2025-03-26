using System.Diagnostics;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Windows.Threading;
using ProvaHidrica.Database;
using ProvaHidrica.Models;
using ProvaHidrica.Windows;
using SocketIOClient;
using Timer = System.Timers.Timer;

namespace ProvaHidrica.Services
{
    static class Api
    {
        private static readonly HttpClient client = new();
        private static readonly SocketIOClient.SocketIO socket = new("https://localhost:4000");
        private static readonly Db db;
        private static readonly Timer timer;

        static Api()
        {
            DbConnectionFactory connectionFactory = new();
            db = new(connectionFactory);

            timer = new Timer(1000);
            timer.Start();

            Debug.WriteLine("Api initialized.");
        }

        public static void Init()
        {
            ConnectWebSocket();
        }

        private static void ConnectWebSocket()
        {
            try
            {
                socket.OnConnected += static (sender, e) =>
                {
                    _ = Task.Run(static () =>
                    {
                        timer.Elapsed += static async (sender, e) =>
                            await SendMessageAsync("backgroundServiceInitialized", new());

                        timer.AutoReset = true;
                        return Task.CompletedTask;
                    });
                    Debug.WriteLine("WebSocket connected successfully.");
                };

                socket.OnDisconnected += (sender, e) =>
                {
                    Debug.WriteLine("WebSocket disconnected.");
                };

                Listen(socket);

                socket.ConnectAsync().Wait();
            }
            catch (AggregateException ex)
            {
                foreach (var innerEx in ex.InnerExceptions)
                {
                    Debug.WriteLine($"WebSocket connection failed: {innerEx.Message}");
                    throw innerEx;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"WebSocket connection failed: {ex.Message}");
                throw;
            }
        }

        private static void Listen(SocketIOClient.SocketIO socket)
        {
            socket.On(
                "createUser",
                static response =>
                {
                    CreateUser(response);
                }
            );
            socket.On(
                "authenticate",
                static response =>
                {
                    LoadUser();
                }
            );
            socket.On(
                "sendingBarcode",
                static response =>
                {
                    Task.Run(async () => await LoadRecipe(response));
                }
            );
        }

        private static void CreateUser(SocketIOResponse response)
        {
            Debug.WriteLine(response.ToString());
            var userJson = response.GetValue<JsonElement>(0);
            string? badgeNumber = userJson.GetProperty("BadgeNumber").GetString();
            string? userName = userJson.GetProperty("UserName").GetString();
            List<string>? permissions = JsonSerializer.Deserialize<List<string>>(
                userJson.GetProperty("Permissions").GetRawText()
            );

            if (badgeNumber != null && userName != null && permissions != null)
            {
                User user = new(String.Empty, badgeNumber, userName, permissions);

                Thread staThread =
                    new(
                        (userObj) =>
                        {
                            NfcWindow nfcWindow = new(Types.Context.Create, userObj as User);
                            nfcWindow.Closed += (s, e) =>
                                Dispatcher.CurrentDispatcher.InvokeShutdown();
                            nfcWindow.ShowDialog();
                        }
                    );
                staThread.SetApartmentState(ApartmentState.STA);
                staThread.Start(user);
            }
            else
            {
                Debug.WriteLine("Invalid user data received.");
            }
        }

        private static void LoadUser()
        {
            Thread staThread =
                new(
                    (userObj) =>
                    {
                        NfcWindow nfcWindow = new(Types.Context.Login, userObj as User);
                        nfcWindow.Closed += (s, e) => Dispatcher.CurrentDispatcher.InvokeShutdown();
                        nfcWindow.ShowDialog();
                    }
                );
            staThread.SetApartmentState(ApartmentState.STA);
            staThread.Start();
        }

        private static async Task LoadRecipe(SocketIOResponse response)
        {
            Recipe? recipe = await db.GetRecipeByVp(response.GetValue<string>(0));

            if (recipe != null)
            {
                await SendMessageAsync("recipeLoaded", recipe);
                return;
            }

            await SendMessageAsync("error", new { message = "Receita não cadastrada." });
        }

        public static async Task SendMessageAsync(string eventName, object data)
        {
            try
            {
                if (socket.Connected)
                {
                    await socket.EmitAsync(eventName, data);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Failed to send message: {ex.Message}");
            }
        }

        public static async Task<bool> Authenticate(string badgeNumber)
        {
            var content = new StringContent(
                JsonSerializer.Serialize(new { badgeNumber }),
                Encoding.UTF8,
                "application/json"
            );

            var response = await client.PostAsync(
                "https://prova-hidrica:4000/api/users/authenticate",
                content
            );

            if (response.IsSuccessStatusCode)
            {
                // Enviar mensagem ao WebSocket após autenticação bem-sucedida
                await SendMessageAsync("userAuthenticated", new { badgeNumber });
            }

            return response.IsSuccessStatusCode;
        }
    }
}
