## Build an image
docker build -t skillmap-core-service -f ./core-service/Dockerfile .

## Run a container
docker run -p 8080:8080 --name skillmap-core skillmap-core-service

## Search for image
docker images | grep skillmap-core

docker compose -f compose.yaml up

docker exec -it skillmap-kafka-broker kafka-topics --bootstrap-server localhost:9092 --delete --topic roadmap-workspace-actions

docker exec -it skillmap-kafka-broker kafka-topics --create --topic roadmap-workspace-actions --bootstrap-server localhost:9092 --partitions 3 --replication-factor 1