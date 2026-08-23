[CmdletBinding()]
param([string]$RepositoryRoot = (Resolve-Path (Join-Path $PSScriptRoot '..\..')).Path)

$ErrorActionPreference = 'Stop'
$failures = [System.Collections.Generic.List[string]]::new()

function Assert-True {
    param([bool]$Condition, [string]$Message)
    if (-not $Condition) { $script:failures.Add($Message) }
}

function Test-CopilotAgent {
    param([string]$RelativePath)
    $path = Join-Path $RepositoryRoot $RelativePath
    Assert-True (Test-Path -LiteralPath $path) "$RelativePath must exist"
    if (-not (Test-Path -LiteralPath $path)) { return }

    $text = Get-Content -Raw -LiteralPath $path
    $frontmatterMatch = [regex]::Match($text, '\A---\r?\n(?<frontmatter>[\s\S]*?)\r?\n---\r?\n')
    Assert-True $frontmatterMatch.Success "$RelativePath must have bounded frontmatter"
    if ($frontmatterMatch.Success) {
        $frontmatter = $frontmatterMatch.Groups['frontmatter'].Value
        Assert-True ($frontmatter -match '(?m)^description:\s*.+$') "$RelativePath needs a frontmatter description"
        Assert-True ($frontmatter -match '(?m)^tools:\s*\["read",\s*"search"\]\s*$') "$RelativePath must allow only read and search tools"
        Assert-True (
            $frontmatter -match '(?m)^disable-model-invocation:\s*true\s*$'
        ) "$RelativePath must disable model invocation with Boolean true"
        Assert-True (
            $frontmatter -match '(?m)^user-invocable:\s*true\s*$'
        ) "$RelativePath must remain explicitly user-invocable with Boolean true"
    }
    Assert-True ($text -match 'PASS.*CONCERNS.*FAIL') "$RelativePath must use the gate decisions"
    Assert-True ($text -match 'BC-###') "$RelativePath must inspect BC rules"
    Assert-True ($text -match 'gate-contract\.md') "$RelativePath must load the gate contract"
    Assert-True ($text -match '(?i)governance file is missing.*FAIL') "$RelativePath must fail when governance is missing"
    Assert-True (
        $text -match '(?s)1\.\s*`Decision`.*?2\.\s*`Scope`.*?3\.\s*`Findings`.*?4\.\s*`Open decisions`.*?5\.\s*`Traceability`'
    ) "$RelativePath must require all gate headings in order"
}

$documentationPaths = @(
    '.bmad-harness/README.md',
    'BrewUpDocs/brewup-bmad-harness.md'
)

foreach ($relativePath in $documentationPaths) {
    $path = Join-Path $RepositoryRoot $relativePath
    Assert-True (Test-Path -LiteralPath $path) "$relativePath must exist"
    if (Test-Path -LiteralPath $path) {
        $text = Get-Content -Raw -LiteralPath $path
        Assert-True ($text -match 'validate-harness\.ps1') "$relativePath must document validation"
        Assert-True ($text -match 'without.*BMAD|BMAD.*not.*installed') "$relativePath must state the installation boundary"
        Assert-True (
            $text.Contains('.specify/memory/domain-carriers/brewup-sales-order-confirmation.md')
        ) "$relativePath must document the original BC governance carrier"
        Assert-True (
            $text.Contains('.specify/memory/architecture/brewup-module-structure.md')
        ) "$relativePath must document the original AR governance carrier"
    }
}

$manifestPath = Join-Path $RepositoryRoot '.bmad-harness\manifest.json'
Assert-True (Test-Path -LiteralPath $manifestPath) 'manifest.json must exist'

