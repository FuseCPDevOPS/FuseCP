# Continue.dev Advanced Optimization Script for FuseCP
# Adds advanced features WITHOUT hardcoded model configurations
# Models can be configured via Continue.dev UI or environment variables

$configPath = "c:\git\FuseCPDevOPS-FuseCP\.continue\config.yaml"
$backupPath = "c:\git\FuseCPDevOPS-FuseCP\.continue\config.yaml.backup"

# Create backup
Copy-Item $configPath $backupPath -Force
Write-Host "✓ Backup created at: $backupPath" -ForegroundColor Green

# Read current config
$currentConfig = Get-Content $configPath -Raw

# Advanced features to append (NO hardcoded models)
$advancedConfig = @'

# ============================================================================
# ADVANCED OPTIMIZATIONS - Agent Mode, Context Providers, Custom Commands
# ============================================================================
# NOTE: Configure your models (together.ai or others) via Continue.dev UI
# or set environment variables. This config focuses on features, not providers.

# ---------------------------------------------------------------------------
# Tab Autocomplete Configuration (Fast inline completions)
# ---------------------------------------------------------------------------
# Configure your autocomplete model in Continue.dev settings UI
# Recommended: Use a fast, low-cost model for autocomplete

tabAutocompleteOptions:
  # Fast response with minimal latency
  useFileSuffix: true
  maxPromptTokens: 2048
  debounceDelay: 300  # ms - balances responsiveness vs API calls
  prefixPercentage: 0.85
  maxSnippetPercentage: 0.7
  disableInComments: false
  useCache: true
  slidingWindowLines: 50
  multilineCompletions: always  # Enable multi-line completions
  templatePriority: automatic

# ---------------------------------------------------------------------------
# Agent Mode Configuration (Optimized tool usage)
# ---------------------------------------------------------------------------
experimental:
  defaultContext: activeCodebase

agentMode:
  toolPolicy:
    maxToolCallsPerTurn: 15  # Prevent infinite loops
    autoRetryOnFailure: true
    maxRetries: 2
  diffPolicy:
    showDiffBeforeApply: true
    requireApproval: false  # Set to true if you want manual approval
  fileEditPolicy:
    maxFileEditsPerTurn: 10
    backupBeforeEdit: true
  batchOperations:
    enabled: true
    maxBatchSize: 5

# ---------------------------------------------------------------------------
# Enhanced Context Providers (Rich @-mentions)
# ---------------------------------------------------------------------------
contextProviders:
  # Reference specific code files or functions
  - name: code
    description: Reference specific code files or functions
    params:
      maxFiles: 10
      includeGitIgnored: false
      
  # Semantic search across entire codebase
  - name: codebase
    description: Semantic search across entire codebase
    params:
      nRetrieve: 10
      nFinal: 5
      threshold: 0.7
      
  # Include git diff in context
  - name: diff
    description: Include git diff in context
    params:
      includeUnstaged: true
      
  # Reference entire folders
  - name: folder
    description: Reference entire folders
    params:
      maxDepth: 3
      maxFiles: 50
      
  # Fetch documentation from URLs
  - name: web
    description: Fetch content from web URLs
    params:
      timeout: 10000
      
  # Quick access to key documentation
  - name: docs
    description: Reference project documentation
    params:
      maxChunks: 10

# ---------------------------------------------------------------------------
# Codebase Indexing Configuration (Optimized for FuseCP)
# ---------------------------------------------------------------------------
indexing:
  # Focus on main source directories
  includePatterns:
    - "FuseCP/Sources/**/*.cs"
    - "FuseCP/Sources/**/*.vb"
    - "FuseCP/Sources/**/*.ps1"
    - "FuseCP/Tools/**/*.ps1"
    - "FuseCP/Sources/**/*.less"
    - "FuseCP/Sources/**/*.aspx"
    - "FuseCP/Sources/**/*.ascx"
    
  # Exclude build outputs and generated files
  excludePatterns:
    - "**/bin/**"
    - "**/obj/**"
    - "**/node_modules/**"
    - "**/packages/**"
    - "**/TestResults/**"
    - "**/*.dll"
    - "**/*.exe"
    - "**/*.pdb"
    - "**/*.log"
    - "FuseCP/Database/install.*.sql"  # Large generated files
    
  # Indexing strategy
  strategy:
    chunkSize: 1000
    chunkOverlap: 200
    embeddingBatchSize: 32
    reindexOnChange: true

