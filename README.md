# Orleans minimal demo with minimal apis

This is a small sample project to show the performance gain by using an actor model to handle concurrency issues on the application level as opposed to the database level

## Setup

This is a minimal api .net 7.0 app that uses sqlite to create a table with one row where we will try to increase a counter through many concurrent requests.

The DAL is the same and has methods to "reset" the db, update and get information from the table. No EF in order to keep things minimal without many dependencies.

The application spins up a single node orleans cluster (1 silo) with localhost clustering (again to minimize dependencies).

## Endpoints

`/setup` -> Setup the database
`/simple` -> Use DAL directly from http handling
`/orleans` -> Send request through a grain to do the same operation in the DAL

## How to test

Download a tool for stress testing. For example purposes we will use [bombardier](https://github.com/codesenberg/bombardier). We assume that we are in the root of the repository, let's run the application

```bash
dotnet restore
dotnet build
dotnet run
```

This will start our webserver at port 5129. In another terminal window let's run our tests

```bash
# Setup the database (this will create a demo.db file in the directory where you )
curl http://localhost:5129/setup
# ok 

# Run the stress tests for the simple solution (for this example use 100 threads to send 10000 requests)
.\bombardier.exe -c 100 -n 10000 http://localhost:5129/simple

# Note the result (use Ctrl-C to stop)

# Run the stress tests using the orleans grain (for this example use 100 threads to send 10000 requests)
.\bombardier.exe -c 100 -n 10000 http://localhost:5129/orleans
```

### Results 

Running the above commands creates the following outputs.

#### Simple

```
Bombarding http://localhost:5129/simple with 1000 request(s) using 100 connection(s)
 1000 / 1000 [=======================================================================================================================] 100.00% 63/s 15s
Done!
Statistics        Avg      Stdev        Max
  Reqs/sec        63.30     313.67    3705.25
  Latency         1.48s   844.22ms      9.47s
  HTTP codes:
    1xx - 0, 2xx - 1000, 3xx - 0, 4xx - 0, 5xx - 0
    others - 0
  Throughput:    15.06KB/s
```

#### Orleans

```
Bombarding http://localhost:5129/orleans with 1000 request(s) using 100 connection(s)
 1000 / 1000 [=======================================================================================================================] 100.00% 82/s 12s
Done!
Statistics        Avg      Stdev        Max
  Reqs/sec        82.83      30.06     190.19
  Latency         1.16s   235.74ms      1.57s
  HTTP codes:
    1xx - 0, 2xx - 1000, 3xx - 0, 4xx - 0, 5xx - 0
    others - 0
  Throughput:    18.51KB/s
```