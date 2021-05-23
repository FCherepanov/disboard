# Disboard
Disaster Dashboard by 'Траектория времени' for Energomach@leadersofdigital, 2021

## Build frontend
1. cd front
2. npm install && npm run build

## Run frontend
1. npm start

## Build backend
1. cd back/Disboard.WebApi
2. docker build -t disboard .

## Run backend
1. docker run -it --rm -d -p 80:80 disboard

## External files

### admin_level_4.json

https://mydata.biz/ru/catalog/databases/borders_ru 
File name: "Адм-территориальные границы РФ в формате GeoJSON.zip"