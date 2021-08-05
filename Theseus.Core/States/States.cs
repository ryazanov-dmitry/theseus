namespace Theseus.Core.States
{
    public class States
    {
        public static class Service
        {
            public readonly static string NoOrder = "No Order";
            public readonly static string SentDkgRequest = "Sent Dkg Request";
            public readonly static string WaitingForDkgPub = "Waiting For DkgPub";
            public readonly static string WaitingForContract = "Waiting For Contract";
            public readonly static string StartedDelivery = "Started Delivery";
        }

        public static class Verifier
        {
            public readonly static string NoDkgSession = "No Dkg Session";
            public readonly static string InitedDkgSession = "Potential Dkg Session";
        }

        
    }
}