# Connect Online Site
Connect-PnPOnline -Url "https://ywzyj.sharepoint.com/sites/wcontosoteam" -UseWebLogin
$site = Get-PnPSite
Get-PnPProperty -ClientObject $site -Property Id #285c9600-eb55-4263-a1cc-afb99eba839c 
# Create List
New-PnPList -Title GraphQueryWithMaxRetry -Template GenericList
#Get the ID
Get-PnPList -Identity Lists/GraphQueryWithMaxRetry #60be66d7-95dc-494a-82c8-226642fbb6a5
# Add Columns
Add-PnPField -List "GraphQueryWithMaxRetry" -InternalName "Person" -DisplayName "Person" -Type User -AddToDefaultView
Add-PnPField -List "GraphQueryWithMaxRetry" -InternalName "choiceDropdown" -DisplayName "choiceDropdown" -Type Choice -Choices  "Stockholm","Helsinki","Oslo" -AddToDefaultView

Add-PnPListItem -List "GraphQueryWithMaxRetry" -Values @{"Title" = "Test Title"; "Person"="basantpandey@ywzyj.onmicrosoft.com"; "choiceDropdown"="Helsinki"}