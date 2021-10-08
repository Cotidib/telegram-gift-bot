using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Library
{
    /*
        EXPERT: La clase cumple con el patrón Expert ya que es la clase 
        experta en obtener información del usuario a través de la consola.

        SRP: La clase cumple con el principio SRP ya que existe una sola razón de cambio.
    */

    public class ConsoleReceiver : IMessageReceiver
    {
        public Task<string> GetInputAsync(long requestId)
        {
            return Task.Run( () => Console.ReadLine());
        }
    }
}
