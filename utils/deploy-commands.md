## Build an image
docker build -t skillmap-core-service -f ./core-service/Dockerfile .

## Run a container
docker run -p 8080:8080 --name skillmap-core skillmap-core-service

## Search for image
docker images | grep skillmap-core