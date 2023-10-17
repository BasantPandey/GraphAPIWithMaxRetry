# Graph API With Max Retry

Graph API With MaxRetry Example

## Setup Your System

1. Create Site Collection in your Tenant (Get the Site ID)
2. Create a List a SharePoint List (Get the List ID)
3. Use Graph Explorer to test your quries
4. Use Client ID and Client certificate to access to token.
5. Use [Graph Developer API](https://github.com/pnp/proxy-samples) [Testing tool](<[Title](https://github.com/microsoft/m365-developer-proxy)>) to check the retry attempts in action.

### Create Site collection

1. Login to SharePoint Admin create site collection
2. Using team site template to create the site.
   ![Alt text](images/image-1.png)

### Setup your data

```
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

```

## Execute Graph Qeury with Retry Project

1. Execute the code without proxy tool.
1. ![Alt text](images/image.png)
1. Download exe from [Graph API Proxy tool](<[Title](https://github.com/microsoft/m365-developer-proxy)>)

1. Open Command Prompt type.
   `m365proxy --failure-rate 50 --no-mocks --allowed-errors 429`
1. ![Alt text](images/image-2.png)
1. Screen output
   ![Alt text](images/image-3.png)
