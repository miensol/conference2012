cls
$connectionCount = 15
$warmupRequests = 50
$compareRequests = 100

$standardAddress = 'localhost:62562/Expensive/Execute'
$asyncAddress = 'localhost:62562/Expensive/ExecuteAsync'

$standarResults = 'execute.csv'
$asyncResults = 'executeAsync.csv' 
Set-Location (Split-Path -Parent $MyInvocation.MyCommand.Path)

Write-Host "Warmup Execute.."
.\ab.exe -n $warmupRequests -c $connectionCount -e $standarResults $standardAddress 
Write-Host "Done warming up execute"
Write-Host ("="*70)
Write-Host "Warmup ExecuteAync..."
.\ab.exe -n $warmupRequests -c $connectionCount -e $asyncResults  $asyncAddress 
Write-Host "Done warming up executeAsync"
Write-Host ("="*70)

Write-Host "Execute .."
.\ab.exe -n $compareRequests -c $connectionCount -e $standarResults $standardAddress 
Write-Host "Done execute"
Write-Host ("="*70)
Write-Host "Warmup ExecuteAync..."
.\ab.exe -n $compareRequests -c $connectionCount -e $asyncResults  $asyncAddress 
Write-Host "Done executeAsync"
Write-Host ("="*70)

Write-Host "Importing restuls"
$execute = (Import-Csv $standarResults -Header Percentage, Standard)
$executeAsync = (Import-Csv $asyncResults -Header Percentage, Async)

. .\LibraryChart

#join objects
function Join-Objects(){
	param(
		$first=@(), 
		$second = $(throw “Please specify a target to join”), 
		$where={$firstItem -eq $secondItem}, 
		$output={$firstItem}
	)

	if(-not $first) 
	{ 
	    foreach($element in $input) { $first += $element }
	}


	foreach($firstItem in $first)
	{
	   foreach($secondItem in $second)
	   {
	      if(& $where) { 
		  	& $output 
		  }
	   }
	}
}

$bothResutls = (Join-Objects $execute $executeAsync -where {
		($firstItem."Percentage served") -eq ($secondItem."Percentage served")
	} -output {@{
		Percentage = $firstItem."Percentage served";
		Execute = $firstItem."Time in ms";
		ExecuteAsync = $secondItem."Time in ms";
	}})
$asyncHash = ($bothResutls | ConvertTo-Hashtable Percentage ExecuteAsync)	
$bothResutls | Out-Chart -chartType 'line' -xField Percentage -yField Execute
$Chart.Series.Add("Second")
$asyncSeries = $Chart.Series["Second"]
$asyncSeries.ChartType = [System.Windows.Forms.DataVisualization.Charting.SeriesChartType]::Line
$asyncSeries.Points.DataBindXY($asyncHash.Keys, $asyncHash.Values)
$Chart.ResetAutoValues()
$Chart.Invalidate()
