namespace Theseus.Core.Tests.Factories
{
    public static class DKGClientFactory
    {
        public static DKGClient CreateDkgClient(IDKGCommunicator dkgComm)
        {
            return new DKGClient(
                null,
                null,
                null,
                null,
                null,
                dkgComm);
        }
    }
}