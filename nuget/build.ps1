param(
    [string]$version
)

# 定义变量
$projectName = "SmileControls"
$projectPath = "$PSScriptRoot\$projectName\$projectName.csproj"
$nupkgOutputDirectory = "$PSScriptRoot\NuGetPackages"

# 清理和构建项目
msbuild $projectPath /p:Configuration=Release /t:Clean,Build

# 创建NuGet包
nuget pack $projectPath -Prop Configuration=Release -Version $version -OutputDirectory $nupkgOutputDirectory -Symbols

# 将NuGet包推送到NuGet.org
nuget push "$nupkgOutputDirectory\$projectName.$version.nupkg" -Source nuget.org -ApiKey ${{secrets.NUGET_API_KEY}}
