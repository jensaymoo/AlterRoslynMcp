# Test Fixture Architecture Handoff

## Purpose

Define the target architecture for a unified sandbox-based test fixture that supports both read-only feature tests and mutating feature tests.

This document is the implementation handoff for the test infrastructure change. `rename_symbol` is the first mutating consumer and should be implemented on top of this design, not as a special case.

## Architectural Decision

All feature tests must execute against a sandbox copy of `tests/TestSolution`.

The test infrastructure uses one shared design with two lifetime modes:

- `Shared` for parallel read-only tests
- `IsolatedPerTest` for mutating tests

The canonical `tests/TestSolution` is immutable baseline input and must never be mutated by tests.

## Design Principles

1. One fixture model, not two unrelated frameworks.
2. Sandbox is mandatory for all feature tests.
3. Lifetime controls isolation depth.
4. Read-only speed is preserved through shared reuse.
5. Mutating correctness is preserved through per-test isolation.
6. Test code should not know sandbox copy mechanics unless it explicitly tests them.

## Target Types

### `FeatureTestFixtureBase`

Abstract base for common test infrastructure.

Responsibilities:

- discover repository root
- resolve canonical `tests/TestSolution` paths
- build the service provider
- load a solution from a provided sandbox solution path
- expose `GetRequiredService<T>()`
- expose common path helpers relative to the active solution root

Constraints:

- must not hardcode shared fixture lifetime assumptions
- must not assume direct use of the canonical baseline path as execution target

### `SharedSandboxFeatureTestsFixture`

Concrete fixture for read-only feature tests.

Responsibilities:

- create one sandbox copy for the test collection
- load one session from that sandbox
- expose loaded solution and sandbox-relative paths
- clean up sandbox data on fixture disposal

Constraints:

- intended only for tests that do not mutate workspace or filesystem state
- may be used with the existing xUnit collection model

### `IsolatedSandboxFeatureTestContext`

Disposable per-test execution context for mutating tests.

Responsibilities:

- create a fresh sandbox copy for a single test
- create a fresh service provider and session for that sandbox
- expose tool resolution and sandbox-relative paths
- clean up provider, session, and sandbox files at disposal

Constraints:

- no sharing across tests
- should be cheap enough for mutating scenarios, but correctness is more important than raw speed

### `TestSolutionSandbox`

Sandbox filesystem abstraction.

Responsibilities:

- create temp directory roots
- copy the canonical `tests/TestSolution` tree into the temp root
- expose sandbox root and sandbox solution path
- remove sandbox data during cleanup

Constraints:

- should be a narrow helper, not a general file utility library
- should keep copy/delete logic out of fixtures and test classes

### `SandboxedToolTests<TTool>`

Base test class for shared read-only tests.

Responsibilities:

- resolve `TTool` from `SharedSandboxFeatureTestsFixture`
- expose sandbox-relative helper paths
- preserve the convenience of the current `ToolTests<TTool>` base class

Constraints:

- must not be used for mutating tool tests

### `IsolatedToolTests<TTool>`

Base test class for mutating feature tests.

Responsibilities:

- create or obtain an `IsolatedSandboxFeatureTestContext` per test
- resolve `TTool` from that context
- expose helper methods for isolated arrange/act/assert flows
- provide a clean default for future mutating tool tests

Constraints:

- must not reuse the shared collection fixture

## Expected Test Usage

### Read-Only Tools

Expected model:

- test class participates in shared collection
- class derives from `SandboxedToolTests<TTool>`
- tools execute against one shared sandboxed solution per collection

Examples:

- `resolve_symbol`
- `find_usages`
- `trace_call_flow`

### Mutating Tools

Expected model:

- test class derives from `IsolatedToolTests<TTool>`
- each test creates a fresh isolated context
- tools execute against their own sandboxed solution copy

Examples:

- `rename_symbol`
- future apply/cleanup/refactoring tools

## File Structure

Suggested structure:

- `tests/RoslynMcp.Features.Tests/Infrastructure/FeatureTestFixtureBase.cs`
- `tests/RoslynMcp.Features.Tests/Infrastructure/SharedSandboxFeatureTestsFixture.cs`
- `tests/RoslynMcp.Features.Tests/Infrastructure/IsolatedSandboxFeatureTestContext.cs`
- `tests/RoslynMcp.Features.Tests/Infrastructure/TestSolutionSandbox.cs`
- `tests/RoslynMcp.Features.Tests/Infrastructure/SandboxedToolTests.cs`
- `tests/RoslynMcp.Features.Tests/Infrastructure/IsolatedToolTests.cs`

Existing files may be renamed or split if that produces a cleaner end state.

## Migration Guidance

Implementation should proceed in this order:

1. Extract common logic from the current fixture into `FeatureTestFixtureBase`.
2. Introduce `TestSolutionSandbox` and move sandbox copy/delete behavior there.
3. Convert the current shared feature fixture into `SharedSandboxFeatureTestsFixture`.
4. Replace the current `ToolTests<TTool>` base with `SandboxedToolTests<TTool>` or equivalent.
5. Add `IsolatedSandboxFeatureTestContext` and `IsolatedToolTests<TTool>`.
6. Add the first mutating test for `rename_symbol` on top of the isolated model.

## Acceptance Criteria

Implementation is complete when all of the following are true:

- all feature tests run against sandbox copies, not the canonical baseline
- read-only tests still use a shared fixture and remain parallel-safe
- mutating tests use isolated per-test sandboxes
- canonical `tests/TestSolution` remains untouched after test execution
- the existing read-only tests continue to pass after migration
- `rename_symbol` has at least one meaningful test using the isolated infrastructure

## Guardrails

The implementation must avoid these mistakes:

- one globally shared mutable sandbox for both read-only and mutating tests
- hidden fallback to the canonical `tests/TestSolution`
- mutating tests inheriting shared-session behavior by accident
- sandbox copy/delete logic duplicated across multiple test classes
- overengineering beyond the responsibilities listed here

## Definition of Done

Done means:

- the unified sandbox fixture architecture is implemented
- read-only and mutating tests use the correct lifetime mode
- at least one real mutating scenario proves the design
- no architectural redesign is needed to support future filesystem-mutating features

Architect phase complete -> handing over to code-monkey for implementation