if (Test-Path -LiteralPath $manifestPath) {
    $manifest = Get-Content -Raw -LiteralPath $manifestPath | ConvertFrom-Json
    Assert-True ($manifest.schemaVersion -eq 1) 'schemaVersion must be 1'
    Assert-True ($manifest.governance.Count -eq 3) 'three governance documents are required'
    $expectedBcRuleIds = @(0..11 | ForEach-Object { 'BC-{0:D3}' -f $_ })
    $expectedArRuleIds = @(0..18 | ForEach-Object { 'AR-{0:D3}' -f $_ })
    Assert-True (
        (@($manifest.requiredRuleIds.bc) -join ',') -ceq ($expectedBcRuleIds -join ',')
    ) 'requiredRuleIds.bc must contain the complete ordered BC-000 through BC-011 set'
    Assert-True (
        (@($manifest.requiredRuleIds.ar) -join ',') -ceq ($expectedArRuleIds -join ',')
    ) 'requiredRuleIds.ar must contain the complete ordered AR-000 through AR-018 set'

    $expectedAgents = @(
        [pscustomobject]@{
            path = '.github/agents/bmad-brewup-load-domain-context.agent.md'
            gate = 'analysis.context'
        },
        [pscustomobject]@{
            path = '.github/agents/bmad-brewup-product-brief-guard.agent.md'
            gate = 'analysis.product-brief'
        },
        [pscustomobject]@{
            path = '.github/agents/bmad-brewup-prd-guard.agent.md'
            gate = 'planning.prd'
        },
        [pscustomobject]@{
            path = '.github/agents/bmad-brewup-architecture-readiness.agent.md'
            gate = 'solutioning.architecture-readiness'
        },
        [pscustomobject]@{
            path = '.github/agents/bmad-brewup-architecture-guard.agent.md'
            gate = 'solutioning.architecture'
        },
        [pscustomobject]@{
            path = '.github/agents/bmad-brewup-epics-stories-guard.agent.md'
            gate = 'solutioning.epics-stories'
        },
        [pscustomobject]@{
            path = '.github/agents/bmad-brewup-implementation-readiness.agent.md'
            gate = 'solutioning.implementation-readiness'
        },
        [pscustomobject]@{
            path = '.github/agents/bmad-brewup-story-guard.agent.md'
            gate = 'implementation.story'
        },
        [pscustomobject]@{
            path = '.github/agents/bmad-brewup-code-review-guard.agent.md'
            gate = 'implementation.code-review'
        }
    )

    foreach ($expectedAgent in $expectedAgents) {
        $registered = @($manifest.agents | Where-Object { $_.path -eq $expectedAgent.path })
        Assert-True ($registered.Count -eq 1) "$($expectedAgent.path) must be registered once"
        if ($registered.Count -eq 1) {
            Assert-True ($registered[0].gate -eq $expectedAgent.gate) "$($expectedAgent.path) must use gate $($expectedAgent.gate)"
            Assert-True ($registered[0].readOnly -eq $true) "$($expectedAgent.path) must be read-only"
        }
    }
}

$roleOverrides = @(
    '_bmad/custom/bmad-agent-analyst.toml',
    '_bmad/custom/bmad-agent-pm.toml',
    '_bmad/custom/bmad-agent-architect.toml',
    '_bmad/custom/bmad-agent-dev.toml'
)

foreach ($relativePath in $roleOverrides) {
    $path = Join-Path $RepositoryRoot $relativePath
    Assert-True (Test-Path -LiteralPath $path) "$relativePath must exist"
    if (Test-Path -LiteralPath $path) {
        $text = Get-Content -Raw -LiteralPath $path
        Assert-True ($text -match '(?m)^\[agent\]\r?$') "$relativePath must use [agent]"
        Assert-True ($text -match 'persistent_facts\s*=') "$relativePath must define persistent_facts"
        Assert-True ($text -match 'file:\{project-root\}/\.bmad-harness/governance/') "$relativePath must load governance"
    }
}

$workflowSkills = @(
    'bmad-product-brief',
    'bmad-prd',
    'bmad-architecture',
    'bmad-create-epics-and-stories',
    'bmad-check-implementation-readiness',
    'bmad-create-story',
    'bmad-dev-story',
    'bmad-code-review'
)

