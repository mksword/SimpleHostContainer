if (Test-Path -Path '..\.vs') {
    Remove-Item -Path '..\.vs' -Recurse -Force
}