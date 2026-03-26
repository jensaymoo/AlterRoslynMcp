---
name: roslynmcp-tracing
description: Call tracing to understand how data and control flow through the system
---

## Purpose

This skill provides a structured approach for tracing calls to understand how data and control flow through the system. It enables deep understanding of execution paths and architectural connections between components. It can also be executed in plan mode, allowing step-by-step tracing of execution paths, systematic decomposition of call chains, and iterative refinement of understanding before taking further actions

## Scope

- Debugging and finding problem source
- Understanding architectural connections between components
- Analyzing call chain for critical methods

---

## CRITICAL INSTRUCTIONS

### Mandatory Requirements

1. **MUST** Always start with `load_solution`
2. **MUST** Specify **full absolute path** to the solution file (from filesystem root to `.sln`/`.slnx`)
3. **MUST** Obtain `symbolId` via `resolve_symbol` before using in other tools
4. **MUST** Use `projectPath` as stable selector for automation (not `projectId`)
5. **SHOULD** Check `readiness state` after loading solution

### Prohibited Actions

- **NEVER** Call analysis tools before loading solution
- **NEVER** Assume `symbolId` — always obtain via `resolve_symbol`
- **NEVER** Use `projectId` for automation (it's snapshot-local)

---

## Tools Used

### 1. load_solution

**MANDATORY FIRST STEP**

Loads .NET solution and prepares workspace for analysis.

| Parameter | Description |
|-----------|-------------|
| `solutionHintPath` | Full absolute path to `.sln`/`.slnx` file |

**Path Examples:**
- Linux/macOS: `/home/developer/projects/MyApplication/MyApplication.sln`
- Windows: `C:\Projects\MyApplication\MyApplication.sln`

**Readiness States to Watch:**
- `degraded_missing_artifacts` — need to run `dotnet restore`
- `degraded_restore_recommended` — need to run `dotnet restore`

---

### 2. resolve_symbol

Obtaining stable symbol identifier for use in other tools.

**Input data (one of the options):**
- `path` + `line` + `column` — position in source file
- `qualifiedName` — full or short name (e.g., `System.String`, `MyClass.MyMethod`)
- `symbolId` — existing ID for verification

| Parameter | Description |
|-----------|-------------|
| `projectPath` | Preferred stable selector |
| `projectName` | Project name for refinement |

---

### 3. trace_call_flow

Execution flow analysis: who calls the symbol and what it calls.

| Parameter | Description |
|-----------|-------------|
| `symbolId` | Symbol ID (required) |
| `path` + `line` + `column` | Alternative to symbolId |
| `direction` | `upstream` / `downstream` / `both` (default) |
| `depth` | Tracing depth (default 2) |
| `includePossibleTargets` | Include possible polymorphic targets |

**Direction Options:**
- `upstream` — find who calls this symbol
- `downstream` — find what this symbol calls
- `both` — complete bidirectional analysis

---

### 4. find_callers

Returns only immediate callers (depth 1).

| Parameter | Description |
|-----------|-------------|
| `symbolId` | Symbol ID (required) |

---

### 5. find_callees

Returns only immediately called symbols (depth 1).

| Parameter | Description |
|-----------|-------------|
| `symbolId` | Symbol ID (required) |

---

## Workflow Steps

> **IMPORTANT:** Follow steps **sequentially**. Skipping steps or changing order may lead to errors or incomplete data.

### Step 1: Load Solution

```
Tool: load_solution
Parameter: solutionHintPath = "<full_absolute_path_to_solution>"
```

**Actions:**
1. Provide full absolute path to `.sln` or `.slnx` file
2. Verify response for readiness state
3. If `degraded_missing_artifacts` or `degraded_restore_recommended` — run `dotnet restore` before proceeding

---

### Step 2: Resolve Symbol Entry Point

```
Tool: resolve_symbol
Parameters:
  - qualifiedName = "<fully_qualified_method_or_type_name>"
  OR
  - path = "<file_path>"
  - line = <line_number>
  - column = <column_number>
```

**Actions:**
1. Identify the method or type you want to trace
2. Use either qualified name or file position to resolve
3. Store the returned `symbolId` for subsequent steps

**Tips:**
- Use `qualifiedName` for well-known entry points (e.g., `MyNamespace.MyService.ProcessOrder`)
- Use `path` + `line` + `column` when exploring from specific code location

---

### Step 3: Build Call Graph

```
Tool: trace_call_flow
Parameters:
  - symbolId = "<symbol_id_from_step_2>"
  - direction = "both"
  - depth = 2 (or 3 for deeper analysis)
```

**Actions:**
1. Use `symbolId` obtained from Step 2
2. Set `direction: both` for complete picture
3. Adjust `depth` based on complexity needs (2-3 recommended)
4. Enable `includePossibleTargets: true` for polymorphic scenarios

**What You Get:**
- Visual representation of call hierarchy
- Upstream callers (who invokes this symbol)
- Downstream callees (what this symbol invokes)

---

### Step 4: Detailed Connection Analysis

```
Tool: find_callers
Parameter: symbolId = "<symbol_id>"

Tool: find_callees
Parameter: symbolId = "<symbol_id>"
```

**Actions:**
1. Use `find_callers` to get immediate callers with more detail
2. Use `find_callees` to understand direct dependencies
3. Repeat for specific nodes discovered in call graph

**Tips:**
- Focus on specific connections of interest from Step 3 results
- Use iteratively to explore branches of the call tree

---

## Expected Outcomes

After completing this workflow, you will have:

1. **Call Graph Visualization** — understanding of execution flow through the system
2. **Upstream Dependencies** — knowledge of all code paths leading to target symbol
3. **Downstream Dependencies** — understanding of all code invoked by target symbol
4. **Architectural Insights** — identification of coupling patterns and component relationships
