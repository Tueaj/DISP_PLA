#!/bin/bash
docker image build DISP -t "frontend:v1.0"
docker image build node-disp -t "shipment-service:v1.0"
docker image build -f "DISP_Saga/CreditService/Dockerfile" DISP_Saga -t "credit-service:v1.0"
docker image build -f "DISP_Saga/InventoryService/Dockerfile" DISP_Saga -t "inventory-service:v1.0"
docker image build -f "DISP_Saga/OrderService/Dockerfile" DISP_Saga -t "order-service:v1.0"

helm upgrade --install ingress-nginx charts/ingress-nginx 
helm upgrade --install community-operator charts/community-operator
helm upgrade --install rabbitmq-cluster-operator charts/rabbitmq-cluster-operator
helm upgrade --install mongodb-cluster charts/mongodb-cluster
helm upgrade --install rabbitmq-cluster charts/rabbitmq-cluster

helm dependency update charts/frontend
helm upgrade --install frontend charts/frontend

helm dependency update charts/order-service
helm upgrade --install order-service charts/order-service

helm dependency update charts/credit-service
helm upgrade --install credit-service charts/credit-service

helm dependency update charts/inventory-service
helm upgrade --install inventory-service charts/inventory-service

helm dependency update charts/shipment-service
helm upgrade --install shipment-service charts/shipment-service