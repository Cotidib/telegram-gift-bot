using System;
using System.Threading;
using System.Threading.Tasks;

namespace Library
{
    /*
        POLIMORFISMO: Se aplica el patrón polimorfismo ya que las clases que implementan 
        el tipo IStateHandler implementan sus operaciones polimórficas.

        SRP: La clase cumple con el principio SRP ya que no tiene más de una razón de cambio, 
        la cual sería modificar la forma en la que se gestiona la fase de preguntas iniciales.

    */

    public class AskInitialQuestionStateHandler : AbstractStateHandler
    {
        public async override Task<object> Handle(IRequest request, IPersonProfile user, IMessageReceiver input, IMessageSender output, ISearchGift searcher, IStorage storage)
        {
            if (request.CurrentState == "initial")
            {
                foreach (InitialQuestion initialQ in CoreBot.Instance.Reader.InitialQuestionsBank)
                {
                    await output.SendMessageAsync(initialQ.Question, request.RequestId);
                    await output.SendMessageAnswersAsync(initialQ.AnswerOptions, request.RequestId);
                    string ans = await input.GetInputAsync(request.RequestId);
                    Console.WriteLine("La respuesta fue " + ans);
                    user.UpdatePreferences(initialQ.AnswerOptions[ans]);
                }

                if (user.Preferences.Count == CoreBot.Instance.Reader.InitialQuestionsBank.Count)
                {
                    storage.UpdateAskInitialCompleted(true);
                    await output.SendMessageAsync("Se ha finalizado la fase de preguntas iniciales", request.RequestId);
                    request.UpdateCurrentState("main");
                }

                return base.Handle(request, user, input, output, searcher, storage);
            }
            else
            {
                return null;
            }

        }
    }
}