foreach ($skill in $workflowSkills) {
    $relativePath = "_bmad/custom/$skill.toml"
    $path = Join-Path $RepositoryRoot $relativePath
    Assert-True (Test-Path -LiteralPath $path) "$relativePath must exist"
    if (Test-Path -LiteralPath $path) {
        $text = Get-Content -Raw -LiteralPath $path
        Assert-True ($text -match '(?m)^\[workflow\]\r?$') "$relativePath must use [workflow]"
        Assert-True ($text -match 'activation_steps_prepend\s*=') "$relativePath must prepend activation"
        Assert-True ($text -match 'persistent_facts\s*=') "$relativePath must define persistent facts"
    }
}

$copilotAgents = @(
    '.github/agents/bmad-brewup-load-domain-context.agent.md',
    '.github/agents/bmad-brewup-product-brief-guard.agent.md',
    '.github/agents/bmad-brewup-prd-guard.agent.md',
    '.github/agents/bmad-brewup-architecture-readiness.agent.md',
    '.github/agents/bmad-brewup-architecture-guard.agent.md',
    '.github/agents/bmad-brewup-epics-stories-guard.agent.md',
    '.github/agents/bmad-brewup-implementation-readiness.agent.md',
    '.github/agents/bmad-brewup-story-guard.agent.md',
    '.github/agents/bmad-brewup-code-review-guard.agent.md'
)

foreach ($relativePath in $copilotAgents) {
    Test-CopilotAgent $relativePath
}

$contextLoaderPath = Join-Path $RepositoryRoot '.github/agents/bmad-brewup-load-domain-context.agent.md'
if (Test-Path -LiteralPath $contextLoaderPath) {
    $contextLoaderText = Get-Content -Raw -LiteralPath $contextLoaderPath
    Assert-True (
        $contextLoaderText -match '(?is)Sales.*Payment.*Warehouse.*open decisions'
    ) 'context loader must summarize all authorities and open decisions'
    Assert-True (
        $contextLoaderText -match '(?is)do not edit\s+BMAD\s+artifacts'
    ) 'context loader must explicitly avoid editing BMAD artifacts'
}

$planningGuardPaths = @(
    '.github/agents/bmad-brewup-product-brief-guard.agent.md',
    '.github/agents/bmad-brewup-prd-guard.agent.md',
    '.github/agents/bmad-brewup-architecture-readiness.agent.md'
)

foreach ($relativePath in $planningGuardPaths) {
    $path = Join-Path $RepositoryRoot $relativePath
    if (Test-Path -LiteralPath $path) {
        $text = Get-Content -Raw -LiteralPath $path
        Assert-True (
            $text -match '(?is)canonical selection precedence.*planning-artifacts.*exact canonical monolith.*sharded.*single eligible variant'
        ) "$relativePath must define deterministic canonical precedence"
        Assert-True (
            $text -match '(?is)case-insensitive regular\s+expression.*\\\.md\$'
        ) "$relativePath must define an anchored eligible-variant expression"
        Assert-True (
            $text -match '(?is)exclude.*review.*validation.*report.*checklist.*audit'
        ) "$relativePath must exclude supplemental review and report artifacts"
        Assert-True (
            $text.Contains('(?i)(^|[-_.])(?:review|validation|report|checklist|audit)s?([-_.]|$)')
        ) "$relativePath must define the exact supplemental-path token expression"
        Assert-True (
            $text -match '(?is)sharded.*index\.md.*ordinal'
        ) "$relativePath must support deterministic sharded artifacts"
        Assert-True (
            $text -match '(?is)same precedence rank.*FAIL'
        ) "$relativePath must fail unresolved artifact ambiguity"
        Assert-True (
            $text -match '(?is)Scope.*exact.*inspected.*missing.*eligible.*excluded'
        ) "$relativePath must report exact inspected, missing, eligible, and excluded paths"
    }
}

