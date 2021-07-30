namespace Theseus.Core.States
{
    public interface IRequestAndStateValidator
    {
        bool IsNotValid(object request);
        void Transition(object request);
    }
}