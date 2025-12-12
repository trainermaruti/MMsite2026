$colorReplacements = @{
    # Gradients - convert all to dark blue gradient
    'linear-gradient\(135deg,\s*#667eea\s+0%,\s*#764ba2\s+100%\)' = 'background: #001f3f'
    'linear-gradient\(135deg,\s*#[0-9a-fA-F]{6}\s+0%,\s*#[0-9a-fA-F]{6}\s+100%\)' = 'linear-gradient(135deg, #001f3f 0%, #003366 100%)'
    'linear-gradient\(180deg,\s*#[0-9a-fA-F]{6}\s+0%,\s*#[0-9a-fA-F]{6}\s+[0-9]+%,\s*#[0-9a-fA-F]{6}\s+100%\)' = 'linear-gradient(180deg, #000000 0%, #001f3f 50%, #000000 100%)'
    'linear-gradient\(90deg,\s*#[0-9a-fA-F]{6}\s+0%,\s*#[0-9a-fA-F]{6}\s+[0-9]+%,\s*#[0-9a-fA-F]{6}\s+100%\)' = 'linear-gradient(90deg, #001f3f 0%, #003366 50%, #001f3f 100%)'
    
    # Specific color hex codes
    '#6366f1' = '#001f3f'
    '#8b5cf6' = '#003366'
    '#60a5fa' = '#003366'
    '#3b82f6' = '#001f3f'
    '#10b981' = '#001f3f'
    '#34d399' = '#003366'
    '#ef4444' = '#001f3f'
    '#f59e0b' = '#001f3f'
    '#a78bfa' = '#003366'
    '#6ee7b7' = '#ffffff'
    '#FFD700' = '#ffffff'
    '#1e293b' = '#000000'
    '#0f172a' = '#000000'
    '#1e1b4b' = '#000000'
    '#0a0a0a' = '#000000'
    '#1a1a1a' = '#001f3f'
    '#2a2a2a' = '#001f3f'
    '#94a3b8' = '#ffffff'
    '#cbd5e1' = '#ffffff'
    '#64748b' = '#ffffff'
    '#f1f5f9' = '#ffffff'
    '#e5e5e5' = '#ffffff'
    
    # RGBA colors - convert to white or dark blue equivalents
    'rgba\(99,\s*102,\s*241,\s*[\d.]+\)' = 'rgba(0, 31, 63, $1)'
    'rgba\(96,\s*165,\s*250,\s*[\d.]+\)' = 'rgba(0, 51, 102, $1)'
    'rgba\(16,\s*185,\s*129,\s*[\d.]+\)' = 'rgba(0, 31, 63, $1)'
    'rgba\(148,\s*163,\s*184,\s*[\d.]+\)' = 'rgba(255, 255, 255, $1)'
    'rgba\(226,\s*232,\s*240,\s*[\d.]+\)' = 'rgba(255, 255, 255, $1)'
}

$files = Get-ChildItem -Path "Views" -Filter "*.cshtml" -Recurse
Write-Host "Found $($files.Count) view files to process..."

foreach ($file in $files) {
    $content = Get-Content $file.FullName -Raw
    $modified = $false
    
    foreach ($pattern in $colorReplacements.Keys) {
        if ($content -match $pattern) {
            $content = $content -replace $pattern, $colorReplacements[$pattern]
            $modified = $true
        }
    }
    
    if ($modified) {
        $content | Set-Content $file.FullName -NoNewline
        Write-Host "Updated: $($file.FullName)"
    }
}

Write-Host "`nColor scheme updated successfully!"