function Test-LaterAgentSelectionContract {
    param(
        [string]$RelativePath,
        [string[]]$CanonicalPaths,
        [string[]]$EligibleExpressions
    )

    $path = Join-Path $RepositoryRoot $RelativePath
    if (-not (Test-Path -LiteralPath $path)) { return }

    $text = Get-Content -Raw -LiteralPath $path
    foreach ($canonicalPath in $CanonicalPaths) {
        Assert-True (
            $text.Contains($canonicalPath)
        ) "$RelativePath must cite canonical artifact path $canonicalPath"
    }
    foreach ($eligibleExpression in $EligibleExpressions) {
        Assert-True (
            $text.Contains($eligibleExpression)
        ) "$RelativePath must define eligible expression $eligibleExpression"
    }
    Assert-True (
        $text.Contains('(?i)(^|[-_.])(?:review|validation|report|checklist|audit)s?([-_.]|$)')
    ) "$RelativePath must define the exact supplemental-path token expression"
    Assert-True (
        $text -match '(?is)(?:same precedence rank|multiple .*same.*rank|multiple exact matches).*FAIL'
    ) "$RelativePath must fail unresolved artifact ambiguity"
    Assert-True (
        $text -match '(?is)never (?:select|choose).*modification time'
    ) "$RelativePath must forbid modification-time selection"
    Assert-True (
        $text -match '(?is)Scope.*(?:selected|approved story path).*inspected.*missing.*excluded'
    ) "$RelativePath must report deterministic artifact-selection evidence under Scope"
}

$prdEligibleExpression = '^(?:[a-z0-9][a-z0-9._-]*-)?prd(?:-[a-z0-9][a-z0-9._-]*)?\.md$'
$architectureEligibleExpression = '^(?:[a-z0-9][a-z0-9._-]*-)?(?:architecture-spine|architecture)(?:-[a-z0-9][a-z0-9._-]*)?\.md$'
$epicsEligibleExpression = '^(?:[a-z0-9][a-z0-9._-]*-)?(?:epics|epics-and-stories)(?:-[a-z0-9][a-z0-9._-]*)?\.md$'
$storyEligibleExpression = '^(?:(?:[a-z0-9][a-z0-9._-]*-)?story(?:-[a-z0-9][a-z0-9._-]*)?|[0-9]+[-_.][0-9]+(?:[-_.][a-z0-9][a-z0-9._-]*)?)\.md$'
$planningCanonicalPaths = @(
    '_bmad-output/planning-artifacts/prd.md',
    '_bmad-output/planning-artifacts/prd/index.md',
    '_bmad-output/planning-artifacts/ARCHITECTURE-SPINE.md',
    '_bmad-output/planning-artifacts/architecture.md',
    '_bmad-output/planning-artifacts/architecture/index.md'
)
$aggregateCanonicalPaths = @($planningCanonicalPaths) + @(
    '_bmad-output/planning-artifacts/epics.md',
    '_bmad-output/planning-artifacts/epics-and-stories.md',
    '_bmad-output/planning-artifacts/epics/index.md'
)

Test-LaterAgentSelectionContract `
    '.github/agents/bmad-brewup-epics-stories-guard.agent.md' `
    $aggregateCanonicalPaths `
    @($prdEligibleExpression, $architectureEligibleExpression, $epicsEligibleExpression)
Test-LaterAgentSelectionContract `
    '.github/agents/bmad-brewup-implementation-readiness.agent.md' `
    $aggregateCanonicalPaths `
    @($prdEligibleExpression, $architectureEligibleExpression, $epicsEligibleExpression)
Test-LaterAgentSelectionContract `
    '.github/agents/bmad-brewup-story-guard.agent.md' `
    (@($planningCanonicalPaths) + @('_bmad-output/implementation-artifacts/')) `
    @($prdEligibleExpression, $architectureEligibleExpression, $storyEligibleExpression)
Test-LaterAgentSelectionContract `
    '.github/agents/bmad-brewup-code-review-guard.agent.md' `
    @('_bmad-output/implementation-artifacts/') `
    @()

