#!/bin/bash

captureStats() {
  local task="$1"
  local container="$2"
  local log_file="docker_stats_$task.log"
  local stop_file="stop_capture_$task"

  rm -f "$stop_file"

  while [ ! -f "$stop_file" ]; do
    docker stats --no-stream --format "{{.Name}}: CPU {{.CPUPerc}} | Mem {{.MemUsage}} | Net {{.NetIO}} | Block {{.BlockIO}}" >> "$log_file" &
    sleep 0.2
  done

  rm -f "$stop_file"
}

executeTask() {
  local task="$1"
  local container="$2"
  local command="$3"
  local iterations="${4:-5}"

  echo "Executing task '$task' $iterations times for benchmarking..."

  for ((i=1; i<=iterations; i++)); do
    echo "Iteration $i for task '$task'..."
    echo ""

    local log_file="docker_stats_$task.log"
    local stop_file="stop_capture_$task"
    : > "$log_file"
    
    captureStats "$task" "$container" &
    stats_pid=$!
    
    sleep 2
    
    start_time=$(date +%s%N)
    echo "----------------------------------------------------------------------------------------"
    docker exec -it "$container" /bin/bash -c "$command"
    end_time=$(date +%s%N)
    exec_time=$(((end_time - start_time) / 1000000))
    
    touch "$stop_file"
    wait $stats_pid

    echo "Iteration: $i: $exec_time ms"
    echo ""
    
    sleep 2
    
    echo "Iteration: $i: $exec_time ms" >> "$log_file"
    cat "$log_file" >> "$task"
    echo >> "$task"
    
    echo "----------------------------------------------------------------------------------------"
  done
  echo "Finished executing task '$task' $iterations times."
  rm "docker_stats_$task.log"
  
  echo "########################################################################################"
  echo ""
}

# -----------------------------------------------------------------------------
# Edit your commands here
# -----------------------------------------------------------------------------

importer="adv-db-systems.importer"
app="adv-db-systems.app"
db="adv-db-systems.memgraph"

#preprocessingCommand=""
importCommand='./dbimporter /data'

task10Command='./dbcli 10 10'
task16Command='./dbcli 16 "Tourism_in_Uttarakhand" 5'                                             # <- 6 is to high
task17Command='./dbcli 17 "Wikipedia_administration_by_MediaWiki_feature" "1880s_films"'
task18Command='./dbcli 18 "Wikipedia_administration_by_MediaWiki_feature" "1880s_films" 10 50'    # <- 15 / 50 is to high

# -----------------------------------------------------------------------------
# Uncomment commands for tasks you want to run. Dont change anything else :)
# -----------------------------------------------------------------------------

echo "Starting benchmarking..."

#executeTask "preprocessing" "$importer" "$preprocessingCommand" 1
executeTask "import" "$importer" "$importCommand" 1

iterationsNumber=5
executeTask "largestNumberOfChildren" "$app" "$task10Command" "$iterationsNumber"
executeTask "neighborhoodPopularity" "$app" "$task16Command" "$iterationsNumber"
executeTask "shortestPathPopularity" "$app" "$task17Command" "$iterationsNumber"
executeTask "directPathWithHighPopularity" "$app" "$task18Command" "$iterationsNumber"

echo "Benchmarking completed. Results saved to individual task files."
