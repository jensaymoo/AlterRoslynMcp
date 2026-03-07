# Unified Sandbox Test Fixture Design

## Purpose

Define one test fixture design that supports both parallel read-only feature tests and isolated mutating feature tests.

`rename_symbol` is the first mutating example, but the design must work for future state-changing features without forcing a second, unrelated test architecture.

## Context

Current feature tests use a shared fixture that loads `tests/TestSolution/TestSolution.sln` once and keeps one shared workspace/session alive for the test collection.

That model is fast and works well for read-only tools, because they do not change state.

Mutating tools change that assumption:

- they can update the loaded Roslyn `Solution`
- they can advance workspace version
- future tools may write to the filesystem

So the problem is real, but the answer does not need to be two completely different systems. The better move is one sandbox-based fixture design with different lifetime modes.

## Design Goal

Create a single fixture concept with one common setup model:

- all feature tests run against a sandboxed copy of the test solution
- the fixture decides how long that sandbox lives
- read-only tests use a shared sandbox for speed
- mutating tests use an isolated sandbox for correctness

Same design. Different lifetime.

## Non-Goals

- rewriting the production architecture for tests
- forcing all tests to run serially
- keeping one globally shared mutable sandbox for every test
- building special infrastructure only for `rename_symbol`

## Core Idea

The canonical `tests/TestSolution` remains read-only test data.

Tests never operate directly on it.

Instead, the fixture creates sandbox copies from that baseline and loads the solution from the sandbox path. The important variable is not whether a sandbox exists, but whether it is shared or isolated.

## Fixture Model

### `SandboxedFeatureTestsFixture`

This is the single conceptual fixture model for feature tests.

Responsibilities:

- locate the canonical test solution
- create sandbox copies of that solution
- load the Roslyn solution from the sandbox path
- expose service resolution and common file path helpers
- clean up sandbox data when the fixture or test scope ends

The fixture supports two lifetime modes.

### 1. Shared Sandbox

Use for read-only tests.

Behavior:

- create one sandbox copy for the fixture or test collection
- load one workspace/session from that sandbox
- reuse it across read-only tests
- keep existing speed and parallel-read benefits

This works because the tests are not allowed to mutate the session or the files.

### 2. Isolated Sandbox

Use for mutating tests.

Behavior:

- create a fresh sandbox copy per test
- load a fresh workspace/session from that sandbox
- dispose and delete the sandbox after the test
- guarantee that mutations cannot bleed into another test

This is the mode that `rename_symbol` should use.

## Why This Is One Design, Not Two

Both test categories use the same ideas and mostly the same machinery:

- same canonical baseline
- same sandbox copy mechanism
- same service wiring
- same solution loading process
- same helper API shape

Only the sandbox lifetime changes.

That keeps the mental model small while still preserving test isolation where it matters.

## Proposed Building Blocks

### `SandboxedFeatureTestsFixture`

Owns shared setup behavior and path discovery.

Responsibilities:

- know the baseline `TestSolution` root
- create a sandbox root
- expose loaded solution metadata
- provide `GetRequiredService<T>()`
- provide file path helpers relative to the active sandbox

### `SandboxLifetime`

Simple configuration concept that decides reuse behavior.

Expected values:

- `Shared`
- `IsolatedPerTest`

This should stay tiny. It is not a framework. It is just the fixture lifetime switch.

### `SandboxedToolTests<TTool>`

Common base type for feature tests using the sandboxed fixture model.

Responsibilities:

- work against the active sandbox instead of the canonical solution
- expose common paths for the sandboxed solution
- keep test code unaware of copy/setup mechanics

### `MutatingToolTestContext`

Small disposable helper used only when a test needs `IsolatedPerTest` behavior.

Responsibilities:

- create one isolated sandbox for one test
- load one fresh session
- expose services and paths for that isolated run
- cleanup after the test finishes

This is intentionally narrower than a separate test framework. It is just the isolated runtime form of the same sandbox model.

## Test Categories

### Read-Only Feature Tests

Use the shared sandbox mode.

Rules:

- may share one loaded session inside the collection fixture
- must not call mutating tools
- must not depend on previous test execution order

### Mutating Feature Tests

Use isolated sandbox mode.

Rules:

- each test gets a fresh sandbox and fresh session
- no mutable state is shared between tests
- tests must assert both operation result and visible post-state

## Verification Expectations

Mutating tests should verify at least these layers.

### Result Contract

Examples:

- `Error` is absent
- changed document count is greater than zero
- changed file list is populated

### Post-Mutation State

Examples:

- the renamed symbol can be resolved by its new name
- affected files reflect the expected logical change
- the old state is no longer observable in the same way

### Isolation

Examples:

- another test still starts from the untouched baseline
- sandbox cleanup succeeds
- no changes leak into shared read-only fixture state

## Filesystem Support

The sandbox design should assume from the start that future features may write to disk.

That is why even read-only tests should operate on a sandbox copy rather than on the canonical `tests/TestSolution` directly.

This gives the test architecture one stable rule:

- baseline is immutable
- execution always happens in a sandbox

## `rename_symbol` as First Example

`rename_symbol` should prove the design works, not define a one-off pattern.

The first test should show:

- an isolated sandbox is created for the test
- the solution is loaded from that sandbox
- the symbol is resolved and renamed
- the updated session reflects the new name
- changed files and affected locations are asserted

## Acceptance Criteria

The fixture design is good enough when:

- there is one clear sandbox-based test model for feature tests
- read-only tests can still run fast and in parallel using a shared sandbox
- mutating tests run safely with isolated sandboxes per test
- the canonical `tests/TestSolution` is never mutated by tests
- `rename_symbol` becomes the first mutating test on this design
- future filesystem-mutating features fit the same model without redesign

## Practical Consequence

The important distinction is no longer "which fixture type do I use?"

It becomes:

- same fixture design
- shared sandbox lifetime for read-only tests
- isolated sandbox lifetime for mutating tests

That is the smallest design that still has teeth.
