Param (
	[switch] $merge,
	[switch] $build,
	[switch] $Sort,
	[switch] $rename,
	[switch] $twoways
)

function Normalize($source, $result)
{

[Xml]$xmlsrc = Get-Content $source
$settings = new-object System.Xml.XmlWriterSettings
$settings.Indent = $true;

try {
    New-Item -Name $result -ItemType File -force |Out-Null
}
catch [Exception]
{
}

$xw = [System.Xml.XmlWriter]::Create((Resolve-Path $result), $settings)
    $xmlsrc.ResourceList.Item | % {
    $node = $xmlsrc.SelectSingleNode("/ResourceList/Item[@Name='" + $_.Name + "']")
        if ($_.'#text')
        {   
            $nodetext = $_.'#text' -Replace '\s+', ' '
            if (($node.ChildNodes.Count -eq 3) -and ($node.'#cdata-section'))
            {
                $cdatatext = $node.'#cdata-section'
                $nodetext = $node.FirstChild.Data + $cdatatext + $node.LastChild.Data
                $node.IsEmpty = $true
                $node.AppendChild($xmlsrc.CreateCDataSection($nodetext.Trim())) | Out-Null
            }
            else
            {
                $node.AppendChild($xmlsrc.CreateCDataSection($nodetext.Trim())) | Out-Null
                $node.RemoveChild($_.FirstChild) | Out-Null
            }
        }
        elseif ($_.'#cdata-section')
        {
            $foo =$_.'#cdata-section' -Replace '\s+', ' '
            $node.AppendChild($xmlsrc.CreateCDataSection($foo.Trim())) | Out-Null
            $node.RemoveChild($_.FirstChild) | Out-Null
        }
        if ($_.Attributes[0].Name -ne 'Name')
        {
            $n = $_.Attributes[1]
            $v = $_.Attributes[0]
            $_.RemoveAllAttributes()
            $_.SetAttribute($n.Name, $n.Value.ToLower())
            $_.SetAttribute($v.Name, $v.Value)
        }
        elseif ($_.Attributes.Count -gt 1)
        {
            $n = $_.Attributes[0]
            $v = $_.Attributes[1]
            $_.RemoveAllAttributes()
            $_.SetAttribute($n.Name, $n.Value.ToLower())
            $_.SetAttribute($v.Name, $v.Value)
        }
    }
    
    $xmlsrc.Save($xw)
}

function Merge-File($source, $target, $result)
{
[Xml]$xmlsrc = Get-Content $source
[Xml]$xmltgt = Get-Content $target

Write-Host "Source items count : $($xmlsrc.ResourceList.Item.Count)"
Write-Host "Target items count : $($xmltgt.ResourceList.Item.Count)"


$dic = New-Object 'system.collections.generic.dictionary[string,system.xml.xmllinkednode]'

Write-Host "Create dictionary on target and find duplicate entries"
$xmltgt.ResourceList.Item | % {
	if ($dic.ContainsKey($_.Name.ToLower())) {
		if ($dic[$_.Name.ToLower()] -ne $_.Value)
		{
			Write-Host $_.Name -foregroundcolor Red. 
		} else {
			Write-Host $_.Name -foregroundcolor Green
		}
		$dic.Set_Item( $_.Name, $_)
	}
	else {
		$dic.Add( $_.Name.ToLower(), $_)
	}
}

$dicsrc = New-Object 'system.collections.generic.dictionary[string,system.xml.xmllinkednode]'

Write-Host "Create dictionary on source and find duplicate entries"
$xmlsrc.ResourceList.Item | % {
	if ($dicsrc.ContainsKey($_.Name.ToLower())) {
		if ((diff $dicsrc[$_.Name.ToLower()] $_.Value -includeequal).SideIndicator -eq '==')
		{
			Write-Host $_.Name -foregroundcolor Red
		} else {
			Write-Host $_.Name -foregroundcolor Green
		}
		$dicsrc.Set_Item( $_.Name.ToLower(), $_)
	}
	else {
		$dicsrc.Add( $_.Name.ToLower(), $_)
	}
}


$previous = $null
$xmlsrc.ResourceList.Item | % {
	if (!$dic.ContainsKey($_.Name.ToLower())) {
		# Insert missing element
		$nodes = $xmltgt.SelectNodes("/ResourceList/Item[@Name='" + $previous.Name + "']")
		if ($nodes -and $nodes.count -gt 0) {
			$index = $nodes.count-1
			$toto = $nodes.Item($index)
			$newnode = $xmltgt.ImportNode($_, $true)
			$newnode.SetAttribute("Version", "-4")
			$node = $toto.ParentNode.InsertAfter($newnode, $toto)
			
		} else {
			Write-Host "Previous key not found, current key : $($_.Name) , previous key : $($previous.Name)" -foregroundcolor Red
		}
	}
    elseif ($dic[$_.Name.ToLower()].'#cdata-section' -ne $_.'#cdata-section' -and !$dic[$_.Name.ToLower()].'#cdata-section'.Contains(".ch"))
    {
        $nodes = $xmltgt.SelectNodes("/ResourceList/Item[@Name='" + $previous.Name + "']")
		if ($nodes -and $nodes.count -gt 0) {
			$index = $nodes.count-1
			$toto = $nodes.Item($index)
			$newnode = $xmltgt.ImportNode($_, $true)			
            $nodeToRemove = $xmltgt.SelectSingleNode("/ResourceList/Item[@Name='" + $_.Name + "']")
            $nodeToRemove.ParentNode.RemoveChild($nodeToRemove) | Out-Null
			$node = $toto.ParentNode.InsertAfter($newnode, $toto)
        }
    }
	$previous = $_
}

try {
    New-Item -Name $result -ItemType File -force |Out-Null
}
catch [Exception]
{
}

$xmltgt.Save((Resolve-Path $result))
$exe = 'C:\Program Files (x86)\Beyond Compare 3\BComp.exe' 
&$exe $target $result

}