# ---------------------------------------------------------------------------
# Advanced Custom Commands (Workflow automation)
# ---------------------------------------------------------------------------
customCommands:

  - name: review
    description: Comprehensive code review with security/performance focus
    prompt: |
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
      - Memory leaks
      
      **Code Quality:**
      - SOLID principles adherence
      - Proper error handling
      - Null reference safety
      - Naming conventions
      - Code duplication
      
      **FuseCP Standards:**
      - Solution sync maintained
      - Build validation passes
      - Provider parity (if Exchange-related)
      - EF workflow compliance (if DB-related)
      
      Provide specific, actionable feedback with code examples.

  - name: refactor
    description: Safe refactoring with validation workflow
    prompt: |
      Refactor the selected code following these principles:
      
      1. **Preserve Behavior**: All existing functionality must work identically
      2. **Small Steps**: Make incremental changes, validate after each
      3. **Test Coverage**: Identify test gaps before refactoring
      4. **Validation Plan**: Specify which validation commands to run
      
      Refactoring goals (prioritize):
      - Reduce complexity
      - Improve readability
      - Eliminate duplication
      - Improve performance
      - Enhance testability
      
      After refactoring, provide:
      - Summary of changes
      - Validation commands to run
      - Risk assessment
      - Rollback plan if issues arise

  - name: explain
    description: Explain complex code patterns in detail
    prompt: |
      Explain the selected code in detail:
      
      1. **Purpose**: What problem does this code solve?
      2. **Flow**: Walk through the execution step-by-step
      3. **Dependencies**: What external systems/APIs does it use?
      4. **Edge Cases**: What scenarios could cause issues?
      5. **Performance**: What's the time/space complexity?
      6. **Alternatives**: Are there better approaches?
      
      Use diagrams or examples where helpful.
      Highlight any FuseCP-specific patterns or conventions.

  - name: test
    description: Generate comprehensive unit tests
    prompt: |
      Generate unit tests for the selected code:
      
      **Test Structure:**
      - Follow Arrange-Act-Assert pattern
      - One assertion per test (when possible)
      - Descriptive test names that explain the scenario
      
      **Coverage Goals:**
      - Happy path scenarios
      - Edge cases and boundary conditions
      - Error conditions and exceptions
      - Null/empty input handling
      - Permission/authorization checks
      
      **FuseCP Patterns:**
      - Use existing test infrastructure
      - Mock external dependencies appropriately
      - Follow naming conventions in existing tests
      - Include both positive and negative tests
      
      Provide complete, runnable test code with all necessary imports.

  - name: migrate
    description: Help with SolidCP to FuseCP migration patterns
    prompt: |
      Analyze this code for SolidCP to FuseCP migration:
      
      **Migration Checklist:**
      1. **Namespace Updates**: SolidCP.* to FuseCP.*
      2. **API Changes**: Identify deprecated/changed APIs
      3. **Configuration**: Web.config vs appsettings.json
      4. **Dependency Injection**: Update service registration
      5. **Async Patterns**: Convert synchronous to async where appropriate
      6. **Provider Model**: Ensure provider parity and patterns
      
      **Validation:**
      - Identify breaking changes
      - Suggest backward compatibility approaches
      - Provide migration script if applicable
      - List testing requirements
      
      Reference origin/SolidCPv1 branch for legacy behavior if needed.

  - name: debug
    description: Systematic debugging workflow
    prompt: |
      Help debug this issue systematically:
      
      **Step 1: Reproduce**
      - Exact steps to reproduce
      - Expected vs actual behavior
      - Environment details
      
      **Step 2: Isolate**
      - Minimal reproduction case
      - Recent changes that might have caused this
      - Related logs/error messages
      
      **Step 3: Analyze**
      - Code flow and potential failure points
      - Variable states at key points
      - Exception stack traces
      
      **Step 4: Fix**
      - Root cause identification
      - Proposed fix with explanation
      - Alternative solutions considered
      - Regression risk assessment
      
      **Step 5: Validate**
      - Test cases to verify the fix
      - Validation commands to run
      - Monitoring/logging to add

  - name: optimize
    description: Performance optimization analysis
    prompt: |
      Analyze this code for performance optimization:
      
      **Profile:**
      - Time complexity analysis
      - Space complexity analysis
      - Hot path identification
      - Bottleneck detection
      
      **Database (if applicable):**
      - Query optimization opportunities
      - Index recommendations
      - N+1 query detection
      - Batch operation opportunities
      
      **Memory:**
      - Allocation patterns
      - Large object heap usage
      - Disposal patterns
      - Cache opportunities
      
      **Async:**
      - Parallelization opportunities
      - Async/await best practices
      - Thread pool usage
      
      Provide specific optimizations with before/after code examples
      and expected performance improvements.

  - name: document
    description: Generate comprehensive documentation
    prompt: |
      Generate documentation for the selected code:
      
      **XML Documentation:**
      - Summary (what it does)
      - Parameters (with types and descriptions)
      - Return values
      - Exceptions that may be thrown
      - Usage examples
      
      **Additional Documentation:**
      - Architecture overview (if complex)
      - Integration points
      - Configuration requirements
      - Security considerations
      - Performance characteristics
      
      **FuseCP Standards:**
      - Follow existing documentation patterns
      - Include copyright header if applicable
      - Reference related documentation
      - Add to appropriate section of docs

