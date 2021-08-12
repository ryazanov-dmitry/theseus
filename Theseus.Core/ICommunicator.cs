namespace Theseus.Core
{
    public interface IDKGCommunicator
    {
        void SendDKGPub(string v, string serviceId);
    }
}