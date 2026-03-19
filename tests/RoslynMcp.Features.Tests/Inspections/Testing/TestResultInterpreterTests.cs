using System.Reflection;
using Is.Assertions;
using RoslynMcp.Core.Models;
using RoslynMcp.Infrastructure;
using Xunit;

namespace RoslynMcp.Features.Tests.Inspections.Testing;

public sealed class TestResultInterpreterTests
{
    [Fact]
    public void Interpret_WhenTrxShowsPassedTests_ReturnsCounts()
    {
        using var fixture = new InterpreterFixture();
        var trxPath = fixture.WriteTrx(
            "passed.trx",
            """
            <TestRun xmlns="http://microsoft.com/schemas/VisualStudio/TeamTest/2010">
              <Results>
                <UnitTestResult testId="passed-id" testName="RunTestsSandbox.PassingTests.Passing_test" outcome="Passed" duration="00:00:01.2500000" />
              </Results>
              <TestDefinitions>
                <UnitTest id="passed-id" name="RunTestsSandbox.PassingTests.Passing_test">
                  <TestMethod className="RunTestsSandbox.PassingTests" name="Passing_test" />
                </UnitTest>
              </TestDefinitions>
              <ResultSummary outcome="Passed">
                <Counters total="1" executed="1" passed="1" failed="0" notExecuted="0" />
              </ResultSummary>
            </TestRun>
            """);

        var result = fixture.Interpret(CreateProcessResult(0), [trxPath]);

        result.Outcome.Is(RunTestOutcomes.Passed);
        result.FailureGroups.Count.Is(0);
        result.Counts.IsNotNull();
        result.Counts!.Total.Is(1);
        result.Counts.Executed.Is(1);
        result.Counts.Passed.Is(1);
        result.Counts.Failed.Is(0);
    }

    [Fact]
    public void Interpret_WhenTrxShowsFailedTests_ReturnsGroupedFailureDetailsFromTrx()
    {
        using var fixture = new InterpreterFixture();
        var trxPath = fixture.WriteTrx(
            "failed.trx",
            """
            <TestRun xmlns="http://microsoft.com/schemas/VisualStudio/TeamTest/2010">
              <Results>
                <UnitTestResult testId="failed-id" testName="RunTestsSandbox.Failures.Failing_test" outcome="Failed" duration="00:00:00.2500000">
                  <Output>
                    <ErrorInfo>
                      <Message>Assert.True() Failure</Message>
                      <StackTrace>at RunTestsSandbox.Failures.FailingTests.Failing_test() in /tmp/Tests.cs:line 42</StackTrace>
                    </ErrorInfo>
                  </Output>
                </UnitTestResult>
              </Results>
              <TestDefinitions>
                <UnitTest id="failed-id" name="RunTestsSandbox.Failures.Failing_test">
                  <TestMethod className="RunTestsSandbox.Failures.FailingTests" name="Failing_test" />
                </UnitTest>
              </TestDefinitions>
              <ResultSummary outcome="Failed">
                <Counters total="1" executed="1" passed="0" failed="1" notExecuted="0" />
              </ResultSummary>
            </TestRun>
            """);

        var result = fixture.Interpret(CreateProcessResult(1), [trxPath]);

        result.Outcome.Is(RunTestOutcomes.TestFailures);
        result.FailureGroups.Count.Is(1);
        result.FailureGroups[0].File.Is("/tmp/Tests.cs");
        result.FailureGroups[0].Count.Is(1);
        result.FailureGroups[0].Failures.Count.Is(1);
        result.FailureGroups[0].Failures[0].TestName.Is("RunTestsSandbox.Failures.Failing_test");
        result.FailureGroups[0].Failures[0].Message.Is("Assert.True() Failure");
        result.FailureGroups[0].Failures[0].Line.Is(42);
        result.Counts!.Failed.Is(1);
    }

    [Fact]
    public void Interpret_WhenFilterMatchesNoTests_ReturnsPassedWithSummary()
    {
        using var fixture = new InterpreterFixture();
        var trxPath = fixture.WriteTrx(
            "no-tests.trx",
            """
            <TestRun xmlns="http://microsoft.com/schemas/VisualStudio/TeamTest/2010">
              <ResultSummary outcome="Completed">
                <Counters total="0" executed="0" passed="0" failed="0" notExecuted="0" />
              </ResultSummary>
            </TestRun>
            """);

        var result = fixture.Interpret(CreateProcessResult(1, standardOutput: string.Empty, appliedFilter: "FullyQualifiedName=Missing.Tests"), [trxPath]);

        result.Outcome.Is(RunTestOutcomes.Passed);
        result.FailureGroups.Count.Is(0);
        result.BuildDiagnostics.IsNull();
        result.Summary.Is("No tests matched the filter.");
        result.Counts!.Total.Is(0);
    }