# ---------------------------------------------------------------------------
# Usage Guide (Quick Reference)
# ---------------------------------------------------------------------------
# SLASH COMMANDS:
#   /validate        - Run fast local validation
#   /sln-sync        - Check solution synchronization
#   /db-check        - Verify database workflow
#   /cleanup-audit   - Audit feature removal completeness
#   /pr-prep         - Prepare PR description
#   /start-of-day    - Run start-of-day routine
#   /review          - Comprehensive code review
#   /refactor        - Safe refactoring workflow
#   /explain         - Explain complex code
#   /test            - Generate unit tests
#   /migrate         - SolidCP to FuseCP migration help
#   /debug           - Systematic debugging
#   /optimize        - Performance optimization
#   /document        - Generate documentation
#
# CONTEXT PROVIDERS (@-mentions):
#   @code filename   - Reference specific file
#   @codebase query  - Semantic search across codebase
#   @git             - Include git diff/history
#   @folder path     - Reference entire folder
#   @web url         - Fetch web content
#   @docs name       - Reference project docs
#
# TAB AUTOCOMPLETE:
#   - Starts automatically as you type
#   - Press Tab to accept suggestion
#   - Press Esc to dismiss
#   - Multi-line completions enabled
#   - Configure your model in Continue.dev settings
#
# AGENT MODE:
#   - Automatically uses tools when needed
#   - Shows diffs before applying changes
#   - Backs up files before editing
#   - Max 15 tool calls per turn
#
# TOKEN EFFICIENCY:
#   - On-demand rules load only when relevant
#   - Use specific @-mentions instead of full codebase
#   - Agent mode has retry limits to prevent loops
#
# MODEL CONFIGURATION:
#   - Configure models via Continue.dev UI settings
#   - Recommended for together.ai: Use fast models for autocomplete
#   - Recommended for chat/edit: Use capable models like Llama/Mistral
#
# BEST PRACTICES:
#   1. Run /start-of-day before starting work
#   2. Use /validate frequently during development
#   3. Use specific @-mentions for context
#   4. Run /pr-prep before creating pull requests
#   5. Use /review for important changes
#   6. Check /sln-sync when modifying project files
'@

# Append advanced features to config
$currentConfig + $advancedConfig | Set-Content $configPath -Encoding UTF8

Write-Host "✓ Advanced optimizations added to config.yaml" -ForegroundColor Green
Write-Host ""
Write-Host "Enhanced features:" -ForegroundColor Cyan
Write-Host "  • Tab Autocomplete (fast inline completions)"
Write-Host "  • Agent Mode (optimized tool usage, 15 calls/turn max)"
Write-Host "  • Enhanced Context Providers (@code, @codebase, @git, @folder, @web)"
Write-Host "  • Codebase Indexing (optimized for FuseCP structure)"
Write-Host "  • Advanced Commands (/review, /refactor, /explain, /test, /migrate, /debug, /optimize, /document)"
Write-Host ""
Write-Host "Next steps:" -ForegroundColor Yellow
Write-Host "  1. Review the updated config: .continue\config.yaml"
Write-Host "  2. Configure your together.ai models in Continue.dev UI settings"
Write-Host "  3. Restart Continue.dev extension"
Write-Host "  4. Try the new commands: /review, /optimize, /test"
Write-Host ""
Write-Host "Note: Models are NOT hardcoded - configure via Continue.dev UI" -ForegroundColor Magenta