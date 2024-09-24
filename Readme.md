# Proyecto: Sistema de Permisos con Kafka, Elasticsearch y Kibana

Este proyecto utiliza **Kafka** para manejar eventos, **Elasticsearch** para almacenar y consultar datos, y **Kibana** para visualizar los índices de Elasticsearch. Todo está dockerizado para facilitar la configuración y ejecución.

## Requisitos

- Docker
- Docker Compose

## Instrucciones para correr el proyecto con Docker

1. Clona el repositorio del proyecto:

   ```bash
   git clone <url-del-repositorio>
   cd <nombre-del-proyecto>
   ```

2. Construye y levanta los contenedores:

   ```bash
   docker-compose up --build
   ```

   Esto levantará los siguientes servicios:
   - **Kafka**: Broker de mensajería.
   - **Elasticsearch**: Sistema de búsqueda y análisis.
   - **Kibana**: Interfaz gráfica para visualizar datos en **Elasticsearch**.
   - **Zookeeper**: Necesario para Kafka.
   - **Backend**: La API que interactúa con Kafka y Elasticsearch.

3. Verifica que los servicios están corriendo correctamente:

   - **Kafka**: Disponible en `localhost:9092`.
   - **Elasticsearch**: Disponible en `localhost:9200`.
   - **Kibana**: Disponible en `localhost:5601`.
   - **Backend**: La API debe estar disponible en swagger `http://localhost:5000/swagger/index.html` o en el puerto especificado en el archivo `docker-compose.yml` .

## Ver los mensajes en Kafka

### Listar los topics en Kafka

Para ver los **topics** que están corriendo en **Kafka**, ejecuta el siguiente comando dentro del contenedor de **Kafka**:

1. Accede al contenedor de Kafka:

   ```bash
   docker exec -u 0 -it kafka /bin/bash
   ```

2. Lista los topics disponibles:

   ```bash
   kafka-topics --list --bootstrap-server localhost:9092
   ```

	En caso de no tener ningun topic disponible crealo con el siguiente comando:

	 ```bash
   	kafka-topics --create --topic permissions-topic --bootstrap-server localhost:9092 --partitions 1 --replication-factor 1
   ```


### Consumir mensajes de un topic en Kafka

Para ver los mensajes publicados en un topic de **Kafka**:

1. Dentro del contenedor de **Kafka**, consume los mensajes de un topic:

   ```bash
   kafka-console-consumer --bootstrap-server kafka:9092 --topic permissions-topic --from-beginning
   ```

Este comando mostrará todos los mensajes que han sido publicados en ese **topic** desde el principio.

## Ver los índices en Elasticsearch y Kibana

### Ver índices en Elasticsearch

Puedes consultar los índices almacenados en **Elasticsearch** con la API de Elasticsearch:

1. Abre un navegador web y dirígete a:

   ```
   http://localhost:9200/_cat/indices?v
   ```

   Esto te devolverá una lista de los índices creados en **Elasticsearch**.

### Acceder a Kibana

**Kibana** proporciona una interfaz gráfica para consultar los índices de **Elasticsearch**.

1. Abre tu navegador y accede a **Kibana** en la siguiente dirección:

   ```
   http://localhost:5601
   ```

2. Una vez en **Kibana**:
   - Ve a **Discover** en el menú de la izquierda.
   - Configura el **index pattern** para que coincida con los índices de **Elasticsearch** (por ejemplo, `logs-*` si los índices tienen ese formato).
   - Luego podrás explorar los datos e índices almacenados en **Elasticsearch**.

## Ejecutar los tests dentro de Docker

Si tienes tests para ejecutar, puedes correrlos dentro de Docker de la siguiente manera:

1. Modifica el archivo **Dockerfile** para incluir la ejecución de los tests (ya incluido en el Dockerfile si sigues las instrucciones anteriores).
   
2. Ejecuta el siguiente comando para levantar los contenedores y correr los tests:

   ```bash
   docker-compose up --build
   ```

3. Revisa los resultados de los tests en los logs del contenedor:

   ```bash
   docker logs <nombre_del_contenedor_backend>
   ```

## Limpiar los recursos

Para detener los contenedores y limpiar los recursos (contenedores, redes, volúmenes):

```bash
docker-compose down --volumes --remove-orphans
```