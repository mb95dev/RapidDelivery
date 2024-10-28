#!/bin/bash
REPOSITORIES=(RapidDelivery.APIGateway RapidDelivery.Services.Deliveries RapidDelivery.Services.Notifications)

echo ${REPOSITORIES[@]} | sed -E -e 's/[[:blank:]]+/\n/g' | xargs -I {} -n 1 -P 0 sh -c 'printf "========================================================\nCloning repository: {}\n========================================================\n"; git clone https://github.com/mb95dev/{}.git'
