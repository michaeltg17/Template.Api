using System;
using System.Collections.Generic;
using System.Text;

namespace IntegrationTests
{
    internal class IntegrationTestsException(string message) : Exception(message)
    {
    }
}
