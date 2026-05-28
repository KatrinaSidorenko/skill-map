## Build an image

docker build -t skillmap-core-service -f ./core-service/Dockerfile .

## Run a container

docker run -p 8080:8080 --name skillmap-core skillmap-core-service

## Search for image

docker images | grep skillmap-core

docker compose -f compose.yaml up

docker exec -it skillmap-kafka-broker kafka-topics --bootstrap-server localhost:9092 --delete --topic roadmap-workspace-actions

docker exec -it skillmap-kafka-broker kafka-topics --create --topic roadmap-workspace-actions --bootstrap-server localhost:9092 --partitions 3 --replication-factor 1

docker exec -it skillmap-kafka-broker kafka-topics --create --topic roadmap-workspace-actions --bootstrap-server localhost:9092 --partitions 1 --replication-factor 1

docker exec -it skillmap-kafka-broker kafka-topics --create --topic workspace-action-reviewed --bootstrap-server localhost:9092 --partitions 1 --replication-factor 1

docker exec -it skillmap-dev-kafka-broker-1  kafka-topics 
--create --topic roadmap-workspace-actions --bootstrap-server localhost:9092 --partitions 1 --replication-factor 1 

docker exec -it skillmap-dev-kafka-broker-1  kafka-topics  --create --topic workspace-action-reviewed --bootstrap-server localhost:9092 --partitions 1 --replication-factor 1 

docker compose -f compose.dev.yaml --env-file .env.dev up -d --build

## FE
docker build --build-arg NEXT_PUBLIC_API_URL=https://localhost:7066/api -t skillmap-fe .
docker images | grep skillmap-fe
docker run -p 3000:3000 --name skillmap-fe skillmap-fe


## Neo4j
docker run --rm `
  --volumes-from skillmap-dev-roadmap-catalog-db-1 `
  -v "C:\Users\Lenovo\Desktop\restore:/backup" `
  neo4j:5.9 `
  neo4j-admin database load neo4j --from-path=/backup --overwrite-destination=true

docker run --rm `  --volumes-from skillmap-dev-roadmap-blueprints-db-1 `  -v "C:\Users\Lenovo\Desktop\restore:/backup" `  neo4j:5.24 `  neo4j-admin database load neo4j --from-path=/backup --overwrite-destination=true