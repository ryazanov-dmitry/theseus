using System;
using Moq;
using Theseus.Core.Crypto;
using Theseus.Core.States;

namespace Theseus.Core.Tests
{
    public class Common
    {
        public static IAuthentication CreateAuth()
        {
            return new Authentication(new RSA());
        }

        public static IRequestAndStateValidator CreateStubState()
        {
            return new Mock<IRequestAndStateValidator>().Object;
        }
    }
}