#!/bin/bash
RetrieveIp(){
  ifconfig | grep -Eo 'inet (addr:)?([0-9]*\.){3}[0-9]*' | grep -Eo '([0-9]*\.){3}[0-9]*' | grep -v -m1 '127.0.0.1'
}

export ADVERTISEDIP=`RetrieveIp`
export PRIMARYADDRESS=`RetrieveIp`
export GATEWAYPORT=3001
export SILOPORT=2001
export PRIMARYPORT=2001
export DASHBOARDPORT=8081
export TESTSAPIPORT=5001
export CLUSTERNAME='test-cluster'
export SERVICENAME='test-service'

dotnet run --project ./SiloHost/SiloHost.fsproj