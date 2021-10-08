using System;
using System.Collections.Generic;

namespace Library
{
    /*
        EXPERT: La clase Request cumple con el patrón Expert ya que al ser la clase 
        experta en conocer la información necesaria para crear un Request, es su 
        responsabilidad actualizar el valor de sus propiedades.

        SRP: La clase Request cumple con el principio SRP ya que no tiene más de 
        una razón de cambio, la cual sería modificar la forma en la que se actualiza 
        la propiedad CurrentState.

    */
    public class Request : IRequest
    {
        public string CurrentState { get; private set; }
        public long RequestId {get;private set;}
        public string ButtonSelected {get; private set;}

        public bool Clicked{get; private set;}

        // public delegate void InputReady();
        // public static event InputReady onCallbackUpdated;

        public Request(string state, long requestId)
        {
            this.CurrentState = state;
            this.RequestId = requestId;
            this.Clicked = false;
        }

        public void UpdateCurrentState(string state)
        {
            this.CurrentState = state;
        }

        public void UpdateButtonSelected(string sel)
        {
            this.ButtonSelected = sel;
        }

        public void UpdateClicked(bool b)
        {
            this.Clicked = b;
        }
 
    }
}
