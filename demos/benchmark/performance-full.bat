echo "Calling execute"
ab -n 1000 -c 75 -e execute.csv localhost:62562/Expensive/Execute 
echo "Calling execute async"
ab -n 1000 -c 75 -e executeAsync.csv localhost:62562/Expensive/ExecuteAsync
