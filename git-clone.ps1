$repos = ("RapidDelivery.APIGateway", "RapidDelivery.Services.Deliveries", "RapidDelivery.Services.Notifications")

foreach($repo in $repos) {
    Write-Host "=========================================="
    Write-Host "Cloning the Repository: "$repo
    Write-Host "=========================================="
    $repo_url = "https://github.com/devmentors/"+$repo+".git"
    git clone $repo_url
}