$productBriefGuardPath = Join-Path $RepositoryRoot '.github/agents/bmad-brewup-product-brief-guard.agent.md'
if (Test-Path -LiteralPath $productBriefGuardPath) {
    $productBriefGuardText = Get-Content -Raw -LiteralPath $productBriefGuardPath
    Assert-True (
        $productBriefGuardText.Contains('^(?:[a-z0-9][a-z0-9._-]*-)?product-brief(?:-[a-z0-9][a-z0-9._-]*)?\.md$')
    ) 'product brief guard must define the exact eligible-variant expression'
}

$prdGuardPath = Join-Path $RepositoryRoot '.github/agents/bmad-brewup-prd-guard.agent.md'
if (Test-Path -LiteralPath $prdGuardPath) {
    $prdGuardText = Get-Content -Raw -LiteralPath $prdGuardPath
    Assert-True (
        $prdGuardText.Contains('^(?:[a-z0-9][a-z0-9._-]*-)?prd(?:-[a-z0-9][a-z0-9._-]*)?\.md$')
    ) 'PRD guard must define the exact eligible-variant expression'
    Assert-True (
        $prdGuardText -match '(?is)acceptance criteria.*requirement-to-rule traceability'
    ) 'PRD guard must check acceptance criteria and requirement-to-rule traceability'
}

$architectureReadinessPath = Join-Path $RepositoryRoot '.github/agents/bmad-brewup-architecture-readiness.agent.md'
if (Test-Path -LiteralPath $architectureReadinessPath) {
    $architectureReadinessText = Get-Content -Raw -LiteralPath $architectureReadinessPath
    Assert-True (
        $architectureReadinessText.Contains('^(?:[a-z0-9][a-z0-9._-]*-)?prd(?:-[a-z0-9][a-z0-9._-]*)?\.md$')
    ) 'architecture readiness must define the exact PRD eligible-variant expression'
    Assert-True (
        $architectureReadinessText -match '(?is)return\s+`FAIL`.*invent a blocking\s+business policy.*return\s+`CONCERNS`.*non-blocking ambiguity.*return\s+`PASS`.*ownership.*external references.*invariant.*open questions.*scope'
    ) 'architecture readiness must define all three readiness thresholds'
}

$architectureGuardPath = Join-Path $RepositoryRoot '.github/agents/bmad-brewup-architecture-guard.agent.md'
if (Test-Path -LiteralPath $architectureGuardPath) {
    $architectureGuardText = Get-Content -Raw -LiteralPath $architectureGuardPath
    Assert-True (
        $architectureGuardText -match 'AR-###'
    ) 'architecture guard must inspect AR rules'
}

$implementationReadinessPath = Join-Path $RepositoryRoot '.github/agents/bmad-brewup-implementation-readiness.agent.md'
if (Test-Path -LiteralPath $implementationReadinessPath) {
    $implementationReadinessText = Get-Content -Raw -LiteralPath $implementationReadinessPath
    Assert-True (
        $implementationReadinessText -match 'AR-###'
    ) 'implementation readiness must inspect AR rules'
    Assert-True (
        $implementationReadinessText -match 'Decision:\s*PASS\s*\|\s*CONCERNS\s*\|\s*FAIL'
    ) 'implementation readiness must define the aggregate decision line'
}

$codeReviewGuardPath = Join-Path $RepositoryRoot '.github/agents/bmad-brewup-code-review-guard.agent.md'
if (Test-Path -LiteralPath $codeReviewGuardPath) {
    $codeReviewGuardText = Get-Content -Raw -LiteralPath $codeReviewGuardPath
    Assert-True (
        $codeReviewGuardText -match '(?is)file paths?.*line numbers?.*when available'
    ) 'code review guard must require file-path evidence'
}

$validator = Join-Path $RepositoryRoot '.bmad-harness\scripts\validate-harness.ps1'
Assert-True (Test-Path -LiteralPath $validator) 'validator script must exist'

