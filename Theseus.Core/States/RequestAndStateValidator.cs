using Theseus.Core.Classificators;

namespace Theseus.Core.States
{
    public class RequestAndStateValidator : IRequestAndStateValidator
    {

        public RequestAndStateValidator(NodeType nodeType)
        {
            
        }

        public bool IsNotValid(object request)
        {
            throw new System.NotImplementedException();
        }

        public void Transition(object request)
        {
            throw new System.NotImplementedException();
        }
    }
}