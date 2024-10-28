#!/bin/bash
REPOSITORIES=(RapidDelivery.APIGateway RapidDelivery.Services.Deliveries RapidDelivery.Services.Notifications)

for REPOSITORY in ${REPOSITORIES[*]}
do
	 echo ========================================================
	 echo Updating the repository: $REPOSITORY
	 echo ========================================================
	 cd $REPOSITORY && git checkout develop && git pull && git checkout master && git pull && cd ..
done
