# readme

Smoke tests in Orleans.

## run

To run, use any of the methods listed below:

* `./run-silo-local.sh`
* `./run-silo-docker.sh`
* run it from your IDE, but please make sure you have the environment variables set to e.g. `DASHBOARDPORT=8081;GATEWAYPORT=3001;PRIMARYPORT=2001;SILOPORT=2001;SMOKETESTSAPIPORT=5000`

To verify everything is working correctly:

* Dashboard: http://localhost:8081
* API: http://localhost:5000/smoketests

Provided the ports are not changed.