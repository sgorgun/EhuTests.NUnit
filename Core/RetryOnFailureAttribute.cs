using NUnit.Framework;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using NUnit.Framework.Internal.Commands;

namespace EhuTests.NUnit.Core;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
public class RetryOnFailureAttribute(int retryCount = 2) : PropertyAttribute, IWrapSetUpTearDown
{
    private readonly int _retryCount = retryCount;

    public TestCommand Wrap(TestCommand command)
    {
        return new RetryCommand(command, _retryCount);
    }

    private class RetryCommand(TestCommand innerCommand, int retryCount) : DelegatingTestCommand(innerCommand)
    {
        private readonly int _retryCount = retryCount;

        public override TestResult Execute(TestExecutionContext context)
        {
            int count = 0;
            while (true)
            {
                context.CurrentResult = innerCommand.Execute(context);
                count++;

                if (context.CurrentResult.ResultState.Status == TestStatus.Passed)
                    break;

                if (count > _retryCount)
                    break;

                context.CurrentResult = context.CurrentTest.MakeTestResult();
            }
            return context.CurrentResult;
        }
    }
}