    [Fact]
    public void Interpret_WhenTrxContainsAsyncFailure_UsesRealTestIdentity()
    {
        using var fixture = new InterpreterFixture();
        var trxPath = fixture.WriteTrx(
            "async-failure.trx",
            """
            <TestRun xmlns="http://microsoft.com/schemas/VisualStudio/TeamTest/2010">
              <Results>
                <UnitTestResult testId="async-id" testName="RunTestsSandbox.AsyncFailures.Async_failure_test" outcome="Failed">
                  <Output>
                    <ErrorInfo>
                      <Message>boom</Message>
                      <StackTrace>at RunTestsSandbox.AsyncFailures.AsyncFailureTests.&lt;Async_failure_test&gt;d__1.MoveNext() in /tmp/AsyncTests.cs:line 27</StackTrace>
                    </ErrorInfo>
                  </Output>
                </UnitTestResult>
              </Results>
              <TestDefinitions>
                <UnitTest id="async-id" name="RunTestsSandbox.AsyncFailures.Async_failure_test">
                  <TestMethod className="RunTestsSandbox.AsyncFailures.AsyncFailureTests" name="Async_failure_test" />
                </UnitTest>
              </TestDefinitions>
              <ResultSummary outcome="Failed">
                <Counters total="1" executed="1" passed="0" failed="1" notExecuted="0" />
              </ResultSummary>
            </TestRun>
            """);

        var result = fixture.Interpret(CreateProcessResult(1), [trxPath]);

        result.FailureGroups[0].Failures[0].TestName.Is("RunTestsSandbox.AsyncFailures.Async_failure_test");
        result.FailureGroups[0].Failures[0].TestName!.Contains("MoveNext", StringComparison.Ordinal).IsFalse();
    }

    [Fact]
    public void Interpret_WhenFailuresSpanFiles_GroupsAndSortsThem()
    {
        using var fixture = new InterpreterFixture();
        var trxPath = fixture.WriteTrx(
            "grouped-failures.trx",
            """
            <TestRun xmlns="http://microsoft.com/schemas/VisualStudio/TeamTest/2010">
              <Results>
                <UnitTestResult testId="alpha-b" testName="Tests.Alpha.B_test" outcome="Failed">
                  <Output>
                    <ErrorInfo>
                      <Message>alpha b</Message>
                      <StackTrace>at Tests.Alpha.B_test() in /tmp/Alpha.cs:line 20</StackTrace>
                    </ErrorInfo>
                  </Output>
                </UnitTestResult>
                <UnitTestResult testId="missing" testName="Tests.Missing.No_file_test" outcome="Failed">
                  <Output>
                    <ErrorInfo>
                      <Message>missing file</Message>
                    </ErrorInfo>
                  </Output>
                </UnitTestResult>
                <UnitTestResult testId="alpha-a" testName="Tests.Alpha.A_test" outcome="Failed">
                  <Output>
                    <ErrorInfo>
                      <Message>alpha a</Message>
                      <StackTrace>at Tests.Alpha.A_test() in /tmp/Alpha.cs:line 10</StackTrace>
                    </ErrorInfo>
                  </Output>
                </UnitTestResult>
                <UnitTestResult testId="beta" testName="Tests.Beta.Only_test" outcome="Failed">
                  <Output>
                    <ErrorInfo>
                      <Message>beta</Message>
                      <StackTrace>at Tests.Beta.Only_test() in /tmp/Beta.cs:line 5</StackTrace>
                    </ErrorInfo>
                  </Output>
                </UnitTestResult>
              </Results>
              <TestDefinitions>
                <UnitTest id="alpha-b" name="Tests.Alpha.B_test"><TestMethod className="Tests.Alpha" name="B_test" /></UnitTest>
                <UnitTest id="missing" name="Tests.Missing.No_file_test"><TestMethod className="Tests.Missing" name="No_file_test" /></UnitTest>
                <UnitTest id="alpha-a" name="Tests.Alpha.A_test"><TestMethod className="Tests.Alpha" name="A_test" /></UnitTest>
                <UnitTest id="beta" name="Tests.Beta.Only_test"><TestMethod className="Tests.Beta" name="Only_test" /></UnitTest>
              </TestDefinitions>
              <ResultSummary outcome="Failed">
                <Counters total="4" executed="4" passed="0" failed="4" notExecuted="0" />
              </ResultSummary>
            </TestRun>
            """);

        var result = fixture.Interpret(CreateProcessResult(1), [trxPath]);

        result.Outcome.Is(RunTestOutcomes.TestFailures);
        result.FailureGroups.Select(static group => group.File).ToArray()
            .Is(["/tmp/Alpha.cs", null, "/tmp/Beta.cs"]);
        result.FailureGroups.Select(static group => group.Count).ToArray()
            .Is([2, 1, 1]);
        result.FailureGroups[0].Failures.Select(static failure => failure.TestName).ToArray()
            .Is(["Tests.Alpha.A_test", "Tests.Alpha.B_test"]);
        result.FailureGroups[0].Failures.Select(static failure => failure.Line).ToArray()
            .Is([10, 20]);
        result.FailureGroups[1].Failures[0].Message.Is("missing file");
        result.FailureGroups[1].Failures[0].Line.IsNull();
    }

