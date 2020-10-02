using Theseus.Core.Crypto;

namespace Theseus.Core.Tests
{
    public class Common
    {
        public static IAuthentication CreateAuth()
        {
            return new Authentication(new RSA());
        }
    }
}