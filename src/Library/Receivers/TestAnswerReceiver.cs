using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Library
{
    /*
        EXPERT: La clase cumple con el patrón Expert ya que es la clase experta 
        en conocer la información para crear una respuesta pre-hecha.

        SRP: La clase cumple con el principio SRP ya que existe una sola razón de cambio.

        ISP: La clase cumple con el principio ISP ya que no depende de un tipo que no usa.
    */

    public class TestAnswerReceiver : IMessageReceiver
    {
        public string Text { get; private set; }

        public TestAnswerReceiver(string text)
        {
            this.Text=text;
            
        }
        private async Task<string> GetTextAsync()
        {
            return this.Text;
        }
        public async Task<string> GetInputAsync(long requestId)
        {
            if(this.Text=="1")
                    this.Text = "2";
                else
                    this.Text="1";
            string answer = await GetTextAsync();
            return answer;
            
        }
    }
}
