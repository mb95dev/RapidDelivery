#!/bin/bash
REPOSITORIES=(RapidDelivery.APIGateway RapidDelivery.Services.Deliveries RapidDelivery.Services.Notifications)

for REPOSITORY in ${REPOSITORIES[*]}
do
	 echo ========================================================
	 echo Cloning the repository: $REPOSITORY
	 echo ========================================================
	 REPO_URL=https://github.com/mb95dev/$REPOSITORY.git
	 git clone $REPO_URL
	 cd $REPOSITORY && cd ..
done
