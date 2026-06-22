---
description: Generate comprehensive unit tests
---
Generate unit tests for the selected code:

**Test Structure:**
- Follow Arrange-Act-Assert pattern
- Use MSTest (`Microsoft.VisualStudio.TestTools.UnitTesting`)
- Descriptive test names that explain the scenario
- Use `[DataRow]` for parameterized tests where appropriate

**Coverage Goals:**
- Happy path scenarios
- Edge cases and boundary conditions
- Error conditions and exceptions
- Null/empty input handling
- Permission/authorization checks (`SecurityContext.CheckAccount`)

**FuseCP Patterns:**
- Use existing test infrastructure from `FuseCP.Tests`
- Follow naming conventions in existing tests under `FuseCP/Sources/FuseCP.Server.Tests/` and `FuseCP/Sources/FuseCP.EnterpriseServer.Tests/`
- Include both positive and negative tests
- Use the exception filter pattern: `catch (Exception ex) when (!(ex is OutOfMemoryException) && !(ex is StackOverflowException) && !(ex is AccessViolationException))`

Provide complete, runnable test code with all necessary imports.
