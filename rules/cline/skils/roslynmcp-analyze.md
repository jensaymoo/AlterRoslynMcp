---
name: roslynmcp-analyze
description: Quick orientation in .NET solution structure and architecture
---

## Purpose

This skill provides a structured approach for quickly understanding the structure and architecture of a .NET solution without diving into code details. It's designed for initial familiarization with new or unfamiliar projects. It can also be executed in plan mode, enabling step-by-step exploration of the solution structure and high-level components before proceeding to deeper analysis or implementation

## Scope

- Starting work with new/unfamiliar project
- Initial assessment of project's technical state

---

## CRITICAL INSTRUCTIONS

### Mandatory Requirements

1. **MUST** Always start with `load_solution`
2. **MUST** Specify **full absolute path** to the solution file (from filesystem root to `.sln`/`.slnx`)
3. **SHOULD** Check `readiness state` after loading solution

### Prohibited Actions

- **NEVER** Call `understand_codebase` before loading solution
- **NEVER** Use relative paths for solution file

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

### 2. understand_codebase

Quick orientation in solution structure with complexity hotspots identification.

| Parameter | Description |
|-----------|-------------|
| `profile` | `quick` / `standard` (default) / `deep` |

**Profile Selection:**
- `quick` — fast overview, minimal details
- `standard` — balanced view of structure and hotspots
- `deep` — comprehensive analysis with detailed metrics

---

## Workflow Steps

> **IMPORTANT:** Follow steps **sequentially**. Skipping steps or changing order may lead to errors.

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

### Step 2: Analyze Codebase Structure

```
Tool: understand_codebase
Parameter: profile = "standard"
```

**What You Get:**
- Project structure overview
- Dependency relationships between projects
- Complexity hotspots identification
- Key entry points and architectural patterns

---

## Expected Outcomes

After completing this workflow, you will have:

1. **Solution Structure** — understanding of how projects are organized
2. **Dependency Graph** — knowledge of project interdependencies
3. **Complexity Hotspots** — identification of potentially complex areas requiring attention
4. **Technical State Assessment** — initial evaluation of solution health
