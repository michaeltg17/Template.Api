using System;
using System.Text;

namespace IntegrationTests
{
    internal class IntegrationTestsException(string message) : Exception(message)
    {
    }
}
