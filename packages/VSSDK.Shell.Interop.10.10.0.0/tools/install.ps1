param($installPath, $toolsPath, $package, $project)

foreach ($reference in $project.Object.References)
{
	switch -regex ($reference.Name.ToLowerInvariant())
	{
	"^microsoft\.visualstudio\.shell\.interop(?:|\.8\.0|\.9\.0|\.10\.0)$"
		{
			$reference.CopyLocal = $false;
			$reference.EmbedInteropTypes = $false;
		}
	default
		{
			# ignore
		}
	}
}
