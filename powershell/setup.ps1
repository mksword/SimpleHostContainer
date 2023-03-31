param(
    [string]
    $ProjectDir,
    [string]
    $Target,
    [string[]]
    $Platforms
)

function CombinPath {
    param(
        [string[]]
        $Paths
    )

    $temp = [System.IO.Path]::Combine($Paths)
    $dir = [System.IO.Path]::GetFullPath($temp)

    return $dir
}

function Handle{
    param(
        [string]
        $ProjectDir,
        [string]
        $Target,
        [string]
        $Platform,
        [string]
        $TargetProjectDir,
        [string]
        $FileName,
        [string]
        $PluginName,
        [string]
        $PluginsDir
    )

    $target_project_path = CombinPath -Paths ($ProjectDir, $TargetProjectDir)
    $target_project_bin_path = CombinPath -Paths ($target_project_path, 'bin', $Target, $Platform)
    $target_file_path = CombinPath -Paths ($target_project_bin_path, $FileName)
    $creationTime = ([System.IO.FileInfo]$target_file_path).LastWriteTime
    $version = [System.Diagnostics.FileVersionInfo]::GetVersionInfo($target_file_path).FileVersion
    
    $plugin_path = CombinPath -Paths ($PluginsDir, $PluginName)

    if (-not (Test-Path -Path $plugin_path)){
        $null = New-Item -Path $plugin_path -ItemType Directory
    }

    $plugin_viersion_path = CombinPath -Paths ($plugin_path, $version)

    if (-not (Test-Path -Path $plugin_viersion_path)) {
        $null = New-Item -Path $plugin_viersion_path -ItemType Directory
    }

    $file_path = CombinPath -Paths ($plugin_viersion_path, $FileName)

    if (Test-Path -Path $file_path) {
        $last_write_time = ([System.IO.FileInfo]$file_path).LastWriteTime

        if ($creationTime -gt $last_write_time) {
            Remove-Item -Path $file_path -Force
            Copy-Item $target_file_path $file_path
            "$target_file_path -> $file_path"
        } else {
            "$file_path existed!"
        }
    } else {
        Copy-Item $target_file_path $file_path
        "$target_file_path -> $file_path"
    }
}

function Platform_Handle {
    param(
        [string]
        $ProjectDir,
        [string]
        $Target,
        [string]
        $Platform
    )

    $bin_path = CombinPath -Paths ($ProjectDir, 'bin', $Target)
    $plugins_path = CombinPath -Path ($bin_path, 'Plugins')
    $parent_path = [System.IO.Path]::GetDirectoryName($ProjectDir)
    $list = $ProjectDir -split '\\'
    $index = @($list).Count - 1
    $project_name = $list[$index]

    if (-not (Test-Path -Path $plugins_path)) {
        $null = New-Item -Path $plugins_path -ItemType Directory
    }

    switch ($project_name){
        "SimpleHostContainer.Test.net472.Plugin" {
            Handle -ProjectDir $ProjectDir -Target $Target -Platform $Platform -TargetProjectDir '..\..\lib\TestData.Part1' -FileName 'TestData.Part1.dll' -PluginName 'TestData.Part1' -PluginsDir $plugins_path
            Handle -ProjectDir $ProjectDir -Target $Target -Platform $Platform -TargetProjectDir '..\..\lib\TestData.Part2' -FileName 'TestData.Part2.dll' -PluginName 'TestData.Part2' -PluginsDir $plugins_path
        }
    }
}

foreach($Platform in $Platforms) {
    Platform_Handle -ProjectDir $ProjectDir -Target $Target -Platform $Platform
}