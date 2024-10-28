#!/bin/bash
REPOSITORIES=(RapidDelivery.APIGateway RapidDelivery.Services.Deliveries RapidDelivery.Services.Notifications)

echo ${REPOSITORIES[@]} | sed -E -e 's/[[:blank:]]+/\n/g' | xargs -I {} -n 1 -P 0 sh -c 'printf "========================================================\nUpdating the repository: {}\n========================================================\n"; git -C {} checkout develop; git -C {} pull; git -C {} checkout master; git -C {} pull; git -C {} checkout develop;'
