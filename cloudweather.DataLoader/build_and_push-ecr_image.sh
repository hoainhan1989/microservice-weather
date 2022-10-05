set -e

aws ecr get-login-password --region ap-southeast-1 --profile weather-ecr-agent | docker login --username AWS --password-stdin 633282583713.dkr.ecr.ap-southeast-1.amazonaws.com
docker build -f ./Dockerfile -t cloud-weather-data-loader:latest .
docker tag cloud-weather-data-loader:latest 633282583713.dkr.ecr.ap-southeast-1.amazonaws.com/cloud-weather-data-loader:latest
docker push 633282583713.dkr.ecr.ap-southeast-1.amazonaws.com/cloud-weather-data-loader:latest