echo "Calling execute"
ab -n 100 -c 75 -e execute.csv localhost:62562/Expensive/Execute
echo "Calling execute async"
ab -n 100 -c 75 -e executeAsync.csv localhost:62562/Expensive/ExecuteAsync
