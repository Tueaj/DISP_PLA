# Readme
## Prerequisites
- Kubectl pointing at K8s cluter
- Installed docker
- Installed helm
## Start up
run ./startUpBash.sh

## K8s host name 
Det er satup til, at K8s host navn er "localhost". Hvis dette skal ændres, kan det gøres i charts/tlpims-service/templates/ingress.yaml linje 16.name

## Flow
Her beskrives hvordan projekts flow, for at oprette en order skal udføres, efter at start up er kørt.

-Det antages, at host name ikke er ændret, og er "localhost"

-Der vil i systemet, være auto genereret data for inventory og credit, som bliver lagt i databasen under startup.

1) Swagger for credit servie tilgåes for at hente et credit id http://localhost/credit/swagger/index.html - Dette skal bruges, når en order skal creates gennem front-enden.

2) Swagger for inventory servie tilgåes for at hente et item ids http://localhost/credit/swagger/index.html - Dette skal bruges, når en order skal creates gennem front-enden.

3) http://localhost/ tilgåes, hvor en order kan oprettes med de fundne id'er, og en order kan oprettes.

4) Swagger for order servie tilgåes for at hente et den order som er oprettet, og tjekke status på denne order http://localhost/order/swagger/index.html