function Sort-File($source, $target, $result)
{
[Xml]$xmlsrc = Get-Content $source
[Xml]$xmltgt = Get-Content $target
[Xml]$xmlres = Get-Content $target

$xmlres.ResourceList.RemoveAll()

$xmlsrc.ResourceList.Item  | % {
	$node = $xmltgt.SelectSingleNode("/ResourceList/Item[@Name='" + $_.Name + "']")
	if ($node) {
		$newnode = $xmlres.ImportNode($node, $true)
		$newnode2 = $xmlres.SelectSingleNode("/ResourceList").AppendChild($newnode)
	} else {
		Write-Host "Node not found $($_.Name)" -foregroundcolor Red
	}
}
$xmlres.Save($result)
}

function Clean-File($source, $target)
{
[Xml]$xmlsrc = Get-Content $source
[Xml]$xmltgt = Get-Content $target


$xmlsrc.ResourceList.Item  | % {
	$_.InnerText = ""
	$_.RemoveAttribute("Version")
	$_.RemoveAttribute("version")
}
$xmlsrc.Save($source + ".empty")

$xmltgt.ResourceList.Item  | % {
	$_.InnerText = ""
	$_.RemoveAttribute("Version")
	$_.RemoveAttribute("version")
}
$xmltgt.Save($target + ".empty")
}

function Build($a, $b, $c)
{
	[Xml]$xmlsrc = Get-Content $a
	[Xml]$xmltgt = Get-Content $b
	[Xml]$xmltgt2 = Get-Content $c
$Excel = New-Object -Com Excel.Application
$Excel.visible = $true
$g = $Excel.Workbooks.Add()
$Sheet = $Excel.Worksheets.Item(1)
$Sheet.Name = "Translate"
$Sheet.Cells.Item(1,1) = "Key"
$Sheet.Cells.Item(1,2) = "fr-FR version"
$Sheet.Cells.Item(1,3) = "fr-FR value"
$Sheet.Cells.Item(1,4) = "fr-BE version"
$Sheet.Cells.Item(1,5) = "fr-BE value"
$Sheet.Cells.Item(1,6) = "nl-BE version"
$Sheet.Cells.Item(1,7) = "nl-BE value"
$Excel.Application.ScreenUpdating = $true
$Excel.Application.DisplayStatusBar = $true
$Excel.Application.Calculation = -4135
$Excel.Application.EnableEvents = $False  
$Excel.ActiveSheet.DisplayPageBreaks = $False  
$Excel.ActiveSheet.AutoFilterMode = $False
$ocell = $Sheet.Range("A2")
$i=0
$total=$xmlsrc.ResourceList.Item.Count
	$xmlsrc.ResourceList.Item  | % {
		$i++
		
		$node = $xmltgt.SelectSingleNode("/ResourceList/Item[@Name='" + $_.Name + "']")
		$node2 = $xmltgt2.SelectSingleNode("/ResourceList/Item[@Name='" + $_.Name + "']")
		if ($_.Version -eq "-4" -or $node.Version -eq "-4" -or $node2.Version -eq "-4" -or $node.InnerText -eq $node2.InnerText) {
			Write-Host "$i / $total - $($_.Name)"
			$ocell.Value2 = $_.Name
			$ocell.Offset(0,1).Value2 = $_.Version
			$ocell.Offset(0,2).Value2 = $_.InnerText
			$ocell.Offset(0,3).Value2 = $node.Version
			$ocell.Offset(0,4).Value2 = $node.InnerText
			$ocell.Offset(0,5).Value2 = $node2.Version
			$ocell.Offset(0,6).Value2 = $node2.InnerText
			$ocell = $ocell.Offset(1,0)
		}
	}
}

if ($merge)
{
	$result = $target + ".merged"
	Merge-File $source $target $result
	if ($twoways) {
		Merge-File $result $source $($source + ".merged")
	}
	if ($rename) {
		Remove-Item $target 
		Rename-Item $result $target -force
		if ($twoways) {
			Remove-Item $source
			Rename-Item $($source + ".merged") $source -force
		}
	}
}

if ($sort)
{
	Sort-File $($source + ".merged") $($target + ".merged") $($target + ".sorted")
	Clean-File $($source + ".merged") $($target + ".sorted")
}

if ($build)
{
	Build $source $target
}