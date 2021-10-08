using System;
using Library;
using LibraryAPI;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using System.IO;
using System.Text;
using System.Collections.Generic;
using Telegram.Bot.Types.ReplyMarkups;
using System.Linq;
using System.Threading.Tasks;

namespace Library
{
    /* 
        POLIMORFISMO: Se cumple con el patrón de polimorfismo ya que las clases 
        está implementando las operaciones polimórficas de sus interfaces.

        SRP: La clase no cumple con el principio SRP ya que tiene más de una razón de cambio
        las cuales pueden ser: 
        -Modificar la forma en la que se envian mensajes a Telegram.
        -Modificar la forma en la que se reciben mensajes desde Telegram.

        ISP: La clase cumple con el principio ISP ya que no depende de un tipo que no usa.

        CHAIN OF RESPONSIBILITY: En esta clase se aplica el patrón Chain of Responsibility 
        para que varios objetos gestionen las diferentes comandos y así favorecer la extensibilidad.

        ADAPTER: La clase implementa el patrón Adapter para transformar a un formato 
        reconocible de manejo de mensajes (inputs y outputs) para el core del bot.
    */

    public class TelegramGateway : IMessageSender, IMessageReceiver
    {
        private static ICommandHandler commandsCommandHandler;
        private static ICommandHandler startCommandHandler;
        private static ICommandHandler searchMyGiftCommandHandler;
        private static ICommandHandler aboutCommandHandler;
        private static ICommandHandler commandNotFoundHandler;
        public static void RunTelegramAPI()
        {
            TelegramBot telegramBot = TelegramBot.Instance;
            Console.WriteLine($"Hola soy el Bot de P2, mi nombre es {telegramBot.BotName} y tengo el Identificador {telegramBot.BotId}");
            ITelegramBotClient bot = telegramBot.Client;
            bot.OnMessage += OnMessage;
            bot.OnCallbackQuery += BotOnCallbackQueryRecieved;
            bot.StartReceiving();
            Console.WriteLine("Presiona una tecla para terminar");
            Console.ReadKey();
            bot.StopReceiving();
        }
        private static void OnMessage(object sender, MessageEventArgs messageEventArgs)
        {
            ITelegramBotClient client = TelegramBot.Instance.Client;
            Message message = messageEventArgs.Message;
            Chat chatInfo = message.Chat;
            string messageText = message.Text.ToLower();

            //Agrega usuarios nuevos al diccionario UserSessions de CoreBot
            if (!CoreBot.Instance.UserSessions.ContainsKey(chatInfo.Id))
            {
                IRequest request = new Request("initial", chatInfo.Id);
                CoreBot.Instance.AddUserSessions(chatInfo.Id, request);
            }

            //Construye cadena de responsabilidad para manejar los comandos
            commandsCommandHandler = new CommandsCommandHandler();
            startCommandHandler = new StartCommandHandler();
            searchMyGiftCommandHandler = new SearchGiftCommandHandler();
            aboutCommandHandler = new AboutCommandHandler();
            commandNotFoundHandler = new CommandNotFoundHandlder();

            commandsCommandHandler.SetNext(startCommandHandler);
            startCommandHandler.SetNext(searchMyGiftCommandHandler);
            searchMyGiftCommandHandler.SetNext(aboutCommandHandler);
            aboutCommandHandler.SetNext(commandNotFoundHandler);

            //Envia el mensaje recibido al primer eslabon de la cadena de responsabilidad
            commandsCommandHandler.Handle(messageText, chatInfo.Id);
        }
  
        public async Task<string> GetInputAsync(long requestId)
        {
            //Recibe el valor de callback del botón y lo envia como input de usuario
            //Tengo que esperar a que se ejecute UpdateButtonSelected para devolver el valor!!!!!!!!
            while(CoreBot.Instance.UserSessions[requestId].Clicked == false)
            {

            }
            return CoreBot.Instance.UserSessions[requestId].ButtonSelected;
        }

        public async Task SendMessageAsync(string message, long requestId)
        {
           await SendMessageTelegramAdapter(message, requestId);
        }

        private async Task SendMessageTelegramAdapter(string message, long requestId)
        {
            CoreBot.Instance.UserSessions[requestId].UpdateClicked(false);
            //Imprime un mensaje en el chat de Telegram
            ITelegramBotClient client = TelegramBot.Instance.Client;
            await client.SendTextMessageAsync(chatId: requestId, text: message);
        }

        public async Task SendMessageAnswersAsync(Dictionary<string, string> ans, long requestId)
        { 
            CoreBot.Instance.UserSessions[requestId].UpdateButtonSelected("");
            await SendMessageAnswersAdapter(ans, requestId);
        }

        private async Task SendMessageAnswersAdapter(Dictionary<string, string> ans, long requestId) //antes era void
        {
            ITelegramBotClient client = TelegramBot.Instance.Client;

            //Mostrar botones de opciones de respuesta
            var rows = new List<List<InlineKeyboardButton>>();

            foreach (var index in ans)
            {
                InlineKeyboardButton button = InlineKeyboardButton.WithCallbackData(text: index.Value, callbackData: index.Key);

                rows.Add(
                    new List<InlineKeyboardButton>
                    {
                         button
                    });

            }
            var keyBoard = new InlineKeyboardMarkup(rows);

            await client.SendTextMessageAsync(
                requestId,
                "Seleccione una de las siguientes opciones:",
                replyMarkup: keyBoard
            );
        }

        private static async void BotOnCallbackQueryRecieved(object sender, CallbackQueryEventArgs callbackQueryEventArgs)
        {
            var callbackQuery = callbackQueryEventArgs.CallbackQuery;

            ITelegramBotClient client = TelegramBot.Instance.Client;

            await client.SendTextMessageAsync(
                    callbackQuery.Message.Chat.Id,
                    $"¡Entendido!"
                );

           
            CoreBot.Instance.UserSessions[callbackQuery.Message.Chat.Id].UpdateButtonSelected(callbackQuery.Data);
            CoreBot.Instance.UserSessions[callbackQuery.Message.Chat.Id].UpdateClicked(true);
        }


    }
}
