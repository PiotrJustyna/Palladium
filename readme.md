# readme

Execute asynchronous, long-running tests in Orleans and produce TRX results via an HTTP web api endpoint.

Good application for this are large suites of smoke/integration tests that take seconds to run each. With this solution, such tests can be distributed across a cluster of many Orleans silos, executed concurrently and their results can be combined into a trx file required by build pipelines.

Below is a simple scenario illustrating execution of 9 long-running asynchronous tests on 3 orleans silos:

![](diagram.png)

![](https://social.msdn.microsoft.com/Forums/getfile/1008208) ([source](https://social.msdn.microsoft.com/Forums/en-US/77d66d8b-f923-4242-a3f2-a2c6591478e1/render-contents-of-trx-file-in-tfs-build-tab?forum=tfsgeneral))

As Orleans is built for much larger scale, this solution scales for many concurrently executing tests (tens, hundreds, thousands, etc.) and that is where it shines.

## run

To run, use any of the methods listed below:

* `./run-silo-local.sh`
* `./run-silo-docker.sh`
* run it from your IDE, but please make sure you have the environment variables set to e.g. `DASHBOARDPORT=8081;GATEWAYPORT=3001;PRIMARYPORT=2001;SILOPORT=2001;TESTSAPIPORT=5001`

To verify everything is working correctly:

* Dashboard: http://localhost:8081
* API: http://localhost:5001/asynchronoustests

Provided the ports are not changed.