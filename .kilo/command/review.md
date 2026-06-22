---
description: Comprehensive code review with security and performance focus
---
Review the current changes with focus on:

**Security:**
- SQL injection vulnerabilities
- XSS/CSRF protection
- Authentication/authorization issues
- Sensitive data exposure
- Input validation

**Performance:**
- N+1 query patterns
- Unnecessary allocations
- Inefficient algorithms
- Missing indexes (if DB-related)
- Memory leaks / missing Dispose

**Code Quality:**
- Null reference safety
- Error handling and exception filters
- Naming conventions matching existing patterns
- Code duplication

**FuseCP Standards:**
- Solution sync maintained
- Build validation passes
- Provider parity (if Exchange-related)
- EF workflow compliance (if DB-related)
- Copyright headers present and current

Provide specific, actionable feedback with code examples.
