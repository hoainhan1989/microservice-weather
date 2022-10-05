set -e

aws ecr get-login-password --region ap-southeast-1 --profile weather-ecr-agent | docker login --username AWS --password-stdin 633282583713.dkr.ecr.ap-southeast-1.amazonaws.com
docker build -f ./Dockerfile -t cloud-weather-temperature:latest .
docker tag cloud-weather-temperature:latest 633282583713.dkr.ecr.ap-southeast-1.amazonaws.com/cloud-weather-temperature:latest
docker push 633282583713.dkr.ecr.ap-southeast-1.amazonaws.com/cloud-weather-temperature:latest