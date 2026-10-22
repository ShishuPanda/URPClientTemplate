# ENTERPRISE UNITY URP MASTER RULES & ARCHITECTURAL STANDARDS

## RULE 1: STRICT ASSEMBLY DECOUPLING (ASMDEF PROTOCOL)
- Zero code in default global assembly (`Assembly-CSharp.dll`).
- Every module, skill, architecture core, and editor tool MUST reside in its own dedicated Assembly Definition (`.asmdef`).
- Dependencies between assemblies must be strictly acyclic, one-directional, and documented.
- All public APIs must expose clean C# interfaces (`I...`) to allow mocking and seamless unit testing.

## RULE 2: HIGH-PERFORMANCE C# STANDARDS (GC.ALLOC = 0 BUDGET)
- Target: C# 11+ / .NET Standard 2.1 specs.
- Zero heap allocations in runtime `Update()`, `LateUpdate()`, and `FixedUpdate()` loops (`0 bytes/frame`).
- Use `readonly struct`, `in`, `out`, and `ref readonly` parameters for large value types.
- Favor `NativeArray<T>`, `NativeSlice<T>`, and the C# Unity Job System (`IJob`, `IJobParallelFor`) for batch/data operations.
- Utilize object pooling (`UnityEngine.Pool.ObjectPool<T>` or custom unmanaged pools) for short-lived entities, events, and audio emitters.

## RULE 3: ASYNCHRONOUS PROGRAMMING PARADIGM
- Standardize on `Cysharp.Threading.Tasks.UniTask` or allocation-free async/await patterns.
- Coroutines (`StartCoroutine`, `IEnumerator`) and standard `System.Threading.Tasks.Task` are prohibited in hot runtime paths due to GC allocation and threading model impedance in Unity.
- Always pass `CancellationToken` (tied to GameObject lifecycle / `destroyCancellationToken`) to all asynchronous methods.

## RULE 4: AI TOKEN OPTIMIZATION & WORKFLOW
- When editing or updating C# files, provide concise, surgical code diffs or targeted replacements.
- Avoid re-emitting massive boilerplate files unless creating new modules.
- Preserve XML documentation and existing comments across all refactors.