if (Test-Path -LiteralPath $validator) {
    $repositoryOutput = & $validator -RepositoryRoot $RepositoryRoot
    $repositoryOutput | Write-Output
    Assert-True ($LASTEXITCODE -eq 0) 'repository harness validation must pass'

    $passOutput = & $validator -RepositoryRoot $RepositoryRoot -ArtifactRoot (Join-Path $PSScriptRoot 'fixtures\pass')
    $passOutput | Write-Output
    Assert-True ($LASTEXITCODE -eq 0) 'pass fixture must pass'

    $realisticPassOutput = & $validator -RepositoryRoot $RepositoryRoot -ArtifactRoot (Join-Path $PSScriptRoot 'fixtures\pass-realistic')
    $realisticPassOutput | Write-Output
    Assert-True ($LASTEXITCODE -eq 0) 'realistic external-reference and separate-module fixture must pass'

    $authorityOutput = & $validator -RepositoryRoot $RepositoryRoot -ArtifactRoot (Join-Path $PSScriptRoot 'fixtures\fail-authority')
    $authorityExit = $LASTEXITCODE
    $authorityOutput | Write-Output
    $authorityText = @($authorityOutput) -join [Environment]::NewLine
    Assert-True ($authorityExit -eq 1) 'authority fixture must fail'
    Assert-True (
        $authorityText -match 'FAIL \[BC-003/BC-007\]'
    ) 'authority fixture must report BC-003/BC-007 evidence'

    $moduleOutput = & $validator -RepositoryRoot $RepositoryRoot -ArtifactRoot (Join-Path $PSScriptRoot 'fixtures\fail-module')
    $moduleExit = $LASTEXITCODE
    $moduleOutput | Write-Output
    $moduleText = @($moduleOutput) -join [Environment]::NewLine
    Assert-True ($moduleExit -eq 1) 'module fixture must fail'
    Assert-True (
        $moduleText -match 'FAIL \[AR-001/AR-016\]'
    ) 'module fixture must report AR-001/AR-016 evidence'

    $missingPaymentModuleOutput = & $validator -RepositoryRoot $RepositoryRoot -ArtifactRoot (Join-Path $PSScriptRoot 'fixtures\fail-missing-payment-module')
    $missingPaymentModuleExit = $LASTEXITCODE
    $missingPaymentModuleOutput | Write-Output
    $missingPaymentModuleText = @($missingPaymentModuleOutput) -join [Environment]::NewLine
    Assert-True ($missingPaymentModuleExit -eq 1) 'missing Payment module fixture must fail'
    Assert-True (
        $missingPaymentModuleText -match 'FAIL \[AR-001/AR-002/AR-016\]'
    ) 'missing Payment module fixture must report AR-001/AR-002/AR-016 evidence'

    $policyOutput = & $validator -RepositoryRoot $RepositoryRoot -ArtifactRoot (Join-Path $PSScriptRoot 'fixtures\fail-policy')
    $policyExit = $LASTEXITCODE
    $policyOutput | Write-Output
    $policyText = @($policyOutput) -join [Environment]::NewLine
    Assert-True ($policyExit -eq 1) 'policy fixture must fail'
    Assert-True (
        $policyText -match 'FAIL \[BC-000\]'
    ) 'policy fixture must report BC-000 evidence'

    function Invoke-StructuralFixture {
        param(
            [string]$FixturePath,
            [string]$ValidatorPath,
            [string]$SourceRepositoryRoot
        )

        $fixture = Get-Content -Raw -LiteralPath $FixturePath | ConvertFrom-Json
        $temporaryBase = [System.IO.Path]::GetFullPath([System.IO.Path]::GetTempPath())
        $temporaryRoot = Join-Path $temporaryBase "brewup-bmad-harness-tests-$([guid]::NewGuid().ToString('N'))"
        New-Item -ItemType Directory -Path $temporaryRoot | Out-Null

        try {
            $requiredPaths = @(
                '.bmad-harness/governance',
                '.bmad-harness/manifest.json',
                '.specify/memory/domain-carriers/brewup-sales-order-confirmation.md',
                '.specify/memory/architecture/brewup-module-structure.md',
                '_bmad/custom',
                '_bmad-output/project-context.md',
                '.github/agents'
            )
            foreach ($relativePath in $requiredPaths) {
                $sourcePath = Join-Path $SourceRepositoryRoot $relativePath
                $destinationPath = Join-Path $temporaryRoot $relativePath
                $destinationParent = Split-Path -Parent $destinationPath
                New-Item -ItemType Directory -Path $destinationParent -Force | Out-Null
                Copy-Item -LiteralPath $sourcePath -Destination $destinationPath -Recurse
            }

            $targetPath = Join-Path $temporaryRoot ([string]$fixture.target)
            $targetText = Get-Content -Raw -LiteralPath $targetPath
            $normalizedTargetText = $targetText.Replace("`r`n", "`n")
            if ($null -ne $fixture.psobject.Properties['prefix']) {
                $mutatedText = [string]$fixture.prefix + $normalizedTargetText + [string]$fixture.suffix
            } else {
                $searchText = [string]$fixture.search
                Assert-True (
                    $normalizedTargetText.Contains($searchText)
                ) "structural fixture '$($fixture.name)' search text must exist"
                $mutatedText = $normalizedTargetText.Replace($searchText, [string]$fixture.replace)
            }
            Set-Content -LiteralPath $targetPath -Value $mutatedText -NoNewline

            $output = & $ValidatorPath -RepositoryRoot $temporaryRoot
            $exitCode = $LASTEXITCODE
            return [pscustomobject]@{
                Name = [string]$fixture.name
                ExpectedCheckId = [string]$fixture.expectedCheckId
                ExpectedText = if ($null -ne $fixture.psobject.Properties['expectedText']) {
                    [string]$fixture.expectedText
                } else {
                    $null
                }
                ExitCode = $exitCode
                Output = @($output)
            }
        } finally {
            $resolvedTemporaryRoot = [System.IO.Path]::GetFullPath($temporaryRoot)
            $temporaryLeaf = Split-Path -Leaf $resolvedTemporaryRoot
            if (
                $resolvedTemporaryRoot.StartsWith($temporaryBase, [System.StringComparison]::OrdinalIgnoreCase) -and
                $temporaryLeaf.StartsWith('brewup-bmad-harness-tests-', [System.StringComparison]::Ordinal)
            ) {
                Remove-Item -LiteralPath $resolvedTemporaryRoot -Recurse -Force
            }
        }
    }

    $structuralFixturePaths = @(
        Get-ChildItem -LiteralPath (Join-Path $PSScriptRoot 'fixtures\structural') -File -Filter '*.json' |
            Sort-Object Name |
            Select-Object -ExpandProperty FullName
    )
    foreach ($fixturePath in $structuralFixturePaths) {
        $structuralResult = Invoke-StructuralFixture $fixturePath $validator $RepositoryRoot
        $structuralResult.Output | Write-Output
        $structuralText = @($structuralResult.Output) -join [Environment]::NewLine
        Assert-True (
            $structuralResult.ExitCode -eq 1
        ) "structural fixture '$($structuralResult.Name)' must fail"
        Assert-True (
            $structuralText -match "FAIL \[$([regex]::Escape($structuralResult.ExpectedCheckId))\]"
        ) "structural fixture '$($structuralResult.Name)' must report $($structuralResult.ExpectedCheckId)"
        if ($null -ne $structuralResult.ExpectedText -and -not [string]::IsNullOrWhiteSpace($structuralResult.ExpectedText)) {
            Assert-True (
                $structuralText.Contains($structuralResult.ExpectedText)
            ) "structural fixture '$($structuralResult.Name)' must identify $($structuralResult.ExpectedText)"
        }
    }
}

if ($failures.Count -gt 0) {
    $failures | ForEach-Object { [Console]::Error.WriteLine($_) }
    exit 1
}

Write-Output 'BMAD harness tests passed.'
