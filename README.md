# z019

## Note

[EOD Historical Data](https://eodhd.com/)

## Docker

- Port
	- Config Type: Port
	- Name: Http
	- Container Port: 8080
	- Connection Type: TCP
- Path
	- Config Type: Path
	- Name: App Data
	- Container Path: /app/data
	- Access Mode: Read/Write

```
docker run
  -d
  --name='z019website'
  --net='bridge'
  -e TZ="America/Los_Angeles"
  -e HOST_OS="Unraid"
  -e HOST_HOSTNAME="Unraid2"
  -e HOST_CONTAINERNAME="z019website"
  -e 'EODHistoricalData__ApiToken'='secretApiToken'
  -l net.unraid.docker.managed=dockerman
  -p '8080:8080/tcp'
  -p '8081:8081/tcp'
  -v '/mnt/user/z019/':'/app/data':'rw' 'danh955/z019website:latest'
```