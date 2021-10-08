using System;
using System.Threading;
using System.Threading.Tasks;

namespace Library
{
    /*
        POLIMORFISMO: Se aplica el patrón polimorfismo ya que las clases que 
        implementan el tipo IStateHandler implementan sus operaciones polimórficas.

        SRP: La clase cumple con el principio SRP ya que no tiene más de una razón 
        de cambio, la cual sería modificar la forma en la que se gestiona la fase de 
        preguntas principales.

    */
    
    public class AskMainQuestionStateHandler : AbstractStateHandler
    {
        public async override Task<object> Handle(IRequest request, IPersonProfile user, IMessageReceiver input, IMessageSender output, ISearchGift searcher, IStorage storage)
        {

            if (request.CurrentState == "main" && storage.AskInitialCompleted)
            {
                int contador = 1;
                await output.SendMessageAsync("Elije el número correspondiente a una de las afirmaciones. A la persona a la que quieres regalarle:", request.RequestId);
                foreach (MainCategory mainQ in CoreBot.Instance.Reader.MainCategoryBank)
                {
                    await output.SendMessageAsync(contador + "-" + mainQ.Question, request.RequestId);
                    storage.AnswersMainCategories.Add(contador.ToString(), mainQ.AnswerOptions[contador.ToString()]);
                    contador += 1;     
                }
                foreach (MainCategory mainQ in CoreBot.Instance.Reader.MainCategoryBank)
                {
                    await output.SendMessageAnswersAsync(mainQ.AnswerOptions, request.RequestId);
                }

                string ans = await input.GetInputAsync(request.RequestId);
                Console.WriteLine("La respuesta fue " + ans);
                user.UpdateSelectedCategory(storage.AnswersMainCategories[ans]);

                await output.SendMessageAsync("Selecciona una segunda opción adicional", request.RequestId);

                string ans2 = await input.GetInputAsync(request.RequestId);
                user.UpdateSelectedCategory(storage.AnswersMainCategories[ans2]);

                if (user.SelectedCategory.Count == 2)
                {
                    storage.UpdateAskMainCompleted(true);
                    await output.SendMessageAsync("Se ha finalizado la fase de preguntas principales", request.RequestId);
                    request.UpdateCurrentState("mixed");
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