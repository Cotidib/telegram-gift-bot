using System;
using System.Threading;
using System.Threading.Tasks;

namespace Library
{
    /*
        POLIMORFISMO: Se aplica el patrón polimorfismo ya que las clases 
        que implementan el tipo IStateHandler implementan sus operaciones polimórficas.

        SRP: La clase cumple con el principio SRP ya que no tiene más de 
        una razón de cambio, la cual sería modificar la forma en la que se gestiona 
        la fase de preguntas mixtas.

    */
    public class AskMixedQuestionStateHandler : AbstractStateHandler
    {
        public async override Task<object> Handle(IRequest request, IPersonProfile user, IMessageReceiver input, IMessageSender output, ISearchGift searcher, IStorage storage)
        {
            if (request.CurrentState == "mixed")
            {

                if (storage.GetMixedCompleted)
                {

                    foreach (MixedCategory category in storage.MixedCategoriesSelected)
                    {
                        await output.SendMessageAsync(category.Question, request.RequestId);
                        await output.SendMessageAnswersAsync(category.AnswerOptions, request.RequestId);
                        
                        string ans = await input.GetInputAsync(request.RequestId);
                        Console.WriteLine("La respuesta fue " + ans);
                        storage.AnswersMixedQuestions.Add(category.Question, category.AnswerOptions[ans]);
                    }

                    if (storage.MixedCategoriesSelected.Count == storage.AnswersMixedQuestions.Count)
                    {
                        storage.UpdateAskMixedCompleted(true);
                        await output.SendMessageAsync("Se ha finalizado la fase de preguntas mixtas", request.RequestId);
                        request.UpdateCurrentState("specific");
                    }

                    return base.Handle(request, user, input, output, searcher, storage);
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }
    }
}