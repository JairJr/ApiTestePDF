﻿# [TechChallenge fase 5](https://github.com/JairJr/ApiTestePDF/blob/Fase5/README.md)

## Introdução

- Projeto criado para inicialmente exemplificar uma POC de geração de PDFs a partir de um template HTML, utilizando o iTextSharp.

- Detalhes do Projeto:
  1. Endpoint para solicitar a geração do pdf a partir de um HTML com a possibilidade de referenciar outros PDFs a serem anexados ao documento final.
  2. Endpoint para consultar os status das solicitações de geração de PDFs.
  3. Utilizamos entity framework para realizar as migrations e registros iniciais de processamento.
  4. Utilizamos Dapper para as atualizações de status em execuções dentro de threads.
  5. Azure Database para salvar os dados referente ao processamento.
![Diagrama sem nome drawio](https://github.com/JairJr/ApiTestePDF/assets/29376086/82eb7fe6-1c08-4601-aed2-fc67ea418af6)

- Objetivo - Fase 5
	- O Tech Challenge desta fase será a publicação do seu projeto em um cluster Kubernetes.
 	- Ressaltamos que o seu microsserviço pode ser o projeto desenvolvido em uma das fases anteriores ou mesmo o desenvolvimento de um novo projeto.   
  	- A proposta deste Tech Challenge é elaborar um arquivo Dockerfile para o seu projeto e realizar sua publicação dentro de um cluster Kubernetes. Este projeto não precisa ser publicado no AKS, pode ser um cluster local.

### Conforme definido o entregável deverá ser um vídeo demonstrando o projeto rodando dentro de um cluster Kubernetes, segue o [video](https://youtu.be/kKP9etnSxcI)
  
## Funcionalidades


   -> ElmahCore para observabilidade de possiveis falhas ou erros
   ![image](https://github.com/JairJr/TechChallenge2/assets/29376086/c9fa0bb7-c340-46ee-88df-b4716551f0fb)
   ![image](https://github.com/JairJr/ApiTestePDF/assets/29376086/a5886e57-b418-4b89-a448-2988e7ab5f60)


### Minikube para execução do cluster Kubernetes
  
  - Minikube é uma ferramenta que facilita a execução de um cluster Kubernetes em um ambiente local, por meio de uma máquina virtual.
  
#### Seguem os comandos utilizados para execução no cluster:
-iniciar minikube

	minikube start --cpus 2 --memory 6000 --driver=hyperv 
-Ajustar para que o docker aponte para o minikube

	minikube -p minikube docker-env --shell powershell | Invoke-Expression
-build da imagem docker

	docker build -t jairtechchallenge:latest .
-Após o build da imagem, é necessário criar o deployment e o service que irá expor o serviço executando a partir da pasta do projeto (é importante que seja ajustada a conexão no arquivo yaml antes deste passo)

   	kubectl create -f .\k8s\api-fase5.yaml
-Para verificar se o serviço foi criado com sucesso, execute o comando abaixo

	kubectl get svc
-Para verificar se o pod foi criado com sucesso, execute o comando abaixo

	kubectl get pods
-Para acessar o serviço, execute o comando abaixo, pois já identifica o ip e a porta de exposição da VM do minikube

	minikube service pdf-api
-Para acessar o dashboard do kubernetes, execute o comando abaixo

	minikube dashboard
 ![image](https://github.com/JairJr/ApiTestePDF/assets/29376086/8039b8d7-1038-40ab-91e3-ca1b9b0a48a6)

-Para escalar o serviço, execute o comando abaixo

	kubectl scale --replicas 3 deployment pdf-api
 ![image](https://github.com/JairJr/ApiTestePDF/assets/29376086/e5ceb3e4-a6e4-4648-987a-26a43313a119)

-Para verificar se o serviço foi escalado podemos validar com o seguinte comando

	kubectl get pods

### Docker para publicação da solução
  ![image](https://github.com/JairJr/ApiTestePDF/assets/29376086/236807e6-ce72-4297-aa95-c28a780f542f)

- para que o projeto possa ser executado no docker é necessário acessar a pasta do projeto, ajustar a connectionstring e executar os seguintes comandos através do terminal:
  
  		docker build -t {apelidodopacote}:latest .
  		docker run -p 8080:5000 -e ASPNETCORE_ENVIRONMENT=Development {apelidodopacote}:{versão} --name pdf-api

### Documentações e Referencias 
- Docker [Documentação do docker para .NET](https://docs.docker.com/language/dotnet/build-images/)
- Minikube [Documentação oficial](https://minikube.sigs.k8s.io/docs/)
- ElmahCore [Documentação do ElmahCore](https://github.com/ElmahCore/ElmahCore)
- Dapper [Repositório](https://github.com/DapperLib/Dapper)
- iText [Repositório](https://github.com/itext/itext-dotnet/)




