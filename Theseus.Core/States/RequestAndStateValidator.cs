using System;
using System.Collections.Generic;
using System.Linq;
using Theseus.Core.Classificators;
using Theseus.Core.Dto;

namespace Theseus.Core.States
{
    public class RequestAndStateValidator : IRequestAndStateValidator
    {
        private readonly Dictionary<string, List<Link>> _stateInputs;
        private string _currentState;

        public RequestAndStateValidator(NodeType nodeType)
        {
            _stateInputs = new Dictionary<string, List<Link>>();

            if(nodeType == NodeType.Service)
                CreateStateForService();

            if (nodeType == NodeType.Verifier)
                CreateStateForVerifier();
        }

        

        public bool IsNotValid(object input)
        {
            var inputTypes = GetValidInputsOfCurrentState();

            if(input is string)
                return !inputTypes.Select(x => x.TargedState).Contains(input);

            return !inputTypes.Select(x=>x.LinkType).Contains(input.GetType());
        }

        private List<Link> GetValidInputsOfCurrentState()
        {
            return _stateInputs[_currentState];
        }

        public void Transition(object input)
        {
            if (IsNotValid(input))
                throw new InvalidOperationException(
                    $"Invalid input '{input.GetType().ToString()}' for the state '{_currentState}'.");

            if (input is string){
                _currentState = (string)input;
                return;
            }

            _currentState = GetValidInputsOfCurrentState().Single(x => x.LinkType == input.GetType()).NextState;
        }

        private void CreateStateForVerifier()
        {
            _currentState = States.Verifier.NoDkgSession;

            _stateInputs.Add(States.Verifier.NoDkgSession, new List<Link> {
                    new Link(typeof(DKGRequest), States.Verifier.NoDkgSession),
                    new Link(States.Verifier.InitedDkgSession)
                });
        }

        private void CreateStateForService()
        {
            _currentState = States.Service.NoOrder;

            _stateInputs.Add(States.Service.NoOrder, new List<Link> {
                    new Link(typeof(DeliveryRequest), States.Service.SentDkgRequest)
                });

            _stateInputs.Add(States.Service.SentDkgRequest, new List<Link> {
                    new Link(typeof(DKGPub), States.Service.WaitingForContract),
                    new Link(typeof(DeliveryContract), States.Service.WaitingForDkgPub)
                });

            _stateInputs.Add(States.Service.WaitingForDkgPub, new List<Link> {
                    new Link(typeof(DKGPub), States.Service.StartedDelivery)
                });

            _stateInputs.Add(States.Service.WaitingForContract, new List<Link> {
                    new Link(typeof(DeliveryContract), States.Service.StartedDelivery)
                });
        }
    }

    internal class Link
    {
        public string TargedState;
        public Type LinkType;
        public string NextState;

        public Link(string targedState)
        {
            this.TargedState = targedState;
        }

        public Link(Type linkType, string nextState)
        {
            this.LinkType = linkType;
            this.NextState = nextState;
        }
    }
}