    [Fact]
    public void Interpret_WhenMultipleTrxFilesExist_AggregatesCountsAndFailureGroups()
    {
        using var fixture = new InterpreterFixture();
        var firstTrxPath = fixture.WriteTrx(
            "first.trx",
            """
            <TestRun xmlns="http://microsoft.com/schemas/VisualStudio/TeamTest/2010">
              <Results>
                <UnitTestResult testId="first-pass" testName="RunTestsSandbox.Aggregation.First_pass" outcome="Passed" />
              </Results>
              <TestDefinitions>
                <UnitTest id="first-pass" name="RunTestsSandbox.Aggregation.First_pass">
                  <TestMethod className="RunTestsSandbox.Aggregation.FirstTests" name="First_pass" />
                </UnitTest>
              </TestDefinitions>
              <ResultSummary outcome="Passed">
                <Counters total="1" executed="1" passed="1" failed="0" notExecuted="0" />
              </ResultSummary>
            </TestRun>
            """);
        var secondTrxPath = fixture.WriteTrx(
            "second.trx",
            """
            <TestRun xmlns="http://microsoft.com/schemas/VisualStudio/TeamTest/2010">
              <Results>
                <UnitTestResult testId="second-fail" testName="RunTestsSandbox.Aggregation.Second_fail" outcome="Failed">
                  <Output>
                    <ErrorInfo>
                      <Message>second failed</Message>
                    </ErrorInfo>
                  </Output>
                </UnitTestResult>
              </Results>
              <TestDefinitions>
                <UnitTest id="second-fail" name="RunTestsSandbox.Aggregation.Second_fail">
                  <TestMethod className="RunTestsSandbox.Aggregation.SecondTests" name="Second_fail" />
                </UnitTest>
              </TestDefinitions>
              <ResultSummary outcome="Failed">
                <Counters total="1" executed="1" passed="0" failed="1" notExecuted="0" />
              </ResultSummary>
            </TestRun>
            """);

        var result = fixture.Interpret(CreateProcessResult(1), [firstTrxPath, secondTrxPath]);

        result.Outcome.Is(RunTestOutcomes.TestFailures);
        result.FailureGroups.Count.Is(1);
        result.FailureGroups[0].File.IsNull();
        result.FailureGroups[0].Count.Is(1);
        result.FailureGroups[0].Failures[0].TestName.Is("RunTestsSandbox.Aggregation.Second_fail");
        result.Counts!.Total.Is(2);
        result.Counts.Executed.Is(2);
        result.Counts.Passed.Is(1);
        result.Counts.Failed.Is(1);
    }

    [Fact]
    public void Interpret_WhenOutputShowsInvalidFilter_ReturnsInfrastructureError()
    {
        using var fixture = new InterpreterFixture();

        var result = fixture.Interpret(
            CreateProcessResult(
                1,
                standardError: "Invalid format for TestCaseFilter Error: Missing closing parenthesis ')'",
                appliedFilter: "FullyQualifiedName=("),
            []);

        result.Outcome.Is(RunTestOutcomes.InfrastructureError);
        result.FailureGroups.Count.Is(0);
        result.BuildDiagnostics.IsNull();
        result.Summary!.ShouldNotBeEmpty();
        result.Summary!.Contains("filter", StringComparison.OrdinalIgnoreCase).IsTrue();
    }

    private static object CreateProcessResult(int exitCode, string? standardOutput = null, string? standardError = null, string? appliedFilter = null)
    {
        var processResultType = typeof(InfrastructureExtensions).Assembly.GetType("RoslynMcp.Infrastructure.Testing.TestProcessResult", throwOnError: true)!;
        return Activator.CreateInstance(
            processResultType,
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
            binder: null,
            args: [exitCode, standardOutput ?? string.Empty, standardError ?? string.Empty, appliedFilter],
            culture: null)!;
    }

    private sealed class InterpreterFixture : IDisposable
    {
        private readonly object _interpreter;
        private readonly MethodInfo _interpretMethod;

        public InterpreterFixture()
        {
            var assembly = typeof(InfrastructureExtensions).Assembly;
            var interpreterType = assembly.GetType("RoslynMcp.Infrastructure.Testing.TestResultInterpreter", throwOnError: true)!;
            _interpreter = Activator.CreateInstance(interpreterType, nonPublic: true)!;
            _interpretMethod = interpreterType.GetMethod("Interpret")!;
            DirectoryPath = Path.Combine(Path.GetTempPath(), "RoslynMcpTests", Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(DirectoryPath);
        }

        public string DirectoryPath { get; }

        public RunTestsResult Interpret(object processResult, IReadOnlyList<string> trxPaths)
            => (RunTestsResult)_interpretMethod.Invoke(_interpreter, [processResult, trxPaths])!;

        public string WriteTrx(string fileName, string content)
        {
            var path = Path.Combine(DirectoryPath, fileName);
            File.WriteAllText(path, content);
            return path;
        }

        public void Dispose()
        {
            if (Directory.Exists(DirectoryPath))
            {
                Directory.Delete(DirectoryPath, recursive: true);
            }
        }
    }
}
